using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMgr : MonoBehaviour
{
    public static DroneMgr inst;

    List<UserDrone> userDrones = new List<UserDrone>();
    
    public GameObject userDroneSpawn;
    public GameObject userDronePrefab;
    
    public DroneList droneList;

    void Awake() {
        inst = this;
    }

    public void UserDroneSpawned(UserDrone newDrone) {
        if (!userDrones.Contains(newDrone)) {
            userDrones.Add(newDrone);
            droneList.DroneSpawned(newDrone);
        }
    }

    public void UserDroneDestroyed(UserDrone destroyed) {
        if (userDrones.Contains(destroyed)) {
            userDrones.Remove(destroyed);
            droneList.DroneDestroyed(destroyed);
        }
    }

    public List<UserDrone> GetUserDrones() {
        return userDrones;
    }
    
    public bool SpawnUserDrone() {
        // TODO: Calculate cost dynamically
        float cost = 70.0f;
        
        if(GameMgr.inst.Resources.Iron < cost)
            return false;
        
        GameMgr.inst.Resources.Iron -= cost;
            
        Vector3 offset = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        Vector3 position = userDroneSpawn.transform.position + offset;
            
        Instantiate(userDronePrefab, position, userDroneSpawn.transform.rotation);
            
        return true;
    }
}
