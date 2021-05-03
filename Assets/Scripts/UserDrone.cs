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
        if (droneBase.isIdle()){
            Asteroid asteroid = GameMgr.inst.getClosestAsteroid(transform.position);
            
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
}
