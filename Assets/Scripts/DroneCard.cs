using UnityEngine;
using UnityEngine.UI;

public class DroneCard : MonoBehaviour
{
    UserDrone drone;

    public Text droneName;
    public Text droneID;
    public Text resourceText;

    public Button launchButton;
    public Button powerButton;
    public Button returnButton;
    public Button autoLaunchButton;

    public Text healthPercent;
    public Text powerPercent;
    public Text resourcePercent;

    public Scrollbar healthBar;
    public Scrollbar powerBar;
    public Scrollbar resourceBar;

    //int barWidth;
    bool autoLaunch = false;
    float timeToAutoLaunch = 20.0f;

    private void Start()
    {
        launchButton.interactable = false;
        returnButton.interactable = false;
    }

    internal void SetDrone(UserDrone drone)
    {
        this.drone = drone;

        droneID.text = drone.GetStringIdentifier();

        //barWidth = 340;
    }

    private void OnDisable()
    {
        this.drone = null;
    }

    private bool ValidateDrone()
    {
        return drone != null;
    }

    private void FixedUpdate()
    {
        if (drone == null)
            Destroy(gameObject);

        if (drone.isDocked)
        {
            if (autoLaunch && timeToAutoLaunch > 0.0f)
                timeToAutoLaunch -= Time.fixedDeltaTime;
            else if (timeToAutoLaunch <= 0.0f)
                AutoLaunchMiner();
        }

        UpdatePercentageDisplay();
        UpdateTextDisplay();

        UpdateButtonState();
    }

    public void OnReturnClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);

        drone.ReturnToBase();
        returnButton.interactable = false;
    }

    public void OnLaunchClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);

        drone.LaunchDrone();
        launchButton.interactable = false;
    }

    public void OnPowerClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);

        Destroy(drone.gameObject);
        Destroy(gameObject);
    }

    public void OnAutoLaunchClick()
    {
        autoLaunch = !autoLaunch;
        ColorBlock c = autoLaunchButton.colors;
        c.normalColor = autoLaunch ? Color.green : Color.white;
        autoLaunchButton.colors = c;
    }

    void UpdateButtonState()
    {
        if (!returnButton.interactable && drone.GetDroneState() != UserDroneState.ReturnToBase && !drone.isDocked)
            returnButton.interactable = true;

        if (!launchButton.interactable && drone.isDocked)
            launchButton.interactable = true;
        else if (!drone.isDocked && launchButton.interactable)
            launchButton.interactable = false;
    }

    public void UpdatePercentageDisplay()
    {
        DroneStatSet currentStats;
        DroneStatSet maxStats;

        if (drone == null)
            return;

        drone.GetDroneStats(out currentStats, out maxStats);

        // Stat percentages for both bar and text display
        float hp = currentStats.Health / maxStats.Health;
        float pp = currentStats.Power / maxStats.Power;
        float rp = 0.0f;
        if(currentStats.Storage != 0.0f) 
            rp = currentStats.Storage / maxStats.Storage;

        /*// Bar relative y locations
        float hpos = (int)(hp * (-0.5 * barWidth));
        float ppos = (int)(pp * (-0.5 * barWidth));
        float rpos = (int)(rp * (-0.5 * barWidth));

        // Bar widths
        float hw = (int)(hp * barWidth);
        float pw = (int)(pp * barWidth);
        float rw = (int)(rp * barWidth);*/

        healthPercent.text = "%" + (hp * 100).ToString("f2");
        powerPercent.text = "%" + (pp * 100).ToString("f2");
        resourcePercent.text = "%" + (rp * 100).ToString("f2");

        healthBar.size = Mathf.Clamp(hp, 0.0f, 1.0f);
        powerBar.size = Mathf.Clamp(pp, 0.0f, 1.0f);
        resourceBar.size = Mathf.Clamp(rp, 0.0f, 1.0f);
    }

    public void UpdateTextDisplay()
    {
        ResourceSet res = drone.minedResources;
        resourceText.text = "Iron: " + res.Iron.ToString("#.0") + "\n"
            + "Copper: " + res.Copper.ToString("#.0") + "\n"
            + "Uranium: " + res.Uranium.ToString("#.0") + "\n"
            + "Ice: " + res.Ice.ToString("#.0");
    }

    public void UpdateStaticText()
    {
        droneID.text = drone.GetStringIdentifier();
    }

    public bool GetAutoLaunch()
    {
        return autoLaunch;
    }

    public void BeginAutoLaunch()
    {
        timeToAutoLaunch = 20.0f;
    }

    void AutoLaunchMiner()
    {
        drone.LaunchDrone();
    }
}
