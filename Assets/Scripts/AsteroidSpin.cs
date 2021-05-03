using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpin : MonoBehaviour
{
    public Vector3 rotateVelocity = Vector3.zero;
    
    // Start is called before the first frame update
    void Start() {
        rotateVelocity.x = Random.Range(-0.5f, 0.5f);
        rotateVelocity.y = Random.Range(-0.5f, 0.5f);
        rotateVelocity.z = Random.Range(-0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed timestep
    void FixedUpdate() {
        transform.Rotate(rotateVelocity);
    }
}
