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

    public List<Asteroid> GetAsteroids()
    {
        List<Asteroid> filtered = new List<Asteroid>();
        
        foreach(Asteroid a in asteroids) {
            if(a != null) {
                filtered.Add(a);
            }
        }
        
        return filtered;
    }
}
