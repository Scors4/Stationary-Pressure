using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMgr : MonoBehaviour
{
    public Text eventTitle;
    public Text requiredResources;
    public Text eventCountdown;
    public Text delinquencyText;

    [Range(60.0f, 3600.0f)]
    public float defaultEventTime = 600.0f;

    ResourceSet eventCost;
    bool inDelinquency = false;
    public float timeToEvent = 0.0f;
    float delinquencyTimer = 0.0f;
    float nextDelinquencyWave = 0.0f;
    int timesCompleted = 0;
    int delinquencySpawnCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        eventCost = new ResourceSet();
        eventCost.Iron = 5000;
        eventCost.Copper = 2500;
        eventCost.Uranium = 0;
        eventCost.Ice = 1500;

        timeToEvent = defaultEventTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeToEvent > 0.0f)
        {
            if (inDelinquency)
            {
                inDelinquency = false;
                delinquencyText.gameObject.SetActive(true);
                delinquencyTimer = 0.0f;
                nextDelinquencyWave = 0.0f;
            }
            timeToEvent -= Time.fixedDeltaTime;
        }
        else
        {
            if(!inDelinquency)
            {
                inDelinquency = true;
                delinquencyText.gameObject.SetActive(true);
            }
            delinquencyTimer += Time.fixedDeltaTime;
            nextDelinquencyWave += Time.fixedDeltaTime;

            if (nextDelinquencyWave >= 180.0f)
            {
                nextDelinquencyWave = 0.0f;

                //TODO: Spawn raider drones here.
                for (int i = 0; i < (delinquencySpawnCount * 2) + 3; i++)
                    DroneMgr.inst.SpawnRaiderDrone();

                delinquencySpawnCount++;
            }
        }

        requiredResources.text = "Iron: " + eventCost.Iron.ToString("#.0") + "\n" +
            "Copper: " + eventCost.Copper.ToString("#.0") + "\n" +
            "Uranium: " + eventCost.Uranium.ToString("#.0") + "\n" +
            "Ice: " + eventCost.Ice.ToString("#.0") + "\n";

        eventCountdown.text = "Time to event: " + Util.formatTime(timeToEvent);

        if (inDelinquency)
            delinquencyText.text = "<%Delinquency Notice%>" + "\n" +
                "<%DelTimerName%>: " + Util.formatTime(delinquencyTimer);
    }

    public bool ValidateResources()
    {
        GameMgr game = GameMgr.inst;
        return game.Resources.Iron >= eventCost.Iron &&
            game.Resources.Copper >= eventCost.Copper &&
            game.Resources.Uranium >= eventCost.Uranium &&
            game.Resources.Ice >= eventCost.Ice;

    }

    public void SendResourcesClick()
    {
        if (!ValidateResources())
            return;

        GameMgr.inst.Resources.Iron -= eventCost.Iron;
        GameMgr.inst.Resources.Copper -= eventCost.Copper;
        GameMgr.inst.Resources.Uranium -= eventCost.Uranium;
        GameMgr.inst.Resources.Ice -= eventCost.Ice;

        timesCompleted += 1;

        eventCost.Iron = (Random.Range(1000, 5000) * timesCompleted);
        eventCost.Copper = (Random.Range(1000, 2500) * timesCompleted);
        eventCost.Uranium = (Random.Range(0, 2500) * timesCompleted);
        eventCost.Ice = (Random.Range(500, 1500) * timesCompleted);

        timeToEvent = Random.Range(defaultEventTime - 240f, defaultEventTime + 600f);
    }
}
