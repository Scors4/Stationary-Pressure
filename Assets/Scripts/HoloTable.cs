using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloTable : MonoBehaviour, Interactable
{
    public List<Transform> partDisplays;
    public List<GameObject> partsList;

    private bool inBuildMode = false;

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
}
