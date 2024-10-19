using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class NarrationManager : MonoBehaviour
{
    public List<AudioClip> narrations; // Lista de clips de narración
    private int currentNarrationIndex = -1; // Índice de la narración actual
    private Queue<int> queuedNarrations = new Queue<int>();

    private AudioSource narrationAudioSource;
    public AudioSource backgroundAudioSource; // AudioSource para el audio de fondo
    
    public GameObject xrOrigin; // Referencia al XR Origin
    private ContinuousMoveProviderBase moveProvider;
    private float originalMoveSpeed;

    private bool narrationInProgress = false; // Controlar si una narración está en progreso
    private bool narrationSequencePending = false; // Controlar si hay una secuencia pendiente
    
    private string currentSceneName;

    void Start()
    {
        narrationAudioSource = GetComponent<AudioSource>();
        if (narrationAudioSource == null)
        {
            Debug.LogError("No se encontró AudioSource");
        }
        
        moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
        
        if (moveProvider != null)
        {
            originalMoveSpeed = moveProvider.moveSpeed;
        }
        
        narrationAudioSource.loop = false; 
        narrationAudioSource.playOnAwake = false; 
        
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void PlayNextNarration(bool disableMovement = false)
    {
       
        if (narrationInProgress) return;

        if (currentNarrationIndex < narrations.Count - 1)
        {
            currentNarrationIndex++;
            PlayCurrentNarration(disableMovement);
        }
        else
        {
            Debug.Log("No hay más narraciones.");
            GoToFinalScene();
        }
    }

    public void PlayFinalScene()
    {
        if (narrationInProgress)
        {
            narrationSequencePending = true; 
            return;
        }

        GoToFinalScene();
    }
    
    public void GoToFinalScene()
    {
        int finalSceneint;
        
        switch (currentSceneName)
        {
            case "AseoConfort":
                finalSceneint = 6;
                break;
            case "CaidaEntorno":
                finalSceneint = 7;
                break;
            case "Movilizacion":
                finalSceneint = 8;
                break;
            case "SaludMental":
                finalSceneint = 9;
                break;
            default:
                finalSceneint = 5;
                break;
        }
        
        SceneTransitionManager.singleton.GoToSceneAsync(finalSceneint); 
    }

    private void PlayCurrentNarration(bool disableMovement)
    {
        narrationInProgress = true;  

        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause(); 
        }

        xrOrigin.GetComponent<FootstepSound>().enabled = false;
        
        if (moveProvider != null && !disableMovement)
        {
            StartCoroutine(DisableMovementWithDelay(0.5f));
        }
        
        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();
        
        Invoke(nameof(ResumeMovementAndBackgroundAudio), narrationAudioSource.clip.length);
        Invoke(nameof(FinishNarration), narrationAudioSource.clip.length + 1f);
    }
    
    public void QueueNarration(bool disableMovement = false)
    {
        if (narrationInProgress)
        {
            if (currentNarrationIndex < narrations.Count - 1)
            {
                queuedNarrations.Enqueue(currentNarrationIndex + 1);
            }
            return;
        }

        if (currentNarrationIndex < narrations.Count - 1)
        {
            currentNarrationIndex++;
            PlayQueuedNarration(disableMovement);
        }
        else
        {
            Debug.Log("No hay más narraciones.");
            GoToFinalScene();
        }
    }

    private void PlayQueuedNarration(bool disableMovement)
    {
        narrationInProgress = true;

        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause();
        }

        xrOrigin.GetComponent<FootstepSound>().enabled = false;

        if (moveProvider != null && !disableMovement)
        {
            StartCoroutine(DisableMovementWithDelay(0.3f));
        }

        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();

        // Manejar el final de la narración
        Invoke(nameof(ResumeMovementAndBackgroundAudio), narrationAudioSource.clip.length);
        Invoke(nameof(FinishQueuedNarration), narrationAudioSource.clip.length + 1f);
    }

    private void FinishQueuedNarration()
    {
        narrationInProgress = false;

        if (queuedNarrations.Count > 0)
        {
            currentNarrationIndex = queuedNarrations.Dequeue();
            PlayQueuedNarration(false);
        }
    }


    private void FinishNarration()
    {
        narrationInProgress = false;  // Marcar el final de la narración
    }
    
    private IEnumerator DisableMovementWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveProvider.moveSpeed = 0f;
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
            moveProvider.moveSpeed = originalMoveSpeed;
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
            PlayFinalScene();  // Ejecutar la secuencia pendiente si existe
        }
    }
}
