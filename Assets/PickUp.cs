using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;
    public bool attached;

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
        transform.position = dest.position;

        transform.parent = dest;
    }

    public void AttachObject(Transform attachPoint)
    {
        destination = attachPoint;

        GetComponent<Rigidbody>().isKinematic = false;
        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
        fj.connectedBody = attachPoint.GetComponentInParent<Rigidbody>();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;

        transform.parent = attachPoint;
    }

    public void DropObject()
    {
        if (GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
        }

        destination = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;

        transform.parent = null;
    }

    public void ThrowObject(Vector3 forward)
    {
        DropObject();
        GetComponent<Rigidbody>().AddForce(forward, ForceMode.Impulse);
    }
}
