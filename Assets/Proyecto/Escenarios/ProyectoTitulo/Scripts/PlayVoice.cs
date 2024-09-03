using System.Collections;
using System;
using UnityEngine;

public class PlayVoice : MonoBehaviour
{
    public NarrationManager narrationManager;
    public GameObject puntoNarracion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el XR Origin tenga la etiqueta "Player"
        {
            narrationManager.PlayNextNarration(); // Reproduce la siguiente narración
            puntoNarracion.SetActive(false);
        }
    }
}