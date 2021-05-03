using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    static int nextId = 0;
    public int id = 0;
    
    public DroneCard droneCard = null;
    public int type = 0;
    
    bool isEngagingRaider = false;
    
    // Start is called before the first frame update
    void Start() {
        droneBase = GetComponent<DroneBase>();
        
        DroneMgr.inst.UserDroneSpawned(this);
        id = nextId++;
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed Timestep
    void FixedUpdate() {
        if(droneBase.health <= 0.0) {
            DroneMgr.inst.UserDroneDestroyed(this);
            Destroy(this);
            return;
        }
        
        RaiderDrone raiderDrone = DroneMgr.inst.GetClosestRaiderDrone(transform.position);
        
        if(isEngagingRaider && raiderDrone == null)
            isEngagingRaider = false;
        
        if(raiderDrone != null) {
            Vector3 positionDiff = raiderDrone.transform.position - droneBase.transform.position;
            if(positionDiff.magnitude <= 25.0f && Quaternion.Angle(Quaternion.LookRotation(positionDiff.normalized), transform.rotation) < 5.0) 
                raiderDrone.GetDroneBase().DoDamage(droneBase.damage);
        }
        
        
        if (!isEngagingRaider && raiderDrone != null) {
            isEngagingRaider = true;
            droneBase.ClearCommands();
            
            droneBase.addCommand(new PursueTargetToRadiusCommand(raiderDrone.GetDroneBase()));
        } else if (droneBase.isIdle()){
            Asteroid asteroid = AsteroidMgr.inst.getClosestAsteroid(transform.position);
            
            if(asteroid != null && asteroid.isActiveAndEnabled) {                
                droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
                droneBase.addCommand(new ZeroVelocityCommand(droneBase));
                droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
                droneBase.addCommand(new MineAsteroidCommand(asteroid));
            }   
        }
    }
    
    void OnEnable() {
        if(DroneMgr.inst != null)
            DroneMgr.inst.UserDroneSpawned(this);
    }
    
    void OnDisable() {
        DroneMgr.inst.UserDroneDestroyed(this);
    }
    
    public DroneBase GetDroneBase() {
        return droneBase;
    }
}
