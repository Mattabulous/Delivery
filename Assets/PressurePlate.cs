using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Material pressed;
    private Material material;
    public bool gravOn;
    public LayerMask lm;
    public UnityEvent enterFunction;
    public UnityEvent exitFunction;
    private bool hasCalledEnter;
    private bool hasCalledExit = true;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position, 1f, new Vector3(0, 0.1f, 0), out hit, lm))
        {
            gravOn = false;
            hasCalledEnter = false;

            if (!hasCalledExit)
            {
                exitFunction.Invoke();
                GetComponent<MeshRenderer>().material = material;
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
            GetComponent<MeshRenderer>().material = pressed;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
