using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDisplay : MonoBehaviour, Interactable
{
    public int selfIndex = 0;
    private int anchorIndex = 0;

    int frameToHide = 0;
    bool isHovered = false;

    public Material hoveredMaterial;
    protected Material originalMaterial;

    MeshRenderer meshRender;
    HoloTable holoTable;


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
        frameToHide = 3;
        if(!isHovered)
        {
            isHovered = true;
            meshRender.material = hoveredMaterial;
        }
    }

    public void OnInteraction(GameObject other)
    {
        holoTable.AddPart(selfIndex, anchorIndex);
    }
}
