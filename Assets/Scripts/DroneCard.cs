using System;
using System.Collections;
using System.Collections.Generic;
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

    public Text healthPercent;
    public Text powerPercent;
    public Text resourcePercent;

    public Image healthBar;
    public Image powerBar;
    public Image resourceBar;

    int barWidth;

    internal void SetDrone(UserDrone drone)
    {
        this.drone = drone;

        droneID.text = drone.type.ToString("d2") + "-" + drone.id.ToString("d3");

        barWidth = 340;
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

        UpdatePercentageDisplay();

    }

    public void OnReturnClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);
    }

    public void OnLaunchClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);
    }

    public void OnPowerClick()
    {
        if (!ValidateDrone())
            Destroy(gameObject);
    }

    public void UpdatePercentageDisplay()
    {
        DroneStatSet currentStats;
        DroneStatSet maxStats;

        drone.GetDroneStats(out currentStats, out maxStats);

        // Stat percentages for both bar and text display
        float hp = currentStats.Health / maxStats.Health;
        float pp = currentStats.Power / maxStats.Power;
        float rp = currentStats.Storage / maxStats.Storage;

        // Bar relative y locations
        float hpos = (int)(hp * (-0.5 * barWidth));
        float ppos = (int)(pp * (-0.5 * barWidth));
        float rpos = (int)(rp * (-0.5 * barWidth));

        // Bar widths
        float hw = (int)(hp * barWidth);
        float pw = (int)(pp * barWidth);
        float rw = (int)(rp * barWidth);

        healthPercent.text = "%" + (hp * 100).ToString("f2");
        powerPercent.text = "%" + (pp * 100).ToString("f2");
        resourcePercent.text = "%" + (rp * 100).ToString("f2");

        Rect healthRect = healthBar.rectTransform.rect;
        Rect powerRect = powerBar.rectTransform.rect;
        Rect resourceRect = resourceBar.rectTransform.rect;

        healthRect.width = hw;
        powerRect.width = pw;
        resourceRect.width = rw;

        healthRect.y = hpos;
        powerRect.y = ppos;
        resourceRect.y = rpos;
    }
}
