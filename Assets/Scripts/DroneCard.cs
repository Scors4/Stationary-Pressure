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

    internal void SetDrone(UserDrone drone)
    {
        this.drone = drone;
        drone.hasCard = true;

        droneID.text = drone.type.ToString("d2") + "-" + drone.id.ToString("d3");
    }

    private void OnDisable()
    {
        drone.hasCard = false;
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
}
