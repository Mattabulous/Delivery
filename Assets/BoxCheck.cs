using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCheck : MonoBehaviour
{
    [SerializeField] BoxChecker bc;

    public void BoxEnter(Transform boxTransform)
    {
        bc.IncreaseBox();
        GetComponent<MeshRenderer>().enabled = false;
        boxTransform.GetComponent<Rigidbody>().isKinematic = true;
        boxTransform.GetComponent<PickUp>().enabled = false;
    }
}
