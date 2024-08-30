using System.Collections;
using System;
using UnityEngine;

public class PlayVoice : MonoBehaviour
{
    public NarrationManager narrationManager; // Asignar el NarrationManager en el Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el XR Origin tenga la etiqueta "Player"
        {
            narrationManager.PlayNextNarration(); // Reproduce la siguiente narración
        }
    }
}