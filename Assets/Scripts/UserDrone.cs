using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserDroneState {
    ReturnToBase,
    Idle,
    Mining,
    Combat,
}

/// The drone type
public enum DroneType {
    Hybrid,
    Miner,
    Combat,
}

public class UserDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    static int nextId = 0;
    public int id = 0;
    
    public DroneCard droneCard = null;
    public DroneType type = DroneType.Hybrid;

    public ResourceSet minedResources;
    public bool isDocked = false;
    
    public UserDroneState state = UserDroneState.Idle;
    
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
        if(type == DroneType.Hybrid || type == DroneType.Miner) {
            if(!droneBase.HasStorageAvailable() && state != UserDroneState.ReturnToBase) 
                ReturnToBase();
        }
        
        if(type == DroneType.Hybrid || type == DroneType.Combat) {
            if(!droneBase.HasPower() && state != UserDroneState.ReturnToBase)
                ReturnToBase();
        }
        
        // Do damage to nearest drone in angle/range
        RaiderDrone raiderDrone = DroneMgr.inst.GetClosestRaiderDrone(transform.position);
        if((type == DroneType.Hybrid || type == DroneType.Combat) && raiderDrone != null) {
            Vector3 positionDiff = raiderDrone.transform.position - droneBase.transform.position;
            if (positionDiff.magnitude <= 25.0f && Quaternion.Angle(Quaternion.LookRotation(positionDiff.normalized), transform.rotation) < 5.0)
                raiderDrone.GetDroneBase().DoDamage(droneBase.DrawPower(droneBase.damage));
        }
        
        if(state == UserDroneState.ReturnToBase) {
            // If its done returning to base and not docked...
            if (!isDocked && droneBase.isIdle())
            {
                this.gameObject.SetActive(false);
                isDocked = true;
                UnloadResources();
                droneBase.RefillPower();
                this.ChangeState(UserDroneState.Idle);
                droneCard.BeginAutoLaunch();
            }
        } 
        
        if(state == UserDroneState.Idle) {
            // Combat tasking takes priority
            if(type == DroneType.Hybrid || type == DroneType.Combat) 
                CombatRaider();
            
            // Mine asteroid
            if(type == DroneType.Hybrid || type == DroneType.Miner)
                MineAsteroid();
        }
        
        if(state == UserDroneState.Mining) {
            // Stop mining and attack raiders if they exist
            if(type == DroneType.Hybrid || type == DroneType.Combat) 
                CombatRaider();
            
            // If idle, transition to idle state
            if(droneBase.isIdle()) 
                ChangeState(UserDroneState.Idle);
        }
        
        if(state == UserDroneState.Combat) {
            // If idle, transition to idle state
            if(droneBase.isIdle()) 
                ChangeState(UserDroneState.Idle);
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
        droneBase.addCommand(new ApproachTargetToRadiusCommand(homeBase));
        droneBase.addCommand(new ZeroVelocityCommand(droneBase));
    }
    
    public void MineAsteroid() {
        Asteroid asteroid = AsteroidMgr.inst.getClosestAsteroid(transform.position);
                    
        // If valid asteroid is found, mine it
        if (asteroid != null && asteroid.isActiveAndEnabled) {
            ChangeState(UserDroneState.Mining);
            droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
            droneBase.addCommand(new ZeroVelocityCommand(droneBase));
            droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
            droneBase.addCommand(new MineAsteroidCommand(asteroid, this));
        }
    }
    
    public void CombatRaider() {
        RaiderDrone raiderDrone = DroneMgr.inst.GetClosestRaiderDrone(transform.position);                  
        
        if (raiderDrone != null) {
            ChangeState(UserDroneState.Combat);
            droneBase.addCommand(new PursueTargetToRadiusCommand(raiderDrone.GetDroneBase()));
            droneBase.addCommand(new ZeroVelocityCommand(droneBase));
        }
    }
    
    void ChangeState(UserDroneState state) {
        droneBase.ClearCommands();
        this.state = state;
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
            case DroneType.Hybrid:
                typeId = 0;
                break;
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
