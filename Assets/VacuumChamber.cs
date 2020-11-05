using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VacuumChamber : MonoBehaviour
{
    public UnityEvent enterFunction;
    public string boxTarget;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Box") && other.GetComponent<Box>().boxName == boxTarget)
        {
            other.gameObject.layer = 13;
            StartCoroutine(TriggerItem());
        }
    }

    IEnumerator TriggerItem()
    {
        yield return new WaitForSeconds(1.5f);
        enterFunction.Invoke();
    }

    public void DebugLogThis()
    {
        Debug.Log("SENT!");
    }
}
