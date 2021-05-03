using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum RESOURCE
{
    IRON,
    COPPER,
    URANIUM
}

/// A set of all possible resources, excluding power
public class ResourceSet {
    public float Iron = 0.0f;
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    public List<Asteroid> asteroids = new List<Asteroid>();
    
    public ResourceSet Resources = new ResourceSet();
    public int Power = 0;
    
    public GameObject asteroidSpawn;
    public GameObject asteroidPrefab;
    public int numAsteroids = 100;
    
    public float timeSurvived = 0.0f;
    public float timeToNextWave = 30.0f;
    bool inWave = false;

    // Start is called before the first frame update
    void Awake() {
        inst = this;
    }

    void Start() {
        trySpawnAsteroids();
    }

    // Update is called once per frame
    void Update() {}
    
    void FixedUpdate() {
        trySpawnAsteroids();
        
        timeSurvived = Time.timeSinceLevelLoad;
        
        if (!inWave) {
            if(timeToNextWave <= 0.0f) {
                timeToNextWave = 0.0f;
                inWave = true;
                DroneMgr.inst.SpawnRaiderWave();
            } else {
                timeToNextWave -= Time.fixedDeltaTime;
            }
        } else if (inWave) {
            if(DroneMgr.inst.GetRaiderDrones().Count == 0) {
                inWave = false;
                
                // TODO: Consider making dynamic
                timeToNextWave = 30.0f;
            }
        }
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
}
