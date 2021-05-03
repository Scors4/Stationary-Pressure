using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bogie : MonoBehaviour
{
    public RectTransform directionRectTransform;
    public List<Material> materials;

    public void UpdateDroneDirection(float angle)
    {
        //Subtract 90 from the angle to point in-line with target.
        Vector3 euler = new Vector3(0, 0, angle - 90);
        directionRectTransform.localEulerAngles = euler;
    }

    public void SetDroneOwner(OWNERS owner)
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

        foreach (Material mat in materials)
            mat.SetColor(0, droneColor);
    }
}
