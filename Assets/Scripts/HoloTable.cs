using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoloTable : MonoBehaviour
{
    public List<Transform> partDisplays;
    List<GameObject> partsList;
    public Text pageText;
    
    public GameObject droneSpawn;
    public GameObject dronePrefab;

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

        UpdatePage();
        UpdateBuildMode();

        pageCount = (partsList.Count / partDisplays.Count);
        pageText.text = "Page " + page+1 + " of " + pageCount+1;
    }

    public void SelectAttachPoint(AttachPoint newPoint)
    {
        if(selectedAttachPoint != null)
            selectedAttachPoint.SetSelected(false);
        
        selectedAttachPoint = newPoint;
        selectedAttachPoint.SetSelected(true);
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
    }

    public void OnBuildModeClick()
    {
        inBuildMode = !inBuildMode;
        Player.player.ToggleBuildMode(inBuildMode);
        UpdateBuildMode();
    }

    public void OnPageUpClick()
    {
        
    }

    public void OnPageDownClick()
    {

    }

    void UpdateBuildMode()
    {
        foreach(Transform display in partDisplays)
        {
            display.gameObject.SetActive(inBuildMode);
        }
    }

    void UpdatePage()
    {

        pageText.text = "Page " + (page + 1) + " of " + (pageCount + 1);

        int i = page * partDisplays.Count;
        foreach (Transform display in partDisplays)
        {
            if(display.childCount > 0)
                Destroy(display.GetChild(0).gameObject);

            if (i < partsList.Count)
            {
                GameObject obj = GameObject.Instantiate(partsList[i], transform);

                obj.transform.SetParent(display, true);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;

                DronePart part = obj.GetComponent<DronePart>();
                part.enabled = false;

                foreach (AttachPoint point in obj.GetComponentsInChildren<AttachPoint>())
                {
                    point.gameObject.SetActive(false);
                }
            }
            else
                break;

            i++;
        }
    }
    
    public void PrintDrone() {
        if(GameMgr.inst.Resources.Iron >= 70.0f){
            GameMgr.inst.Resources.Iron -= 70.0f;
            Instantiate(dronePrefab, droneSpawn.transform.position, droneSpawn.transform.rotation);
        }
    }
}
