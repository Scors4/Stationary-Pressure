using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBase : MonoBehaviour
{
    private Transform transform = null;
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    
    private float weight = 1.0f; // default to 1.0 to prevent divide-by-zero issues.
    
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Fixed timestep
    void FixedUpdate() {
        velocity += acceleration;
        transform.position += velocity;
    }
}


