using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneList : MonoBehaviour
{
    public GameObject DroneCardPrefab;
    public RectTransform panelParent;

    List<DroneCard> cards = new List<DroneCard>();

    public void Start() {}

    private void BuildNewCard(UserDrone drone)
    {
        DroneCard droneCard = Instantiate(DroneCardPrefab).GetComponent<DroneCard>();
        droneCard.transform.SetParent(panelParent);
        droneCard.transform.localPosition = Vector3.zero;
        droneCard.transform.localScale = Vector3.one;
        droneCard.transform.localEulerAngles = Vector3.zero;

        droneCard.SetDrone(drone); 
        drone.droneCard = droneCard; 
        
        cards.Add(droneCard);
    }
    
    public void DroneSpawned(UserDrone drone) {
        BuildNewCard(drone);
    }
    
    public void DroneDestroyed(UserDrone drone) {
        cards.Remove(drone.droneCard);
    }
}
