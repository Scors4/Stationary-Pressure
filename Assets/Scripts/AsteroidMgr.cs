using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMgr : MonoBehaviour {
    public static AsteroidMgr inst;
     
    public List<Asteroid> asteroids = new List<Asteroid>();
    
    public GameObject asteroidSpawn;
    public GameObject asteroidPrefab;
    public int numAsteroids = 100;
    [Range(50, 5000)]
    public float spawnRadius = 100.0f;


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
        Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius / 10.0f, spawnRadius / 10.0f), Random.Range(-spawnRadius, spawnRadius));
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
    
    public List<Asteroid> GetAsteroids() {
        return asteroids;
    }
    
    public void AsteroidSpawned(Asteroid asteroid) {
        asteroids.Add(asteroid); 
    }
    
    public void AsteroidDestroyed(Asteroid asteroid) {
        asteroids.Remove(asteroid);
    }
}
