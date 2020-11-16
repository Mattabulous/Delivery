using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxChecker : MonoBehaviour
{
    private int cBoxes;
    [SerializeField] int boxCount;
    public UnityEvent finishedFunction;

    public void IncreaseBox()
    {
        cBoxes++;
        CheckBox();
    }

    public void DecreaseBox()
    {
        cBoxes--;
        CheckBox();
    }

    public void CheckBox()
    {
        if(cBoxes >= boxCount)
        {
            finishedFunction.Invoke();
        }
    }
}
