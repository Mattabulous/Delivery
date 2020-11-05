using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public bool gravOn;
    public LayerMask lm;
    public UnityEvent enterFunction;
    public UnityEvent exitFunction;
    private bool hasCalledEnter;
    private bool hasCalledExit = true;

    private void Update()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position, 1f, Vector3.up, out hit, lm))
        {
            gravOn = false;
            hasCalledEnter = false;

            if(!hasCalledExit)
            {
                exitFunction.Invoke();
                hasCalledExit = true;
            }
        }

        if (!gravOn)
            GetComponent<Rigidbody>().AddForce(Vector3.up * 9.81f, ForceMode.Acceleration);
        else
        {
            if(!hasCalledEnter)
            {
                enterFunction.Invoke();
                hasCalledEnter = true;
            } 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Box"))
        {
            gravOn = true;
            hasCalledExit = false;
        }
    }

    public void CallFunction()
    {
        Debug.Log("TESTING");
    }

    public void ToggleActive(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }
}
