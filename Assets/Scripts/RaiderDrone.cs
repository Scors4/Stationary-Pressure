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
        if(droneBase.health <= 0.0) {
            DroneMgr.inst.RaiderDroneDestroyed(this);
            Destroy(gameObject);
            return;
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
