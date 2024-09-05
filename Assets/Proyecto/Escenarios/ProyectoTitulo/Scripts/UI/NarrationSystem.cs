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

    public void PlayNarration(int index, bool disableMovement = false)
    {
        // Verificar si el índice es válido y actualizar el índice actual
        if (index >= 0 && index < narrations.Count)
        {
            currentNarrationIndex = index;
            PlayCurrentNarration(disableMovement);
        }
        else
        {
            Debug.LogWarning("Índice fuera de rango");
        }
    }

    public void PlayNextNarration(bool disableMovement = false)
    {
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
        if (currentNarrationIndex < narrations.Count)
        {
            currentNarrationIndex++;
            StartCoroutine(PlayNarrationSequenceCoroutine());
        }
        else
        {
            Debug.LogWarning("No hay más narraciones para continuar.");
        }
    }
    
    private IEnumerator PlayNarrationSequenceCoroutine()
    {
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
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause(); // Pausar el audio de fondo
        }

        xrOrigin.GetComponent<FootstepSound>().enabled = false;
        
        if (moveProvider != null && !disableMovement)
        {
            StartCoroutine(DisableMovementWithDelay(2f));
        }

        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();

        Invoke("ResumeMovementAndBackgroundAudio", narrationAudioSource.clip.length);
    }
    
    private IEnumerator DisableMovementWithDelay(float delay)
    {
        // Esperar el tiempo especificado (2 segundos en este caso)
        yield return new WaitForSeconds(delay);

        // Desactivar el movimiento
        moveProvider.enabled = false;
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
        StartCoroutine(PlayThreeNarrationsCoroutine());
    }

    private IEnumerator PlayThreeNarrationsCoroutine()
    {
        // Reproducir la primera narración
        PlayNextNarration(true);

        // Esperar hasta que la primera narración termine
        yield return new WaitForSeconds(GetCurrentNarrationDuration());

        // Reproducir la segunda narración
        PlayNextNarration(true);

        // Esperar hasta que la segunda narración termine
        yield return new WaitForSeconds(GetCurrentNarrationDuration());

        // Reproducir la tercera narración
        PlayNextNarration(true);

        // Esperar hasta que la tercera narración termine (opcional si necesitas hacer algo después)
        yield return new WaitForSeconds(GetCurrentNarrationDuration());
    }

}

