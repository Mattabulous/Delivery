﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Wormhole : MonoBehaviour
{
    public Transform previousTeleported;
    [SerializeField] Transform destination;
    [SerializeField] bool stopVelocity;
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if((other.CompareTag("Player") || other.CompareTag("Box")) && previousTeleported == null)
        {
            if(!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }

            if(stopVelocity)
            {
                if(other.GetComponent<Rigidbody>())
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            if (destination.GetComponent<Wormhole>())
            {
                destination.GetComponent<Wormhole>().previousTeleported = other.transform;
            }

            if (other.CompareTag("Player"))
            {
                other.GetComponent<FirstPersonController>().GetComponent<CharacterController>().Move(destination.position - other.transform.position);
            }
            else
            {
                other.transform.position = destination.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(previousTeleported != null && other.transform == previousTeleported)
        {
            previousTeleported = null;
        }
    }

    public void SetWormholeScene(string scene)
    {
        sceneToLoad = scene;
    }
}
