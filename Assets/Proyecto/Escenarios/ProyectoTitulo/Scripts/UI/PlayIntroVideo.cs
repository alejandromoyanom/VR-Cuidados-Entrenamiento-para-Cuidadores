using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

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
        leftRayInteractor.enabled = true;
        rightRayInteractor.enabled = true;

        // Desactivar el Canvas para ocultar el video
        videoCanvas.gameObject.SetActive(false);
    }
}