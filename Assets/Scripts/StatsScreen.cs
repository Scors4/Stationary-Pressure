using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    /// The label for power
    public Text PowerText;
    /// The label for iron
    public Text IronText;
    /// The label for time
    public Text TimeText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PowerText.text = "Power: " + GameMgr.inst.Power;
        IronText.text = "Iron: " + GameMgr.inst.Resources.Iron;
        
        float time = Time.timeSinceLevelLoad;
        TimeText.text = "Time: " + ((int)Mathf.Floor(time / 60)).ToString("d2") + ":" + (((int)Mathf.Floor(time)) % 60).ToString("d2");
    }
}
