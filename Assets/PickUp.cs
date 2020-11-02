using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;

    public void PickUpObject(Transform dest)
    {
        destination = dest;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        transform.position = dest.position;
        transform.rotation = dest.rotation;

        transform.parent = dest;
    }

    public void DropObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;

        transform.parent = null;
    }
}
