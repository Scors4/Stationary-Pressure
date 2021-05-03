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

public class GameMgr : MonoBehaviour {
    public static GameMgr inst;
    
    public ResourceSet Resources = new ResourceSet();
    public int Power = 0;
    
    public float timeSurvived = 0.0f;
    public float timeToNextWave = 30.0f;
    bool inWave = false;

    void Awake() {
        inst = this;
    }

    void Start() {}

    // Update is called once per frame
    void Update() {}
    
    void FixedUpdate() {
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
}
