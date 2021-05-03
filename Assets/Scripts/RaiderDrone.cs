using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderDrone : MonoBehaviour
{
    DroneBase droneBase = null;
    
    // Start is called before the first frame update
    void Start()
    {
        droneBase = GetComponent<DroneBase>();
        
        DroneMgr.inst.RaiderDroneSpawned(this);
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed timestep
    void FixedUpdate() {
        
    }
}
