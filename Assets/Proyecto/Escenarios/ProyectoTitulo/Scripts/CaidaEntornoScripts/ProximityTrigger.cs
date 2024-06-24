using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    public WaterCleaning waterCleaningScript;
    private void Start()
    {
        if (waterCleaningScript != null)
        {
            waterCleaningScript.SetWaterCollider(GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CleaningTool")) // Asegúrate de que el paño tenga esta tag
        {
            waterCleaningScript.canvas.SetActive(true);
            waterCleaningScript.StartCleaning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CleaningTool"))
        {
            waterCleaningScript.canvas.SetActive(false);
            waterCleaningScript.StopCleaning();
        }
    }
}
