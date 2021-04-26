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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PowerText.text = "Power: " + GameMgr.inst.Power;
        IronText.text = "Iron: " + GameMgr.inst.Resources.Iron;
    }
}
