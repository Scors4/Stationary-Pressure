using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isInBuild {get; private set;}
    public float mouseSensitivity = 125;
    public float moveSpeed = 15;
    private Camera playerCamera;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        isInBuild = true;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraMovement();
        UpdatePlayerMovement();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0))
            FireInteraction();
    }

    void FixedUpdate()
    {
        if (isInBuild)
            UpdatePossibleInteraction();
    }

    void UpdateCameraMovement()
    {
        playerCamera.transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity);
    }

    void UpdatePlayerMovement()
    {
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            transform.Translate((Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime) + (Input.GetAxis("Horizontal") * Vector3.right * moveSpeed * Time.deltaTime));
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
