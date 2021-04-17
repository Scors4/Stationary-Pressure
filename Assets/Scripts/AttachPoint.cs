using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour, Interactable
{
    public Vector3 rotationOffset = Vector3.zero;
    public MeshRenderer visual;

    private int frameToHide = 0;
    private bool pointVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        visual.enabled = pointVisible;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frameToHide > 0)
            frameToHide--;
        else if (pointVisible)
        {
            pointVisible = false;
            visual.enabled = false;
        }
    }

    public void OnHover(GameObject other)
    {
        frameToHide = 3;
        if(!pointVisible)
        {
            pointVisible = true;
            visual.enabled = true;
        }
    }

    public void OnInteraction(GameObject other)
    {

    }
}
