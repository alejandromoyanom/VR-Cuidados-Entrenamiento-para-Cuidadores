using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class NarrationManager : MonoBehaviour
{
    public List<AudioClip> narrations; // Lista de clips de narración
    private int currentNarrationIndex = -1; // Índice de la narración actual

    private AudioSource narrationAudioSource;
    public AudioSource backgroundAudioSource; // AudioSource para el audio de fondo
    
    public GameObject xrOrigin; // Referencia al XR Origin
    private DynamicMoveProvider moveProvider;

    private bool narrationInProgress = false; // Controlar si una narración está en progreso
    private bool narrationSequencePending = false; // Controlar si hay una secuencia pendiente

    void Start()
    {
        narrationAudioSource = GetComponent<AudioSource>();
        if (narrationAudioSource == null)
        {
            Debug.LogError("No se encontró AudioSource");
        }
        
        if (xrOrigin != null)
        {
            moveProvider = xrOrigin.GetComponent<DynamicMoveProvider>();
            if (moveProvider == null)
            {
                Debug.LogError("No se encontró MoveProvider");
            }
        }
        
        narrationAudioSource.loop = false; // Asegurarse de que el audio no se reproduzca en bucle
        narrationAudioSource.playOnAwake = false; // Evitar que se reproduzca al iniciar
    }

    public void PlayNextNarration(bool disableMovement = false)
    {
        // No permitir que se reproduzcan nuevas narraciones si una ya está en progreso
        if (narrationInProgress) return;

        if (currentNarrationIndex < narrations.Count - 1)
        {
            currentNarrationIndex++;
            PlayCurrentNarration(disableMovement);
        }
        else
        {
            Debug.Log("No hay más narraciones.");
            SceneTransitionManager.singleton.GoToSceneAsync(5);
        }
    }

    public void PlayNarrationSequenceFromCurrent()
    {
        if (narrationInProgress)
        {
            narrationSequencePending = true; // Marcar como pendiente si ya hay una en progreso
            return;
        }

        currentNarrationIndex++;
        StartCoroutine(PlayNarrationSequenceCoroutine());
    }
    
    private IEnumerator PlayNarrationSequenceCoroutine()
    {
        narrationInProgress = true;  // Marcar la narración como en progreso
        narrationSequencePending = false; // Reiniciar el estado de la secuencia pendiente

        while (currentNarrationIndex < narrations.Count)
        {
            // Reproducir la narración actual
            PlayCurrentNarration(false); // Desactiva el movimiento mientras se reproduce

            // Esperar hasta que termine la narración actual
            yield return new WaitForSeconds(narrationAudioSource.clip.length + 1f);

            // Avanzar al siguiente índice
            currentNarrationIndex++;
        }

        // Al terminar la secuencia, reactivar el movimiento
        ResumeMovementAndBackgroundAudio();

        narrationInProgress = false;  // Marcar que la narración ha terminado

        // Verifica que el SceneTransitionManager exista antes de usarlo
        if (SceneTransitionManager.singleton != null)
        {
            SceneTransitionManager.singleton.GoToSceneAsync(5); // Cambiar de escena al terminar
        }
        else
        {
            Debug.LogError("SceneTransitionManager no se encuentra o es nulo.");
        }

        
    }

    private void PlayCurrentNarration(bool disableMovement)
    {
        narrationInProgress = true;  // Marcar el inicio de la narración

        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause(); // Pausar el audio de fondo
        }

        xrOrigin.GetComponent<FootstepSound>().enabled = false;
        
        if (moveProvider != null && !disableMovement)
        {
            StartCoroutine(DisableMovementWithDelay(0.5f));
        }

        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();

        // Manejar el final de la narración
        Invoke(nameof(ResumeMovementAndBackgroundAudio), narrationAudioSource.clip.length);
        Invoke(nameof(FinishNarration), narrationAudioSource.clip.length + 1f);
    }

    private void FinishNarration()
    {
        narrationInProgress = false;  // Marcar el final de la narración
    }
    
    private IEnumerator DisableMovementWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveProvider.enabled = false; // Desactivar el movimiento
    }

    private void ResumeMovementAndBackgroundAudio()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.UnPause(); // Reanudar el audio de fondo
        }

        xrOrigin.GetComponent<FootstepSound>().enabled = true;
        
        if (moveProvider != null)
        {
            moveProvider.enabled = true; // Reactivar el movimiento
        }
    }

    public float GetCurrentNarrationDuration()
    {
        if (narrationAudioSource.clip != null)
        {
            return narrationAudioSource.clip.length;
        }
        else
        {
            Debug.LogWarning("No hay narración actual o el clip es nulo.");
            return 0f;
        }
    }

    public void PlayThreeNarrations()
    {
        if (!narrationInProgress)  // Solo permitir iniciar si no hay narraciones en progreso
        {
            StartCoroutine(PlayThreeNarrationsCoroutine());
        }
    }

    private IEnumerator PlayThreeNarrationsCoroutine()
    {
        // Reproducir la primera narración
        PlayNextNarration();
        yield return new WaitForSeconds(narrationAudioSource.clip.length + 1f);

        // Reproducir la segunda narración
        PlayNextNarration(true); 
        yield return new WaitForSeconds(narrationAudioSource.clip.length + 1f);

        // Reproducir la tercera narración
        PlayNextNarration(true);
        yield return new WaitForSeconds(narrationAudioSource.clip.length + 1f);

        narrationInProgress = false;  // Marcar que se terminó la narración en progreso

        // Verificar si hay una secuencia pendiente
        if (narrationSequencePending)
        {
            PlayNarrationSequenceFromCurrent();  // Ejecutar la secuencia pendiente si existe
        }
    }
}
