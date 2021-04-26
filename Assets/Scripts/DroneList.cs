using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneList : MonoBehaviour
{
    public GameObject DroneCardPrefab;
    public RectTransform panelParent;

    DroneMgr droneMgr;
    List<GameObject> cards;

    public void Awake()
    {
        cards = new List<GameObject>();
    }

    public void Start()
    {
        droneMgr = DroneMgr.inst;
        foreach(DroneBase drone in droneMgr.GetDrones())
        {
            BuildNewCard(drone);
        }
    }

    public void FixedUpdate()
    {
        if(droneMgr.GetDrones().Count > cards.Count)
        {
            foreach(DroneBase drone in droneMgr.GetDrones())
            {
                if (!drone.hasCard)
                    BuildNewCard(drone);
            }
        }
    }

    private void BuildNewCard(DroneBase drone)
    {
        GameObject card = Instantiate(DroneCardPrefab);
        card.transform.SetParent(panelParent);
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = Vector3.one;
        card.transform.localEulerAngles = Vector3.zero;

        DroneCard droneCard = card.GetComponent<DroneCard>();
        if (droneCard != null)
            droneCard.SetDrone(drone);

        cards.Add(card);
    }
}
