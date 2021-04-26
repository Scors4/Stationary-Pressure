using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum RESOURCE
{
    IRON
}

/// A set of all possible resources, excluding power
public class ResourceSet {
    public float Iron = 0.0f;
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    public List<Asteroid> asteroids;
    
    public ResourceSet Resources = new ResourceSet();
    public int Power = 0;
    
    public GameObject asteroidSpawn;
    public GameObject asteroidPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        inst = this;
        if(asteroids == null)
            asteroids = new List<Asteroid>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate() {
        for(int i = GetAsteroids().Count; i < 100; i++) {
            SpawnAsteroid();
        }
    }

    public List<Asteroid> GetAsteroids()
    {
        // To filter out dead asteriods.
        // TODO: Consider calling a method on GameMgr to destroy asteroids instead of just nuking the GameObject.
        List<Asteroid> filtered = new List<Asteroid>();
        
        foreach(Asteroid a in asteroids) {
            if(a != null) {
                filtered.Add(a);
            }
        }
        
        return filtered;
    }
    
    public void SpawnAsteroid() {
        Vector3 offset = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-10.0f, 10.0f), Random.Range(-100.0f, 100.0f));
        Vector3 position = asteroidSpawn.transform.position + offset;
        
        Instantiate(asteroidPrefab, position, asteroidSpawn.transform.rotation);
    }
}
