using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bogie : MonoBehaviour
{
    public RectTransform directionRectTransform;
    public List<Image> images;

    public Text idText;

    public void UpdateDroneDirection(float angle)
    {
        //Subtract 90 from the angle to point in-line with target.
        Vector3 euler = new Vector3(0, 0, angle - 90);
        directionRectTransform.localEulerAngles = euler;
    }

    public void SetDroneOwner(DroneBase drone, OWNERS owner)
    {
        Color droneColor = Color.yellow;
        switch(owner)
        {
            case OWNERS.PLAYER:
                droneColor = Color.green;
                break;
            case OWNERS.RAIDER:
                droneColor = Color.red;
                break;
            case OWNERS.NEUTRAL:
                droneColor = Color.white;
                break;
        }

        foreach (Image image in images)
            image.color = droneColor;

        idText.color = droneColor;

        UserDrone uDrone = drone.GetComponent<UserDrone>();
        if(uDrone != null)
        {
            idText.text = uDrone.type.ToString("d2") + "-" + uDrone.id.ToString("d3");
        }
    }

}
