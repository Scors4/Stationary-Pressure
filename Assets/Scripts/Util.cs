using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static string formatTime(float time) {
        return ((int)Mathf.Floor(time / 60)).ToString("d2") + ":" + (((int)Mathf.Floor(time)) % 60).ToString("d2");
    }
}
