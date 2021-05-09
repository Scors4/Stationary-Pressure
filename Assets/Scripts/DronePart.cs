using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePart : MonoBehaviour, Interactable
{
    public Material highlightMat;
    public float mass;
    public Vector3 displayScale = Vector3.one;

    public bool showName = false;
    public string displayName = "";

    public SerializableDictionary<RESOURCE, float> resourceCost;
    public SerializableDictionary<DroneStatFields, float> statsGiven;

    public List<AttachPoint> anchorPoints;
    public AttachType attachType;
    AttachPoint parentPoint;
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

        UpdateAttachType();
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
        if (!enabled)
            return;

        if (parentPoint != null)
        {
            parentPoint.gameObject.SetActive(true);
            HoloTable ht = GetComponentInParent<HoloTable>();
            ht.PartRemoved(this);
        }
        Destroy(gameObject);
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

    public void SetParentAttachPoint(AttachPoint newParentPoint)
    {
        parentPoint = newParentPoint;
        attachType = parentPoint.attachType & attachType;
        UpdateAttachType();
    }

    public AttachType GetAttachType()
    {
        return attachType;
    }

    public SerializableDictionary<RESOURCE, float> GetResourceCost()
    {
        return resourceCost;
    }

    public SerializableDictionary<DroneStatFields, float> GetStatFields()
    {
        return statsGiven;
    }

    public void UpdateAttachType()
    {
        foreach (AttachPoint point in anchorPoints)
            point.attachType = attachType;
    }
}
