using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    bool isEngagingUserDrone = false;
    
    void Awake() {
        droneBase = GetComponent<DroneBase>();
    }
    
    // Start is called before the first frame update
    void Start() {
        DroneMgr.inst.RaiderDroneSpawned(this);

        DroneBase drone = GetComponent<DroneBase>();
        if (drone != null)
            drone.SetOwner(OWNERS.RAIDER);

        DroneStatSet droneStats = new DroneStatSet();
        droneStats.Health = 100;

        drone.SetDroneStats(droneStats);
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed timestep
    void FixedUpdate() {
        if(droneBase.GetDroneCurrentStats().Health <= 0.0) {
            DroneMgr.inst.RaiderDroneDestroyed(this);
            Destroy(gameObject);
            return;
        }
        
        UserDrone userDrone = DroneMgr.inst.GetClosestUserDrone(transform.position);
        
        if(isEngagingUserDrone && droneBase.isIdle())
            isEngagingUserDrone = false;
        
        if(userDrone != null) {
            Vector3 positionDiff = userDrone.transform.position - droneBase.transform.position;
            if(positionDiff.magnitude <= 25.0f && Quaternion.Angle(Quaternion.LookRotation(positionDiff.normalized), transform.rotation) < 5.0) 
                userDrone.GetDroneBase().DoDamage(droneBase.damage);
        }
        
        if (!isEngagingUserDrone && userDrone != null) {
            isEngagingUserDrone = true;
            droneBase.ClearCommands();
            
            droneBase.addCommand(new PursueTargetToRadiusCommand(userDrone.GetDroneBase()));
        } else if (droneBase.isIdle()){
            droneBase.addCommand(new ZeroVelocityCommand(droneBase));
           /// TODO: target Base/user 
        }
    }
    
    void OnEnable() {
        if(DroneMgr.inst != null)
            DroneMgr.inst.RaiderDroneSpawned(this);
    }
    
    void OnDisable() {
        DroneMgr.inst.RaiderDroneDestroyed(this);
    }
    
    public DroneBase GetDroneBase() {
        return droneBase;
    }
}
