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
        
        GameMgr.inst.asteroids.Add(this);    
    }

    // Update is called once per frame
    void Update() {}
    
    /// Get the asteroid's position
    public Vector3 getPosition() {
        return transform.position;
    }
    
    /// Try to mine this asteroid
    public void Mine() {        
        if(Resources.Iron > 0.0f) {
            Resources.Iron -= 1.0f;
            GameMgr.inst.Resources.Iron += 1.0f;
        }
        
        if(IsEmpty()) {
            GameMgr.inst.asteroids.Remove(this);
            gameObject.GetComponent<Fracture>().FractureObject();
        }
    }
    
    /// Whether this asteroid is empty
    public bool IsEmpty() {
        return Resources.Iron == 0.0f;
    }
}
