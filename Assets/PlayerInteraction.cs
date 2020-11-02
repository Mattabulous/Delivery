using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform pickUpPoint;
    private bool objectGrabbed;
    public Transform snapped;
    public PickUp objectPickUp;
    [SerializeField] LayerMask box;
    [SerializeField] LayerMask boxSnap;

    float rotSpeed = 60;

    bool canE;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(objectPickUp != null)
        {
            if(Input.GetKey(KeyCode.R))
            {
                MouseLook.canMove = false;

                Cursor.lockState = CursorLockMode.Confined;

                float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

                objectPickUp.transform.Rotate(Vector3.up, -rotX);
                objectPickUp.transform.Rotate(Vector3.right, rotY);
            }
            else
            {
                MouseLook.canMove = true;

                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        RaycastHit hit;
        RaycastHit hit2;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, box))
        {
            if(hit.collider.GetComponent<PickUp>())
            {
                if(Input.GetKeyDown(KeyCode.E) && !objectGrabbed)
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

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2, 5, boxSnap))
        {
            bool parentChecker;
            try
            {
                parentChecker = hit2.collider.transform.root != objectPickUp.transform.root;
            }
            catch (NullReferenceException e)
            {
                parentChecker = true;
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
            if (objectPickUp != null)
            {
                objectPickUp.transform.position = pickUpPoint.position;

                snapped = null;
            }
        }

        if (objectGrabbed)
        {
            if(Input.GetKeyDown(KeyCode.E) && canE)
            {
                objectPickUp.DropObject();
                objectGrabbed = false;

                if(snapped != null)
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
