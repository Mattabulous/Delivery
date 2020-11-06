using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerInput pi;

    [SerializeField] Camera cam;
    [SerializeField] Transform pickUpPoint;
    private bool objectGrabbed;
    public Transform snapped;
    public PickUp objectPickUp;
    [SerializeField] LayerMask box;
    [SerializeField] LayerMask boxSnap;
    [SerializeField] Transform trailStart;

    [SerializeField] float maxZoom;
    public float cZoom;

    float rotSpeed = 60;

    bool canE;

    private float force = 4;

    private bool toggledSnap;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        pi.actions.FindAction("Throw").started += delegate { OnThrowStarted(); };
        pi.actions.FindAction("Throw").canceled += delegate { OnThrowRelease(); };
        pi.actions.FindAction("SnapToggle").performed += delegate { OnSnap(); };
    }

    void OnThrowStarted()
    {
        trailStart.gameObject.SetActive(true);

        force += 3 * Time.deltaTime;

        objectPickUp.DrawTrajectory(pickUpPoint.transform.position, cam.transform.forward * force);
    }

    void OnThrowRelease()
    {
        trailStart.gameObject.SetActive(false);

        objectPickUp.ThrowObject(cam.transform.forward * force);
        objectGrabbed = false;

        force = 4;

        if (objectPickUp.GetComponent<Box>())
        {
            foreach (Transform child in objectPickUp.transform)
            {
                if (!child.GetComponentInChildren<PickUp>())
                    child.gameObject.SetActive(true);
            }
        }

        objectPickUp = null;
    }

    void OnSnap()
    {
        toggledSnap = !toggledSnap;
    }

    // Update is called once per frame
    void Update()
    {
        if(objectPickUp != null)
        {
            if(pi.actions.FindAction("Rotate").ReadValue<bool>())
            {
                MouseLook.canMove = false;

                Cursor.lockState = CursorLockMode.Confined;

                float rotX = pi.actions.FindAction("RotX").ReadValue<float>() * rotSpeed * Mathf.Deg2Rad;
                float rotY = pi.actions.FindAction("RotY").ReadValue<float>() * rotSpeed * Mathf.Deg2Rad;

                objectPickUp.transform.Rotate(Vector3.up, -rotX);
                objectPickUp.transform.Rotate(Vector3.right, rotY);
            }
            else
            {
                MouseLook.canMove = true;

                Cursor.lockState = CursorLockMode.Locked;
            }

            cZoom = Mathf.Clamp(cZoom, 2.5f, maxZoom);

            cZoom += (pi.actions.FindAction("Scroll").ReadValue<float>() * 0.1f);

            pickUpPoint.localPosition = new Vector3(0, pickUpPoint.localPosition.y, cZoom);
        }

        RaycastHit hit;
        RaycastHit hit2;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, box))
        {
            if(hit.collider.GetComponent<PickUp>())
            {
                if(pi.actions.FindAction("PickUp").triggered && !objectGrabbed)
                {
                    canE = false;
                    StartCoroutine(ECooldown());

                    objectPickUp = hit.collider.GetComponent<PickUp>();

                    if (objectPickUp.attached)
                    {
                        objectPickUp.destination.GetComponent<Collider>().enabled = true;
                        objectPickUp.attached = false;
                    }

                    objectPickUp.PickUpObject(pickUpPoint);
                    objectGrabbed = true;

                    if(hit.collider.GetComponent<Box>())
                    {
                        foreach (Transform child in hit.transform)
                        {
                            if (!child.GetComponentInChildren<PickUp>())
                                child.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2, 5, boxSnap) && toggledSnap && objectPickUp != null)
        {
            if (hit2.collider.GetComponentInParent<Box>().boxName == objectPickUp.GetComponent<Box>().boxName)
            {
                bool parentChecker;
                try
                {
                    parentChecker = hit2.collider.transform.root != objectPickUp.transform.root;
                }
                catch (NullReferenceException e)
                {
                    parentChecker = false;
                }

                if (objectGrabbed && parentChecker)
                {
                    objectPickUp.transform.position = hit2.collider.transform.position;
                    objectPickUp.transform.rotation = hit2.collider.transform.rotation;

                    snapped = hit2.collider.transform;
                }
            }
            else
            {
                snapped = null;
            }
        }
        else
        {
            snapped = null;

            if (objectPickUp != null)
            {
                objectPickUp.transform.position = pickUpPoint.position;
            }
        }

        if (objectGrabbed)
        {
            if(pi.actions.FindAction("Place").triggered && canE)
            {
                objectPickUp.DropObject();
                objectGrabbed = false;

                if (snapped != null)
                {
                    snapped.GetComponent<Collider>().enabled = false;

                    objectPickUp.AttachObject(snapped);
                    objectPickUp.attached = true;

                    snapped = null;
                }

                if (objectPickUp.GetComponent<Box>())
                {
                    foreach (Transform child in objectPickUp.transform)
                    {
                        if (!child.GetComponentInChildren<PickUp>())
                            child.gameObject.SetActive(true);
                    }
                }

                objectPickUp = null;
            }
        }
    }

    IEnumerator ECooldown()
    {
        yield return new WaitForSeconds(0.1f);
        canE = true;
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 5, Color.red);
    }
}
