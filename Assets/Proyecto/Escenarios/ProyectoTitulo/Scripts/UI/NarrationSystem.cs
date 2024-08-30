using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;


public class NarrationManager : MonoBehaviour
{
    public List<AudioClip> narrations; // Lista de clips de narración
    private int currentNarrationIndex = -1; // Índice de la narración actual

    private AudioSource narrationAudioSource;
    public AudioSource backgroundAudioSource; // AudioSource para el audio de fondo
    private bool playAllInSequence = false; // Controlar si se deben reproducir todas las narraciones
    
    public GameObject xrOrigin; // Referencia al XR Origin
    private DynamicMoveProvider moveProvider;

    void Start()
    {
        narrationAudioSource = GetComponent<AudioSource>();
        if (narrationAudioSource == null)
        {
            Debug.LogError("No se encontro AudioSource");
        }
        
        if (xrOrigin != null)
        {
            moveProvider = xrOrigin.GetComponent<DynamicMoveProvider>();
            if (moveProvider == null)
            {
                Debug.LogError("No se encontro MoveProvider");
            }
        }
        
        narrationAudioSource.loop = false; // Asegurarse de que el audio no se reproduzca en bucle
        narrationAudioSource.playOnAwake = false; // Evitar que se reproduzca al iniciar
    }

    public void PlayNarration(int index, bool playAll = false)
    {
        playAllInSequence = playAll; // Configura si se deben reproducir todas las narraciones en secuencia

        // Verificar si el índice es válido y actualizar el índice actual
        if (index >= 0 && index < narrations.Count)
        {
            currentNarrationIndex = index;
            PlayCurrentNarration();
        }
        else
        {
            Debug.LogWarning("Indice fuera de rango");
        }
    }

    public void PlayNextNarration()
    {
        if (currentNarrationIndex < narrations.Count - 1)
        {
            currentNarrationIndex++;
            PlayCurrentNarration();
        }
        else
        {
            Debug.Log("No hay mas narraciones");
        }
    }

    public void ContinueWithCurrentNarration(bool playAll = false)
    {
        playAllInSequence = playAll; // Configura si se deben reproducir todas las narraciones en secuencia
        PlayNextNarration();
    }

    private void PlayCurrentNarration()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause(); // Pausar el audio de fondo
        }
        
        if (moveProvider != null)
        {
            moveProvider.enabled = false; // Desactivar el movimiento
        }

        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();

        // Invocar la siguiente narración solo si playAllInSequence es verdadero
        if (playAllInSequence)
        {
            Invoke("PlayNextNarration", narrationAudioSource.clip.length);
        }
        else
        {
            Invoke("ResumeMovementAndBackgroundAudio", narrationAudioSource.clip.length);
        }
    }

    private void ResumeMovementAndBackgroundAudio()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.UnPause(); // Reanudar el audio de fondo
        }
        
        if (moveProvider != null)
        {
            moveProvider.enabled = true; // Reactivar el movimiento
        }
    }
}

