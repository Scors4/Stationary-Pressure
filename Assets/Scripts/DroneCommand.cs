using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DroneCommand : MonoBehaviour
{
    Canvas screenCanvas;
    GraphicRaycaster raycaster;

    // Start is called before the first frame update
    void Awake()
    {
        screenCanvas = GetComponentInChildren<Canvas>();
        raycaster = GetComponentInChildren<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        Debug.Log("Mouse Down.");
    }

    public void OnMouseDrag()
    {
        
    }

    public void OnMouseEnter()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnMouseExit()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
