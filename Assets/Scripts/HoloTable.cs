using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoloTable : MonoBehaviour
{
    public List<PartDisplay> partDisplays;
    List<GameObject> partsList;
    public Text pageText;
    public Text costText;

    SerializableDictionary<RESOURCE, float> currentResourceCost;
    SerializableDictionary<DroneStatFields, float> currentDroneStats;

    public Button buildModeButton;

    protected AttachPoint selectedAttachPoint;

    private bool inBuildMode = false;
    private int page = 0;
    private int pageCount = 0;

    private void Start()
    {
        GameObject[] parts = Resources.LoadAll<GameObject>("Drone Parts");
        partsList = new List<GameObject>();
        partsList.AddRange(parts);

        currentResourceCost = new SerializableDictionary<RESOURCE, float>();
        currentDroneStats = new SerializableDictionary<DroneStatFields, float>();

        UpdatePage();
        UpdateBuildMode();

        pageCount = (partsList.Count / partDisplays.Count);
        pageText.text = "Page " + (page + 1) + " of " + (pageCount + 1);
    }

    public void SelectAttachPoint(AttachPoint newPoint)
    {
        if(selectedAttachPoint != null)
            selectedAttachPoint.SetSelected(false);
        
        selectedAttachPoint = newPoint;
        selectedAttachPoint.SetSelected(true);

        UpdateValidPartCheck();
    }

    public void AddPart(int displayIndex, int anchorIndex)
    {
        if (selectedAttachPoint == null)
            return;

        if (page * partDisplays.Count + displayIndex > partsList.Count)
            return;

        GameObject obj = GameObject.Instantiate(partsList[page * partDisplays.Count + displayIndex], selectedAttachPoint.transform);
        DronePart part = obj.GetComponent<DronePart>();
        Transform anchor = part.GetAnchorPoint(anchorIndex);

        obj.transform.localEulerAngles = (Vector3.up * 180) - anchor.localEulerAngles;
        obj.transform.localPosition = -(anchor.localRotation * anchor.localPosition);

        obj.transform.SetParent(selectedAttachPoint.transform.parent, true);

        part.AttachToAnchor(anchorIndex);
        part.SetParentAttachPoint(selectedAttachPoint);
        selectedAttachPoint.SetSelected(false);
        selectedAttachPoint.gameObject.SetActive(false);
        selectedAttachPoint = null;

        SerializableDictionary<RESOURCE, float> costsRemoved = part.GetResourceCost();
        foreach (RESOURCE res in costsRemoved.Keys)
        {
            if (!currentResourceCost.ContainsKey(res))
                currentResourceCost[res] = 0;

            currentResourceCost[res] = currentResourceCost[res] + costsRemoved[res];
        }

        SerializableDictionary<DroneStatFields, float> statsRemoved = part.GetStatFields();
        foreach (DroneStatFields stat in statsRemoved.Keys)
        {
            if (!currentDroneStats.ContainsKey(stat))
                currentDroneStats[stat] = 0;

            currentDroneStats[stat] = currentDroneStats[stat] - statsRemoved[stat];
        }
    }

    public void PartRemoved(DronePart part)
    {
        DronePart[] removedParts = part.GetComponentsInChildren<DronePart>();

        foreach(DronePart removedPart in removedParts)
        {
            SerializableDictionary<RESOURCE, float> costsRemoved = removedPart.GetResourceCost();
            foreach(RESOURCE res in costsRemoved.Keys)
            {
                currentResourceCost[res] = currentResourceCost[res] - costsRemoved[res];
            }

            SerializableDictionary<DroneStatFields, float> statsRemoved = removedPart.GetStatFields();
            foreach(DroneStatFields stat in statsRemoved.Keys)
            {
                currentDroneStats[stat] = currentDroneStats[stat] - statsRemoved[stat];
            }
        }

        SerializableDictionary<RESOURCE, float> costRemoved = part.GetResourceCost();
        foreach (RESOURCE res in costRemoved.Keys)
        {
            currentResourceCost[res] = currentResourceCost[res] - costRemoved[res];
        }

        SerializableDictionary<DroneStatFields, float> statRemoved = part.GetStatFields();
        foreach (DroneStatFields stat in statRemoved.Keys)
        {
            currentDroneStats[stat] = currentDroneStats[stat] - statRemoved[stat];
        }
    }

    public void OnBuildModeClick()
    {
        inBuildMode = !inBuildMode;
        Player.player.ToggleBuildMode(inBuildMode);
        UpdateBuildMode();
    }

    public void OnPageUpClick()
    {
        page++;
        if (page > pageCount)
            page = 0;

        UpdatePage();
    }

    public void OnPageDownClick()
    {
        page--;
        if (page < 0)
            page = pageCount;

        UpdatePage();
    }

    void UpdateBuildMode()
    {
        foreach(PartDisplay display in partDisplays)
        {
            display.gameObject.SetActive(inBuildMode);
        }
    }

    void UpdatePage()
    {
        pageText.text = "Page " + (page + 1) + " of " + (pageCount + 1);

        int i = page * partDisplays.Count;
        foreach (PartDisplay display in partDisplays)
        {
            if (display.transform.childCount > 0)
            {
                Destroy(display.transform.GetChild(0).gameObject);
                display.SetDisplayedPart(null);
            }

            if (i < partsList.Count)
            {
                GameObject obj = GameObject.Instantiate(partsList[i], transform);
                DronePart part = obj.GetComponent<DronePart>();

                obj.transform.localScale = part.displayScale;

                obj.transform.SetParent(display.transform, true);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;

                part.enabled = false;
                display.SetDisplayedPart(part);

                foreach (AttachPoint point in obj.GetComponentsInChildren<AttachPoint>())
                {
                    point.gameObject.SetActive(false);
                }
            }
            else
                break;

            i++;
        }

        UpdateValidPartCheck();
    }

    void UpdateValidPartCheck()
    {
        if (selectedAttachPoint == null)
        {
            foreach (PartDisplay display in partDisplays)
                display.SetPartValid(true);
        }
        else
        { 

            foreach(PartDisplay display in partDisplays)
            {
                DronePart part = display.GetDisplayedPart();
                if (part != null)
                {
                    display.SetPartValid((part.GetAttachType() & selectedAttachPoint.GetAttachType()) != 0);
                }
            }
        }
    }


    public void PrintDrone() {
        // TODO: Pass more info to DroneMgr for instantiation
        DroneMgr.inst.SpawnUserDrone();
    }
}
