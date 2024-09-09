using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class PlayIntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource; 
    public Canvas videoCanvas;
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;
    
    private const string TutorialCompletedKey = "TutorialCompleted";

    void Start()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 0);
        PlayerPrefs.Save();
        
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        // Sincronizar la reproducción
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += SyncAndPlay;

        // Detectar cuando el video ha terminado
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void SyncAndPlay(VideoPlayer vp)
    {
        // Desactivar el movimiento y los rayos al iniciar el video
        leftRayInteractor.enabled = false;
        rightRayInteractor.enabled = false;
        
        // Reproducir el audio y el video al mismo tiempo
        videoCanvas.gameObject.SetActive(true);
        audioSource.Play();
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Esperar hasta que termine el audio antes de desactivar el canvas
        StartCoroutine(WaitForAudioToFinish());
    }

    IEnumerator WaitForAudioToFinish()
    {
        // Esperar a que el audio termine
        while (audioSource.isPlaying)
        {
            yield return null; // Esperar un frame
        }

        // Reactivar rayos de interacción y desactivar el Canvas cuando el audio termine
        leftRayInteractor.enabled = true;
        rightRayInteractor.enabled = true;
        videoCanvas.gameObject.SetActive(false);
    }
}