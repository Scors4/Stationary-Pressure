using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMgr : MonoBehaviour
{
    public static DroneMgr inst;

    public List<UserDrone> userDrones = new List<UserDrone>();
    public List<RaiderDrone> raiderDrones = new List<RaiderDrone>();
    
    public GameObject userDroneSpawn;
    public GameObject userDronePrefab;
    
    public GameObject raiderDroneSpawn;
    public GameObject raiderDronePrefab;
    
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
    
    public void RaiderDroneSpawned(RaiderDrone newDrone) {
        if (!raiderDrones.Contains(newDrone)) {
            raiderDrones.Add(newDrone);
        }
    }

    public void RaiderDroneDestroyed(RaiderDrone destroyed) {
        raiderDrones.Remove(destroyed);
    }

    public List<UserDrone> GetUserDrones() {
        return userDrones;
    }
    
    public List<RaiderDrone> GetRaiderDrones() {
        return raiderDrones;
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
    
    public bool SpawnRaiderDrone() {   
        // TODO: Add modules to drone based on difficulty
        
        Vector3 offset = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        Vector3 position = raiderDroneSpawn.transform.position + offset;
            
        Instantiate(raiderDronePrefab, position, raiderDroneSpawn.transform.rotation);
            
        return true;
    }
    
    public void SpawnRaiderWave() {
        // TODO: Scale with input difficulty param
        for(int i = 0; i < 5; i++) 
            SpawnRaiderDrone();
    }
    
    public UserDrone GetClosestUserDrone(Vector3 pos) {
        int minIndex = -1;
        float magSqr = float.MaxValue;
        
        for(int i = 0; i < userDrones.Count; i++) {
            float newMagSqr = (userDrones[i].GetDroneBase().GetPosition() - pos).sqrMagnitude;
            if(newMagSqr < magSqr) {
                magSqr = newMagSqr;
                minIndex = i;
            }
        }
        
        if(minIndex == -1) {
            return null;
        } else {
            return userDrones[minIndex];
        }
    }
    
    public RaiderDrone GetClosestRaiderDrone(Vector3 pos) {
        int minIndex = -1;
        float magSqr = float.MaxValue;
        
        for(int i = 0; i < raiderDrones.Count; i++) {
            float newMagSqr = (raiderDrones[i].GetDroneBase().GetPosition() - pos).sqrMagnitude;
            if(newMagSqr < magSqr) {
                magSqr = newMagSqr;
                minIndex = i;
            }
        }
        
        if(minIndex == -1) {
            return null;
        } else {
            return raiderDrones[minIndex];
        }
    }
}
