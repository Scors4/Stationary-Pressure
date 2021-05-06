using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadarScreen : MonoBehaviour
{
    public Transform radarPanel;
    public GameObject sensor;
    public GameObject droneBogie;
    public GameObject asteroidBogie;

    [Range(25, 2500)]
    public float radarRange = 50;

    float width;
    float height;
    List<Bogie> droneIcons;
    List<GameObject> roidIcons;

    // Start is called before the first frame update
    void Awake()
    {
        droneIcons = new List<Bogie>();
        roidIcons = new List<GameObject>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        RectTransform rect = canvas.GetComponent<RectTransform>();
        width = rect.rect.width;
        height = rect.rect.height;

        if (sensor == null)
            sensor = this.gameObject;
    }

    void FixedUpdate()
    {
        List<DroneBase> activeDrones = DroneMgr.inst.GetAllDrones();
        List<Asteroid> activeRoids = AsteroidMgr.inst.GetAsteroids();

        if (droneIcons.Count != activeDrones.Count)
        {
            while (droneIcons.Count < activeDrones.Count)
            {
                GameObject obj = Instantiate(droneBogie);
                obj.transform.SetParent(radarPanel);
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                droneIcons.Add(obj.GetComponent<Bogie>());
            }
            
            if (droneIcons.Count > activeDrones.Count)
            {
                droneIcons.RemoveRange(activeDrones.Count, (droneIcons.Count - activeDrones.Count));
            }

            for(int i = 0; i < droneIcons.Count; i++)
            {
                DroneBase drone = activeDrones[i];
                Bogie bogie = droneIcons[i];

                bogie.SetDroneOwner(drone, drone.GetOwner());
            }
        }

        if (roidIcons.Count != activeRoids.Count)
        {
            while (roidIcons.Count < activeRoids.Count)
            {
                GameObject obj = Instantiate(asteroidBogie);
                obj.transform.SetParent(radarPanel);
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one / 2.0f;

                roidIcons.Add(obj);
            }

            if (roidIcons.Count > activeRoids.Count)
            {
                roidIcons.RemoveRange(activeRoids.Count, (roidIcons.Count - activeRoids.Count));
            }
        }

        for (int i = 0; i < droneIcons.Count; i++)
        {
            DroneBase drone = activeDrones[i];
            Bogie bogie = droneIcons[i];

            float distance = Vector3.Distance(drone.transform.position, sensor.transform.position);
            distance = Mathf.Clamp((distance / radarRange), 0.0f, 1.0f);

            // -90 to 90 on the XZ axis, relative to the "reverse" direction (-Z)
            Vector3 toVector = sensor.transform.InverseTransformPoint(drone.transform.position);
            toVector.y = 0;
            float angle = Vector3.SignedAngle(-Vector3.forward, toVector, Vector3.up);
            angle = Mathf.Clamp(angle, -90, 90);

            angle *= Mathf.Deg2Rad;

            RectTransform rect = bogie.GetComponent<RectTransform>();
            float posX = Mathf.Sin(angle) * distance * (width / 2);
            float posY = Mathf.Cos(angle) * distance * height;

            rect.anchoredPosition = new Vector2(posX, posY);

            Vector3 relVector = sensor.transform.InverseTransformDirection(drone.transform.forward);
            bogie.UpdateDroneDirection(-Vector3.SignedAngle(-Vector3.forward, relVector, Vector3.up));


        }

        for(int i = 0; i < roidIcons.Count; i++)
        {
            Asteroid asteroid = activeRoids[i];
            GameObject roid = roidIcons[i];

            float distance = Vector3.Distance(asteroid.transform.position, sensor.transform.position);
            distance = Mathf.Clamp((distance / radarRange), 0.0f, 1.0f);

            Vector3 toVector = sensor.transform.InverseTransformPoint(asteroid.transform.position);
            toVector.y = 0;
            float angle = Vector3.SignedAngle(-Vector3.forward, toVector, Vector3.up);
            angle = Mathf.Clamp(angle, -90, 90);

            angle *= Mathf.Deg2Rad;

            RectTransform rect = roid.GetComponent<RectTransform>();
            float posX = Mathf.Sin(angle) * distance * (width / 2);
            float posY = Mathf.Cos(angle) * distance * height;

            rect.anchoredPosition = new Vector2(posX, posY);
        }
    }
}
