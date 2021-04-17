using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A class to continually rotate an entity to make it seem more like an asteroid.
public class Asteroid : MonoBehaviour
{
    private Transform transform = null;
    
    public Vector3 RotateVelocity = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
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
}
