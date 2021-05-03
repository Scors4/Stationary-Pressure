using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMgr : MonoBehaviour {
    public static AsteroidMgr inst;
     
    public List<Asteroid> asteroids = new List<Asteroid>();
    
    public GameObject asteroidSpawn;
    public GameObject asteroidPrefab;
    public int numAsteroids = 100;
    
    void Awake() {
        inst = this;
    }
    
    // Start is called before the first frame update
    void Start() {
        trySpawnAsteroids();
    }

    // Update is called once per frame
    void Update() {}
    
    /// Fixed Timestep
    void FixedUpdate() {
        trySpawnAsteroids();
    }
    
    void trySpawnAsteroids() {
        for(int i = asteroids.Count; i < numAsteroids; i++)
            SpawnAsteroid();
    }
    
    public void SpawnAsteroid() {
        Vector3 offset = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-10.0f, 10.0f), Random.Range(-100.0f, 100.0f));
        Vector3 position = asteroidSpawn.transform.position + offset;
        
        Instantiate(asteroidPrefab, position, asteroidSpawn.transform.rotation);
    }
    
    public Asteroid getClosestAsteroid(Vector3 pos) {
        int minIndex = -1;
        float magSqr = float.MaxValue;
        
        for(int i = 0; i < asteroids.Count; i++) {
            float newMagSqr = (asteroids[i].getPosition() - pos).sqrMagnitude;
            if(newMagSqr < magSqr) {
                magSqr = newMagSqr;
                minIndex = i;
            }
        }
        
        if(minIndex == -1) {
            return null;
        } else {
            return asteroids[minIndex];
        }
    }
    
    public void AsteroidSpawned(Asteroid asteroid) {
        asteroids.Add(asteroid); 
    }
    
    public void AsteroidDestroyed(Asteroid asteroid) {
        asteroids.Remove(asteroid);
        asteroid.gameObject.GetComponent<Fracture>().FractureObject();
    }
}
