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
    public float Copper = 0.0f;
    public float Uranium = 0.0f;
    public float Ice = 0.0f;
}

public class GameMgr : MonoBehaviour {
    public static GameMgr inst;

    public GameObject lossScreen;
    public GameObject pauseScreen;
    public FirstPersonController firstPersonController;
    public ResourceSet Resources = new ResourceSet();
    public Transform hangarLocation = null;
    
    public float timeSurvived = 0.0f;
    public float timeToNextWave = 120.0f;
    public float timeToLoss = 60.0f;
    bool inWave = false;
    bool gamePaused = false;
    bool gameLost = false;

    float Power = 60;
    float powerDrainRate = 0.15f;

    void Awake() {
        inst = this;
    }

    void Start() {}

    // Update is called once per frame
    void Update() 
    {

        if(!gameLost && Input.GetKeyDown(KeyCode.Escape))
        {
            if(gamePaused)
            {
                Time.timeScale = 1.0f;
                pauseScreen.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                gamePaused = false;
                firstPersonController.cameraCanMove = true;
                firstPersonController.playerCanMove = true;
            }
            else
            {
                Time.timeScale = 0.0f;
                pauseScreen.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                gamePaused = true;
                firstPersonController.cameraCanMove = false;
                firstPersonController.playerCanMove = false;
            }
        }
    }
    
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
                timeToNextWave = Random.Range(30.0f, 120.0f);
            }
        }

        if(Resources.Uranium > 0 && Power < 250.0f)
        {
            Resources.Uranium -= (0.25f * Time.fixedDeltaTime);
            Power += 1.0f * Time.fixedDeltaTime;

            if (Resources.Uranium < 0f)
                Resources.Uranium = 0.0f;
        }

        if (Resources.Ice > 0.0f && Power > 0.0f)
        {
            Resources.Ice -= 0.2f * Time.fixedDeltaTime;
        }

        if (Power <= 0)
        {
            if (timeToLoss <= 0.0f)
            {
                gameLost = true;
                Time.timeScale = 0.0f;
                lossScreen.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                firstPersonController.cameraCanMove = false;
                firstPersonController.playerCanMove = false;
            }
            else
                timeToLoss -= Time.fixedDeltaTime;
        }
        else
        {
            if (timeToLoss < 60.0f)
                timeToLoss = 60.0f;

            Power -= Time.fixedDeltaTime * powerDrainRate;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
        firstPersonController.cameraCanMove = true;
        firstPersonController.playerCanMove = true;
    }

    public float GetPowerLevel()
    {
        return Power;
    }
}
