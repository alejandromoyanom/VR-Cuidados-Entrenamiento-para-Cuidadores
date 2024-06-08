using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningTrigger : MonoBehaviour
{
    public WaterCleaning waterCleaningScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterCleaningScript.StartCleaning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterCleaningScript.StopCleaning();
        }
    }
}
