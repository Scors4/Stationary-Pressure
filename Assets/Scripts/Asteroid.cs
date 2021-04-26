using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A class to continually rotate an entity to make it seem more like an asteroid.
public class Asteroid : MonoBehaviour, DroneTargetable
{
    new private Transform transform = null;
    
    public Vector3 RotateVelocity = Vector3.zero;
    
    public ResourceSet Resources = new ResourceSet();
    
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        
        Resources.Iron = 100.0f;
        
        if(GameMgr.inst != null)
            GameMgr.inst.asteroids.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // A Fixed Timestep
    void FixedUpdate() {
        // Rotate for visuals
        transform.Rotate(RotateVelocity);
    }
    
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
            gameObject.GetComponent<Fracture>().FractureObject();
        }
    }
    
    /// Whether this asteroid is empty
    public bool IsEmpty() {
        return Resources.Iron == 0.0f;
    }
}
