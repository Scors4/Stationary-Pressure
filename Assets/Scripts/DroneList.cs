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
        foreach(UserDrone drone in droneMgr.GetUserDrones())
        {
            BuildNewCard(drone);
        }
    }

    public void FixedUpdate()
    {
        if(droneMgr.GetUserDrones().Count > cards.Count)
        {
            foreach(UserDrone drone in droneMgr.GetUserDrones())
            {
                if (!drone.hasCard)
                    BuildNewCard(drone);
            }
        }
    }

    private void BuildNewCard(UserDrone drone)
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
