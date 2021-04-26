using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenInteractor : MonoBehaviour
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

        if (screenCanvas == null || raycaster == null)
        {
            enabled = false;
            return;
        }

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

            if (pch != null)
                pch.OnPointerClick(ped);
        }
    }

    public void OnScrollDown()
    {
        if (ScrollPanel.childCount == 0)
            return;

        float step = 1.0f / (ScrollPanel.childCount);
        scrollbar.value -= step * 2;

        if (scrollbar.value < 0)
            scrollbar.value = 0;
    }

    public void OnScrollUp()
    {
        if (ScrollPanel.childCount == 0)
            return;

        float step = 1.0f / (ScrollPanel.childCount);
        scrollbar.value += step * 2;

        if (scrollbar.value > 1.0f)
            scrollbar.value = 1.0f;
    }
}
