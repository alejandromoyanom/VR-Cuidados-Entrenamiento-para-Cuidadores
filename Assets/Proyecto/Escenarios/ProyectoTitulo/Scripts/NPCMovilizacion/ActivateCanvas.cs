using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCanvas : MonoBehaviour
{
    public GameObject canvasGrab;
    public GameObject canvasGrab2;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasGrab.SetActive(true);
            canvasGrab2.SetActive(true);
        }
    }
}
