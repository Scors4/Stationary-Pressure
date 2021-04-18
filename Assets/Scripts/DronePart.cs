using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePart : MonoBehaviour, Interactable
{
    public Material highlightMat;
    public float mass;
    public SerializableDictionary<RESOURCE, float> resourceDemands;

    public List<AttachPoint> anchorPoints;
    int frameToHide;
    bool isHighlighted = false;
    GameObject highlightObject;

    private void Start()
    {
        highlightObject = new GameObject("Highlight");

        Mesh mesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        Transform newParent = GetComponentInChildren<MeshFilter>().transform;
        MeshFilter meshFilter = highlightObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = highlightObject.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;
        highlightObject.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
        meshRenderer.material = highlightMat;

        highlightObject.transform.SetParent(newParent, false);
        highlightObject.SetActive(false);
    }

    public void OnHover(GameObject other)
    {
        if (!enabled)
            return;

        frameToHide = 3;
        if(!isHighlighted)
        {
            ToggleHighlight();
        }
    }

    public void OnInteraction(GameObject other)
    {

    }

    void FixedUpdate()
    {
        if (frameToHide > 0)
            frameToHide--;
        else if (isHighlighted)
        {
            ToggleHighlight();
        }
    }

    private void ToggleHighlight()
    {
        isHighlighted = !isHighlighted;
        highlightObject.SetActive(isHighlighted);
    }

    public Transform GetAnchorPoint(int index)
    {
        if(anchorPoints == null)
            return null;

        return anchorPoints[index % anchorPoints.Count].transform;
    }

    public void AttachToAnchor(int index)
    {
        if (anchorPoints == null)
            return;

        anchorPoints[index % anchorPoints.Count].gameObject.SetActive(false);
    }

    public int GetAnchorCount()
    {
        return anchorPoints.Count;
    }
}
