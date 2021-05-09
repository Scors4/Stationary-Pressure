using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMgr : MonoBehaviour
{
    public static DroneMgr inst;

    public List<UserDrone> userDrones = new List<UserDrone>();
    public List<RaiderDrone> raiderDrones = new List<RaiderDrone>();
    public List<DroneBase> allDrones = new List<DroneBase>();

    public Transform homeSite;

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

            DroneBase db = newDrone.GetComponent<DroneBase>();
            if (db != null)
                allDrones.Add(db);
        }
    }

    public void UserDroneDestroyed(UserDrone destroyed) {
        if (userDrones.Contains(destroyed)) {
            userDrones.Remove(destroyed);
            droneList.DroneDestroyed(destroyed);

            DroneBase db = destroyed.GetComponent<DroneBase>();
            if (db != null)
                allDrones.Remove(db);
        }
    }
    
    public void RaiderDroneSpawned(RaiderDrone newDrone) {
        if (!raiderDrones.Contains(newDrone)) {
            raiderDrones.Add(newDrone);

            DroneBase db = newDrone.GetComponent<DroneBase>();
            if (db != null)
                allDrones.Add(db);
        }
    }

    public void RaiderDroneDestroyed(RaiderDrone destroyed) {
        raiderDrones.Remove(destroyed);

        DroneBase db = destroyed.GetComponent<DroneBase>();
        if (db != null)
            allDrones.Remove(db);
    }

    public List<UserDrone> GetUserDrones() {
        return userDrones;
    }
    
    public List<RaiderDrone> GetRaiderDrones() {
        return raiderDrones;
    }
    
    public List <DroneBase> GetAllDrones()
    {
        return allDrones;
    }

    bool CanPrintDrone(ResourceSet resourceCost)
    {
        ResourceSet currentSupply = GameMgr.inst.Resources;
        return resourceCost.Iron <= currentSupply.Iron
            && resourceCost.Copper <= currentSupply.Copper
            && resourceCost.Uranium <= currentSupply.Uranium;
    }

    public bool SpawnUserDrone(ResourceSet resourceCost, DroneStatSet droneStats) {
        // TODO: Calculate cost dynamically

        if (!CanPrintDrone(resourceCost))
            return false;

        GameMgr.inst.Resources.Iron -= resourceCost.Iron;
        GameMgr.inst.Resources.Copper -= resourceCost.Copper;
        GameMgr.inst.Resources.Uranium -= resourceCost.Uranium;
            
        Vector3 offset = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        Vector3 position = userDroneSpawn.transform.position + offset;
            
        GameObject go = Instantiate(userDronePrefab, position, userDroneSpawn.transform.rotation);
        go.GetComponent<DroneBase>().SetDroneStats(droneStats);
        go.GetComponent<UserDrone>().SetDroneHome(homeSite);
            
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
