using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A class to continually rotate an entity to make it seem more like an asteroid.
public class Asteroid : MonoBehaviour, DroneTargetable
{
    public ResourceSet Resources = new ResourceSet();
    
    // Start is called before the first frame update
    void Start() {
        Resources.Iron = 100.0f;
        
        AsteroidMgr.inst.AsteroidSpawned(this);   
    }

    // Update is called once per frame
    void Update() {}
    
    /// Get the asteroid's position
    public Vector3 getPosition() {
        return transform.position;
    }
    
    /// Try to mine this asteroid
    public void Mine(float power) {        
        if(Resources.Iron > 0.0f) {
            Resources.Iron -= power;
            
            // TODO: The player currently gains some extra resources from each "mine" op
            // since the iron levels in an asteroid can go below zero. Fix this.
            GameMgr.inst.Resources.Iron += power;
        }
        
        if(IsEmpty()) {
            AsteroidMgr.inst.AsteroidDestroyed(this);
            gameObject.GetComponent<Fracture>().FractureObject();
        }
        
        Debug.Log(Resources.Iron);
    }
    
    /// Whether this asteroid is empty
    public bool IsEmpty() {
        // TODO: Check for all resources
        return Resources.Iron <= 0.0f;
    }
}
