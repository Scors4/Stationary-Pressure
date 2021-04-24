using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum RESOURCE
{
    IRON
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    public List<Asteroid> asteroids;
    
    public SerializableDictionary<RESOURCE, float> Resources;
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
        return asteroids;
    }
}
