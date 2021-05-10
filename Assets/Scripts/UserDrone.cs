using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserDroneState {
    ReturnToBase,
    Normal,
}

/// The drone type
public enum DroneType {
    Miner,
    Combat,
}

public class UserDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    static int nextId = 0;
    public int id = 0;
    
    public DroneCard droneCard = null;
    public DroneType type = DroneType.Miner;

    public ResourceSet minedResources;
    public bool isDocked = false;
    
    bool isEngagingRaider = false;
    bool isReturningHome = false;
    public UserDroneState state = UserDroneState.Normal;
    
    public Transform homeBase = null;
    
    void Awake() {
        droneBase = GetComponent<DroneBase>();
        droneBase.SetOwner(OWNERS.PLAYER);
        minedResources = new ResourceSet();
        id = nextId++;
    }
    
    // Start is called before the first frame update
    void Start() {
        DroneMgr.inst.UserDroneSpawned(this);
        if (droneCard != null)
            droneCard.UpdateStaticText();   
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed Timestep
    void FixedUpdate() {
        switch(type) {
            case DroneType.Miner:
                if(!droneBase.HasStorageAvailable() && !isReturningHome) 
                    ReturnToBase();
                break;
            case DroneType.Combat:
                if(!droneBase.HasPower() && !isReturningHome)
                    ReturnToBase();
                break;
            default:
                Debug.LogWarning("Unknown DroneType: " + type);
                break;
        }
        
        if(state == UserDroneState.ReturnToBase) {
            Vector3 positionDiff = homeBase.position - droneBase.transform.position;
            if(positionDiff.magnitude > 25.0f && !isReturningHome) {
                isReturningHome = true;
                
                droneBase.addCommand(new ApproachTargetToRadiusCommand(homeBase));
                droneBase.addCommand(new ZeroVelocityCommand(droneBase));
            }
            else if (droneBase.isIdle() && isReturningHome)
            {
                this.gameObject.SetActive(false);
                isDocked = true;
                UnloadResources();
                droneBase.RefillPower();
                this.ChangeState(UserDroneState.Normal);
            }
        } else {
            switch(type) {
                case DroneType.Miner:
                    Asteroid asteroid = AsteroidMgr.inst.getClosestAsteroid(transform.position);
                    
                    // If valid asteroid is found, mine it
                    if (asteroid != null && asteroid.isActiveAndEnabled) {
                        droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
                        droneBase.addCommand(new ZeroVelocityCommand(droneBase));
                        droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
                        droneBase.addCommand(new MineAsteroidCommand(asteroid, this));
                    }
                    break;
                case DroneType.Combat:
                    RaiderDrone raiderDrone = DroneMgr.inst.GetClosestRaiderDrone(transform.position);
                    
                    // Reset before aquiring new target
                    if (isEngagingRaider && droneBase.isIdle())
                        isEngagingRaider = false;
                    
                    // Do damage to nearest drone in angle/range
                    if (raiderDrone != null) {
                        Vector3 positionDiff = raiderDrone.transform.position - droneBase.transform.position;
                        if (positionDiff.magnitude <= 25.0f && Quaternion.Angle(Quaternion.LookRotation(positionDiff.normalized), transform.rotation) < 5.0)
                            raiderDrone.GetDroneBase().DoDamage(droneBase.DrawPower(droneBase.damage));
                        
                        // If not targeting raider, target raider
                        if(!isEngagingRaider) {
                            isEngagingRaider = true;
                            droneBase.ClearCommands();

                            droneBase.addCommand(new PursueTargetToRadiusCommand(raiderDrone.GetDroneBase()));
                        }
                    }
                    
                    break;
                default:
                    Debug.LogWarning("Unknown DroneType: " + type);
                    break;
            }
        }
    }
    
    void OnEnable() {
        if(DroneMgr.inst != null)
            DroneMgr.inst.UserDroneSpawned(this);
    }
    
    void OnDestroy() {
        DroneMgr.inst.UserDroneDestroyed(this);
    }
    
    public DroneBase GetDroneBase() {
        return droneBase;
    }

    public void GetDroneStats(out DroneStatSet current, out DroneStatSet max)    
    {
        current = droneBase.GetDroneCurrentStats();
        max = droneBase.GetDroneMaxStats();
    }

    public UserDroneState GetDroneState()
    {
        return state;
    }
    
    public void ReturnToBase() {
        this.ChangeState(UserDroneState.ReturnToBase);
    }
    
    void ChangeState(UserDroneState state) {
        droneBase.ClearCommands();
        this.state = state;
        this.isEngagingRaider = false;
        this.isReturningHome = false;
    }

    void UnloadResources()
    {
        GameMgr.inst.AddResources(minedResources);
        minedResources = new ResourceSet();
        droneBase.ClearStorage();
    }
    
    public void LaunchDrone()
    {
        if (!isDocked)
            return;

        this.gameObject.SetActive(true);
        isDocked = false;
    }

    public void SetDroneHome(Transform newHome)
    {
        homeBase = newHome;
    }

    public void SetDroneType(DroneType type)
    {
        this.type = type;
        droneCard.UpdateStaticText();
    }
    
    /// Return the string id of this drone
    public string GetStringIdentifier() {
        int typeId = 0;  
        switch(type) {
            case DroneType.Miner:
                typeId = 1;
                break;
            case DroneType.Combat:
                typeId = 2;
                break;
            default:
                Debug.LogWarning("Unknown DroneType: " + type.ToString());
                break;
        }
        
        return typeId.ToString("d2") + "-" + id.ToString("d3");
    }
}
