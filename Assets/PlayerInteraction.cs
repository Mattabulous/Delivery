using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform pickUpPoint;
    private bool objectGrabbed;
    private PickUp objectPickUp;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5))
        {
            if(hit.collider.GetComponent<PickUp>())
            {
                if(Input.GetKeyDown(KeyCode.E) && !objectGrabbed)
                {
                    objectPickUp = hit.collider.GetComponent<PickUp>();
                    objectPickUp.PickUpObject(pickUpPoint);
                    objectGrabbed = true;
                }
            }
        }

        if(objectGrabbed)
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                objectPickUp.DropObject();
                objectGrabbed = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 5, Color.red);
    }
}
