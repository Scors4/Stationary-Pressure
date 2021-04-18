using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloTable : MonoBehaviour, Interactable
{
    public List<Transform> partDisplays;
    public List<GameObject> partsList;

    protected AttachPoint selectedAttachPoint;

    private bool inBuildMode = true;
    private int page = 0;

    private void Start()
    {
        GameObject[] parts = Resources.LoadAll<GameObject>("Drone Parts");
        partsList.AddRange(parts);

        foreach(AttachPoint point in GetComponentsInChildren<AttachPoint>())
        {
            point.gameObject.SetActive(inBuildMode);
        }

        int i = 0;
        foreach(Transform display in partDisplays)
        {
            if(i < partsList.Count)
            {
                GameObject obj = GameObject.Instantiate(partsList[i], transform);
                obj.transform.localPosition = display.localPosition;
                obj.transform.localEulerAngles = display.localEulerAngles + new Vector3(90, 0, 0);

                DronePart part = obj.GetComponent<DronePart>();
                part.enabled = false;

                foreach(AttachPoint point in obj.GetComponentsInChildren<AttachPoint>())
                {
                    point.gameObject.SetActive(false);
                }
            }

            i++;
        }
    }

    public void OnInteraction(GameObject other)
    {
        ToggleBuildMode();
    }

    public void OnHover(GameObject other)
    {
        
    }

    public void ToggleBuildMode()
    {
        inBuildMode = !inBuildMode;
        foreach (AttachPoint point in GetComponentsInChildren<AttachPoint>())
        {
            point.gameObject.SetActive(inBuildMode);
        }
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
        obj.transform.position = (selectedAttachPoint.transform.position - (anchor.localRotation * anchor.localPosition));

        Debug.Log("Vector 3: " + anchor.InverseTransformPoint(obj.transform.position));

        part.AttachToAnchor(anchorIndex);
    }
}
