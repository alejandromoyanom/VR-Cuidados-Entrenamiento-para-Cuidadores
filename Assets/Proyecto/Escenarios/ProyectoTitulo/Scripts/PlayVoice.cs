using System.Collections;
using System;
using UnityEngine;

public class PlayVoice : MonoBehaviour
{
    public NarrationManager narrationManager;
    public GameObject puntoNarracion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            narrationManager.PlayNextNarration(); 
            puntoNarracion.SetActive(false);
        }
    }
}