using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bogie : MonoBehaviour
{
    public RectTransform directionRectTransform;

    public void UpdateDroneDirection(float angle)
    {
        //Subtract 90 from the angle to point in-line with target.
        Vector3 euler = new Vector3(0, 0, angle - 90);
        directionRectTransform.localEulerAngles = euler;
    }
}
