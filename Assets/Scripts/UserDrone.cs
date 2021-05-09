using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserDroneState {
    ReturnToBase,
    Normal,
}

public class UserDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    static int nextId = 0;
    public int id = 0;
    
    public DroneCard droneCard = null;
    public int type = 0;

    public ResourceSet minedResources;
    public bool isDocked = false;
    
    bool isEngagingRaider = false;
    bool isReturningHome = false;
    public UserDroneState state = UserDroneState.Normal;
    
    public Transform homeBase = null;
    
    // Start is called before the first frame update
    void Start() {
        droneBase = GetComponent<DroneBase>();
        minedResources = new ResourceSet();

        id = nextId++;
        DroneMgr.inst.UserDroneSpawned(this);
        

        DroneBase drone = GetComponent<DroneBase>();
        if (drone != null)
            drone.SetOwner(OWNERS.PLAYER);
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed Timestep
    void FixedUpdate() {
        if(droneBase.GetDroneCurrentStats().Health <= 0.0) {
            DroneMgr.inst.UserDroneDestroyed(this);
            Destroy(gameObject);
            return;
        }

        if (type != 2 && !droneBase.HasStorageAvailable() && !isReturningHome)
            ReturnToBase();

        if (type != 1 && !droneBase.HasPower() && !isReturningHome)
            ReturnToBase();
        
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
                this.ChangeState(UserDroneState.Normal);
            }
        } else {

            if (type != 1)
            {
                RaiderDrone raiderDrone = DroneMgr.inst.GetClosestRaiderDrone(transform.position);

                if (isEngagingRaider && droneBase.isIdle())
                    isEngagingRaider = false;

                if (raiderDrone != null)
                {
                    Vector3 positionDiff = raiderDrone.transform.position - droneBase.transform.position;
                    if (positionDiff.magnitude <= 25.0f && Quaternion.Angle(Quaternion.LookRotation(positionDiff.normalized), transform.rotation) < 5.0)
                    {
                        raiderDrone.GetDroneBase().DoDamage(droneBase.DrawPower(droneBase.damage));
                    }
                }


                if (!isEngagingRaider && raiderDrone != null)
                {
                    isEngagingRaider = true;
                    droneBase.ClearCommands();

                    droneBase.addCommand(new PursueTargetToRadiusCommand(raiderDrone.GetDroneBase()));
                }
                else if (type != 2 && droneBase.isIdle())
                {
                    Asteroid asteroid = AsteroidMgr.inst.getClosestAsteroid(transform.position);

                    if (asteroid != null && asteroid.isActiveAndEnabled)
                    {
                        droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
                        droneBase.addCommand(new ZeroVelocityCommand(droneBase));
                        droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
                        droneBase.addCommand(new MineAsteroidCommand(asteroid, this));
                    }
                }
            }
            else
            {
                Asteroid asteroid = AsteroidMgr.inst.getClosestAsteroid(transform.position);

                if (asteroid != null && asteroid.isActiveAndEnabled)
                {
                    droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
                    droneBase.addCommand(new ZeroVelocityCommand(droneBase));
                    droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
                    droneBase.addCommand(new MineAsteroidCommand(asteroid, this));
                }
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

    public void SetDroneType(int typeIn)
    {
        type = typeIn;
        droneCard.UpdateStaticText();
    }
}
