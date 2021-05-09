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
    public Transform centerPoint;
    public float maxAttachDistance = 4;

    ResourceSet currentResourceCost;
    DroneStatSet currentDroneStats;

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

        currentResourceCost = new ResourceSet();
        currentDroneStats = new DroneStatSet();

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

        SerializableDictionary<RESOURCE, float> costsIncurred = part.GetResourceCost();
        if(costsIncurred.ContainsKey(RESOURCE.IRON))
            currentResourceCost.Iron += costsIncurred[RESOURCE.IRON];
        if (costsIncurred.ContainsKey(RESOURCE.COPPER))
            currentResourceCost.Copper += costsIncurred[RESOURCE.COPPER];
        if (costsIncurred.ContainsKey(RESOURCE.URANIUM))
            currentResourceCost.Uranium += costsIncurred[RESOURCE.URANIUM];


        SerializableDictionary<DroneStatFields, float> statsAdded = part.GetStatFields();
        if (statsAdded.ContainsKey(DroneStatFields.HEALTH))
            currentDroneStats.Health += statsAdded[DroneStatFields.HEALTH];
        if (statsAdded.ContainsKey(DroneStatFields.POWER))
            currentDroneStats.Power+= statsAdded[DroneStatFields.POWER];
        if (statsAdded.ContainsKey(DroneStatFields.FUEL))
            currentDroneStats.Fuel += statsAdded[DroneStatFields.FUEL];
        if (statsAdded.ContainsKey(DroneStatFields.STORAGE))
            currentDroneStats.Storage += statsAdded[DroneStatFields.STORAGE];

        UpdateCostDisplay();
        UpdateAttachDistLimit();
    }

    public void PartRemoved(DronePart part)
    {
        DronePart[] removedParts = part.GetComponentsInChildren<DronePart>();

        foreach(DronePart removedPart in removedParts)
        {
            SerializableDictionary<RESOURCE, float> costsIncurred = part.GetResourceCost();
            if (costsIncurred.ContainsKey(RESOURCE.IRON))
                currentResourceCost.Iron -= costsIncurred[RESOURCE.IRON];
            if (costsIncurred.ContainsKey(RESOURCE.COPPER))
                currentResourceCost.Copper -= costsIncurred[RESOURCE.COPPER];
            if (costsIncurred.ContainsKey(RESOURCE.URANIUM))
                currentResourceCost.Uranium -= costsIncurred[RESOURCE.URANIUM];

            SerializableDictionary<DroneStatFields, float> statsRemoved = removedPart.GetStatFields();
            if (statsRemoved.ContainsKey(DroneStatFields.HEALTH))
                currentDroneStats.Health -= statsRemoved[DroneStatFields.HEALTH];
            if (statsRemoved.ContainsKey(DroneStatFields.POWER))
                currentDroneStats.Power -= statsRemoved[DroneStatFields.POWER];
            if (statsRemoved.ContainsKey(DroneStatFields.FUEL))
                currentDroneStats.Fuel -= statsRemoved[DroneStatFields.FUEL];
            if (statsRemoved.ContainsKey(DroneStatFields.STORAGE))
                currentDroneStats.Storage -= statsRemoved[DroneStatFields.STORAGE];
        }

        /*SerializableDictionary<RESOURCE, float> costIncurred = part.GetResourceCost();
        if (costIncurred.ContainsKey(RESOURCE.IRON))
            currentResourceCost.Iron -= costIncurred[RESOURCE.IRON];
        if (costIncurred.ContainsKey(RESOURCE.COPPER))
            currentResourceCost.Copper -= costIncurred[RESOURCE.COPPER];
        if (costIncurred.ContainsKey(RESOURCE.URANIUM))
            currentResourceCost.Uranium -= costIncurred[RESOURCE.URANIUM];

        SerializableDictionary<DroneStatFields, float> statRemoved = part.GetStatFields();
        if (statRemoved.ContainsKey(DroneStatFields.HEALTH))
            currentDroneStats.Health -= statRemoved[DroneStatFields.HEALTH];
        if (statRemoved.ContainsKey(DroneStatFields.POWER))
            currentDroneStats.Power -= statRemoved[DroneStatFields.POWER];
        if (statRemoved.ContainsKey(DroneStatFields.FUEL))
            currentDroneStats.Fuel -= statRemoved[DroneStatFields.FUEL];
        if (statRemoved.ContainsKey(DroneStatFields.STORAGE))
            currentDroneStats.Storage -= statRemoved[DroneStatFields.STORAGE];*/


        

        UpdateCostDisplay();
    }

    public void UpdateCostDisplay()
    {
        string cost = "";

        cost += "Iron: " + currentResourceCost.Iron.ToString("#.0") + "\n";
        cost += "Copper: " + currentResourceCost.Copper.ToString("#.0") + "\n";
        cost += "Uranium: " + currentResourceCost.Uranium.ToString("#.0");

        costText.text = cost;
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
            if (display.GetDisplayedPart() != null)
            {
                Destroy(display.GetDisplayedPart().gameObject);
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

    void UpdateAttachDistLimit()
    {
        foreach(AttachPoint point in GetComponentsInChildren<AttachPoint>())
        {
            if (!point.isActiveAndEnabled)
                continue;

            if(Vector3.Distance(point.transform.position, centerPoint.position) > maxAttachDistance)
            {
                point.gameObject.SetActive(false);
            }
        }
    }

    bool ValidateDroneToPrint()
    {
        bool print = true;

        if (currentDroneStats.Health <= 0)
            print = false;

        if (currentDroneStats.Power <= 0 && currentDroneStats.Storage <= 0)
            print = false;

        return print;
    } 

    public void PrintDrone() {
        if (ValidateDroneToPrint())
            DroneMgr.inst.SpawnUserDrone(currentResourceCost, currentDroneStats);
    }
}
