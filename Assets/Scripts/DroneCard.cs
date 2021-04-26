using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneCard : MonoBehaviour
{
    DroneBase drone;

    public Text droneName;
    public Text droneID;
    public Text resourceText;

    public Button launchButton;
    public Button powerButton;
    public Button returnButton;

    internal void SetDrone(DroneBase drone)
    {
        this.drone = drone;

        droneID.text = drone.type.ToString("d2") + "-" + drone.id.ToString("d3");
    }

    public void OnReturnClick()
    {

    }

    public void OnLaunchClick()
    {

    }

    public void OnPowerClick()
    {

    }
}
