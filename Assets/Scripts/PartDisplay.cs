using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDisplay : MonoBehaviour, Interactable
{
    public int selfIndex = 0;
    private int anchorIndex = 0;

    int frameToHide = 0;
    bool isHovered = false;
    bool isPartValid = true;

    public Material hoveredMaterial;
    public Material invalidMaterial;
    protected Material originalMaterial;

    MeshRenderer meshRender;
    HoloTable holoTable;
    DronePart displayedPart;


    private void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        originalMaterial = meshRender.material;

        holoTable = GetComponentInParent<HoloTable>();
    }

    void FixedUpdate()
    {
        if (frameToHide > 0)
            frameToHide--;
        else if (isHovered)
        {
            isHovered = false;
            meshRender.material = originalMaterial;
        }
    }

    public void OnHover(GameObject other)
    {
        if (!isPartValid)
            return;

        frameToHide = 3;
        if(!isHovered)
        {
            isHovered = true;
            meshRender.material = hoveredMaterial;
        }
    }

    public void OnInteraction(GameObject other)
    {
        if (!isPartValid)
            return;

        holoTable.AddPart(selfIndex, anchorIndex);
    }

    public void SetPartValid(bool isValid)
    {
        if (isValid == isPartValid)
            return;

        isPartValid = isValid;

        if(isValid)
        {
            meshRender.material = originalMaterial;
        }
        else
        {
            meshRender.material = invalidMaterial;
        }
    }

    public void SetDisplayedPart(DronePart part)
    {
        displayedPart = part;
    }

    public DronePart GetDisplayedPart()
    {
        return displayedPart;
    }
}
