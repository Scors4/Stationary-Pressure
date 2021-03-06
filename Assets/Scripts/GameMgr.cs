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

public enum Difficulty {
    Normal,
    Hard,
}

/// A set of all possible resources, excluding power
public class ResourceSet {
    public float Iron = 0.0f;
    public float Copper = 0.0f;
    public float Uranium = 0.0f;
    public float Ice = 0.0f;
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    public static Difficulty difficulty = Difficulty.Normal;

    public GameObject lossScreen;
    public GameObject pauseScreen;
    public FirstPersonController firstPersonController;
    public ResourceSet Resources = new ResourceSet();
    public Transform hangarLocation = null;
    public List<Light> poweredLights;

    public float timeSurvived = 0.0f;
    public float timeToNextWave = 480.0f;
    public float timeToLoss = 60.0f;
    bool inWave = false;
    bool gamePaused = false;
    bool gameLost = false;
    bool lightsPowered = true;

    float power = 60;
    float powerDrainRate = 0.15f;

    void Awake()
    {
        inst = this;
    }

    void Start()
    {
        Debug.Log("Game Difficulty: " + GameMgr.difficulty.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused && DroneMgr.inst.offeringEscape && Input.GetKeyDown(KeyCode.Q))
        {
            power = 0;
            timeToLoss = 0;
            SetGameLost();
        }

        if (!gameLost && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
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

    void FixedUpdate()
    {
        timeSurvived = Time.timeSinceLevelLoad;

        if (!inWave)
        {
            if (timeToNextWave <= 0.0f)
            {
                timeToNextWave = 0.0f;
                inWave = true;
                DroneMgr.inst.SpawnRaiderWave();
            }
            else
            {
                timeToNextWave -= Time.fixedDeltaTime;
            }
        }
        else if (inWave)
        {
            if (DroneMgr.inst.GetRaiderDrones().Count == 0)
            {
                inWave = false;

                // TODO: Consider making dynamic
                timeToNextWave = Random.Range(120.0f, 480.0f);
            }
        }

        if (Resources.Uranium > 0 && power < 250.0f)
        {
            Resources.Uranium -= (0.25f * Time.fixedDeltaTime);
            power += 15.0f * Time.fixedDeltaTime;

            if (Resources.Uranium < 0f)
                Resources.Uranium = 0.0f;
        }

        if (power <= 0)
        {
            if (lightsPowered)
            {
                lightsPowered = false;
                if (poweredLights != null)
                {
                    foreach (Light light in poweredLights)
                        light.enabled = false;
                }
            }

            if (timeToLoss <= 0.0f)
            {
                SetGameLost();
            }
            else
                timeToLoss -= Time.fixedDeltaTime;
        }
        else
        {
            if (!lightsPowered)
            {
                lightsPowered = true;
                if (poweredLights != null)
                {
                    foreach (Light light in poweredLights)
                        light.enabled = true;
                }
            }

            if (timeToLoss < 60.0f)
                timeToLoss = 60.0f;

            power -= Time.fixedDeltaTime * powerDrainRate;
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
        return power;
    }

    public void AddResources(ResourceSet resourcesIn)
    {
        Resources.Iron += resourcesIn.Iron;
        Resources.Copper += resourcesIn.Copper;
        Resources.Uranium += resourcesIn.Uranium;
        Resources.Ice += resourcesIn.Ice;
    }

    public void SetGameLost()
    {
        gameLost = true;
        Time.timeScale = 0.0f;
        lossScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        firstPersonController.cameraCanMove = false;
        firstPersonController.playerCanMove = false;
    }
}
