using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player player;

    public bool isInBuild {get; private set;}
    private Camera playerCamera;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = this;
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        isInBuild = false;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0))
            FireInteraction();
    }

    void FixedUpdate()
    {
        if (isInBuild)
            UpdatePossibleInteraction();
    }

    public void ToggleBuildMode(bool newState)
    {
        isInBuild = newState;
    }

    void UpdatePossibleInteraction()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 5, Color.white, 1.0f);
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 5, 1 << 3))
        {
            Interactable ap = hit.collider.gameObject.GetComponent<Interactable>();
            if(ap != null)
            {
                ap.OnHover(this.gameObject);
            }
            else
            {
                Interactable pap = hit.collider.GetComponentInParent<Interactable>();
                if(pap != null)
                {
                    pap.OnHover(this.gameObject);
                }
            }
        }
    }

    void FireInteraction()
    {
        if (!isInBuild)
            return;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 5, Color.white, 1.0f);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 5, 1 << 3))
        {
            Interactable ap = hit.collider.gameObject.GetComponent<Interactable>();
            if (ap != null)
            {
                ap.OnInteraction(this.gameObject);
            }
            else
            {
                Interactable pap = hit.collider.GetComponentInParent<Interactable>();
                if (pap != null)
                {
                    pap.OnInteraction(this.gameObject);
                }
            }
        }
    }
}
