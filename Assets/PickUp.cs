using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;
    public bool attached;
    public Material rootObjectColour;
    public Material cMaterial;

    private void Start()
    {
        cMaterial = GetComponent<Renderer>().material;
    }

    public void PickUpObject(Transform dest)
    {
        destination = dest;

        if(GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
        }

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;

        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if(rb.CompareTag("Box"))
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.GetComponent<Collider>().enabled = false;
            }
        }

        transform.position = dest.position;

        if(transform.parent != null && transform.parent.parent.CompareTag("Box"))
        {
            transform.parent.parent.GetComponent<MeshRenderer>().material = transform.parent.parent.GetComponent<PickUp>().cMaterial;
            GetComponent<MeshRenderer>().material = rootObjectColour;
        }

        transform.parent = dest;
    }

    public void AttachObject(Transform attachPoint)
    {
        destination = attachPoint;

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        transform.parent = attachPoint;
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;

        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
        fj.connectedBody = attachPoint.GetComponentInParent<Rigidbody>();
        fj.enableCollision = true;

        bool setRoot = false;
        foreach(Transform t in transform.root.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Box"))
            {
                if(!setRoot)
                {
                    t.GetComponent<MeshRenderer>().material = rootObjectColour;
                    setRoot = true;
                }
                else
                {
                    t.GetComponent<MeshRenderer>().material = cMaterial;
                }
            }
        }
    }

    public void DropObject()
    {
        if (GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
        }

        destination = null;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.CompareTag("Box"))
            {
                rb.isKinematic = false;
                rb.GetComponent<Collider>().enabled = true;
            }
        }

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;

        transform.parent = null;
    }

    public void ThrowObject(Vector3 forward)
    {
        DropObject();
        GetComponent<Rigidbody>().AddForce(forward, ForceMode.VelocityChange);
    }
}
