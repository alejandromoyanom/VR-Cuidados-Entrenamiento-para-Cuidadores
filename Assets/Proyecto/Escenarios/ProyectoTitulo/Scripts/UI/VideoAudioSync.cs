using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections;

public class VideoAudioSync : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public AudioSource audioSource;  
    public DynamicMoveProvider moveProvider; 
    public Canvas videoCanvas;
    public NarrationManager narrationManager;

    private const string TutorialCompletedKey = "TutorialCompleted";

    void Start()
    {
        // Verificar si el tutorial ya se ha completado
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
        {
            // Si el tutorial ya fue completado, no reproducir el video ni el audio
            moveProvider.enabled = true;

            // Reproducir la siguiente narración al iniciar la escena
            narrationManager.PlayNextNarration();
            return;
        }

        // Desactivar el movimiento al inicio
        moveProvider.enabled = false;

        // Asegúrate de que el video no comience automáticamente
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        // Sincronizar la reproducción
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += SyncAndPlay;

        // Subscribir al evento cuando el video termina
        videoPlayer.loopPointReached += OnVideoEnd;
    }


    void SyncAndPlay(VideoPlayer vp)
    {
        // Reproducir el audio y el video al mismo tiempo
        videoCanvas.gameObject.SetActive(true);
        audioSource.Play();
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Detener el video y el audio
        videoPlayer.Stop();
        audioSource.Stop();
        videoCanvas.gameObject.SetActive(false);
        
        // Marcar el tutorial como completado
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();

        // Reactivar el movimiento cuando el video termine
        moveProvider.enabled = true;

        // Iniciar la narración con un retraso
        StartCoroutine(PlayNarrationWithDelay());
    }
    
    IEnumerator PlayNarrationWithDelay()
    {
        // Esperar 1 segundo
        yield return new WaitForSeconds(2f);

        // Iniciar la primera narración
        narrationManager.PlayNextNarration();
    }
}
