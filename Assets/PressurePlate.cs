using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public bool gravOn;
    public LayerMask lm;
    public UnityEvent function;
    private bool hasCalled;

    private void Update()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position, 1f, Vector3.up, out hit, lm))
        {
            gravOn = false;
            hasCalled = false;
        }

        if (!gravOn)
            GetComponent<Rigidbody>().AddForce(Vector3.up * 9.81f, ForceMode.Acceleration);
        else
        {
            if(!hasCalled)
            {
                function.Invoke();
                hasCalled = true;
            } 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Box"))
        {
            gravOn = true;
        }
    }

    public void CallFunction()
    {
        Debug.Log("TESTING");
    }
}
