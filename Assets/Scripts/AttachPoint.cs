using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum AttachType
{
    Power = 0x1,
    Mining = 0x2,
    Fuel = 0x4,
    Ammo = 0x8
}

public class AttachPoint : MonoBehaviour, Interactable
{
    public Material pointSelectedMaterial;
    protected Material pointNormalMaterial;

    private HoloTable holoTable;

    public Vector3 rotationOffset = Vector3.zero;
    public MeshRenderer visual;

    public AttachType attachType;

    private int frameToHide = 0;
    private bool pointVisible = false;
    private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        visual.enabled = pointVisible;
        pointNormalMaterial = visual.material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frameToHide > 0)
            frameToHide--;
        else if (pointVisible)
        {
            pointVisible = false;
            if(!isSelected)
                visual.enabled = false;
        }
    }

    public void OnHover(GameObject other)
    {
        if (!enabled)
            return;

        frameToHide = 3;
        if(!pointVisible)
        {
            pointVisible = true;
            visual.enabled = true;
        }
    }

    public void OnInteraction(GameObject other)
    {
        if (holoTable == null)
            holoTable = GetComponentInParent<HoloTable>();

        holoTable.SelectAttachPoint(this);
    }

    public void SetSelected(bool newState)
    {
        if (newState == isSelected)
            return;

        if (newState)
        {
            visual.material = pointSelectedMaterial;
        }
        else
        {
            visual.material = pointNormalMaterial;
            frameToHide = 1;
        }

        isSelected = newState;
    }
}
