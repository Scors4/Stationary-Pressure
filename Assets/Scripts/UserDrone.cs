using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    // Start is called before the first frame update
    void Start() {
        droneBase = GetComponent<DroneBase>();
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed Timestep
    void FixedUpdate() {
        if (droneBase.isIdle()){
            Asteroid asteroid = GameMgr.inst.getClosestAsteroid(transform.position);
            
            if(asteroid != null && asteroid.isActiveAndEnabled) {                
                droneBase.addCommand(new ApproachTargetToRadiusCommand(asteroid.transform));
                droneBase.addCommand(new ZeroVelocityCommand(this.droneBase));
                droneBase.addCommand(new RotateToPointCommand(asteroid.transform.position));
                droneBase.addCommand(new MineAsteroidCommand(asteroid));
            }   
        }
    }
}
