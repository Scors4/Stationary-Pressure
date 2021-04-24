using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMgr : MonoBehaviour
{
    public static DroneMgr inst;

    List<DroneBase> drones;

    void Awake()
    {
        inst = this;
        drones = new List<DroneBase>();
    }

    public void DroneSpawned(DroneBase newDrone)
    {
        if (!drones.Contains(newDrone))
            drones.Add(newDrone);
    }

    public void DroneDestroyed(DroneBase destroyed)
    {
        if (drones.Contains(destroyed))
            drones.Remove(destroyed);
    }

    public List<DroneBase> GetDrones()
    {
        return drones;
    }
}
