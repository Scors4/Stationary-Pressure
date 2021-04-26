using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DroneCommand : MonoBehaviour
{
    public RectTransform ScrollPanel;

    Canvas screenCanvas;
    GraphicRaycaster raycaster;

    Scrollbar scrollbar;

    EventSystem es;

    // Start is called before the first frame update
    void Awake()
    {
        screenCanvas = GetComponentInChildren<Canvas>();
        raycaster = GetComponentInChildren<GraphicRaycaster>();

        scrollbar = GetComponentInChildren<Scrollbar>();
    }

    private void Start()
    {
        es = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        PointerEventData ped = new PointerEventData(es);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        es.RaycastAll(ped, results);

        foreach(RaycastResult result in results)
        {
            IPointerClickHandler pch = result.gameObject.GetComponent<IPointerClickHandler>();

            if(pch != null)
                pch.OnPointerClick(ped);
        }
    }

    public void OnScrollDown()
    {
        float step = 1.0f / (ScrollPanel.childCount);
        scrollbar.value -= step * 2;

        if (scrollbar.value < 0)
            scrollbar.value = 0;
    }

    public void OnScrollUp()
    {
        float step = 1.0f / (ScrollPanel.childCount);
        scrollbar.value += step * 2;

        if (scrollbar.value > 1.0f)
            scrollbar.value = 1.0f;
    }
}
