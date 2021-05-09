using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    /// The label for power
    public Text PowerText;
    /// The label for iron
    public Text ResourceText;
    /// The label for time
    public Text TimeText;
    /// The time to the next wave
    public Text TimeNextWaveText;
    /// The # of remaining raiders
    public Text RaidersRemaining;
    
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        PowerText.text = "Power: " + GameMgr.inst.GetPowerLevel().ToString("#.0");
        ResourceText.text = "Iron: " + GameMgr.inst.Resources.Iron.ToString("#.0") + "\n"
            + "Copper: " + GameMgr.inst.Resources.Copper.ToString("#.0") + "\n"
            + "Uranium: " + GameMgr.inst.Resources.Uranium.ToString("#.0") + "\n"
            + "Ice: " + GameMgr.inst.Resources.Ice.ToString("#.0");
        
        TimeText.text = "Time: " + Util.formatTime(GameMgr.inst.timeSurvived);
        TimeNextWaveText.text = "Next Wave In: " + Util.formatTime(GameMgr.inst.timeToNextWave);
        
        RaidersRemaining.text = "Raiders Remaining: " + DroneMgr.inst.GetRaiderDrones().Count;
    }
}
