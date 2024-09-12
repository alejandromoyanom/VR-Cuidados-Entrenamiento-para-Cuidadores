using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections;
using UnityEngine.SceneManagement;

public class VideoAudioSync : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public AudioSource audioSource;  
    public Canvas videoCanvas;
    public NarrationManager narrationManager;
    public GameObject faderScreen;

    private const string TutorialCompletedKey = "TutorialCompleted";
    private GameObject canvasObjeto;

    void Start()
    {
        // Verificar si el tutorial ya se ha completado
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
        {
            faderScreen.SetActive(true);

            // Reproducir la siguiente narración al iniciar la escena
            narrationManager.PlayNextNarration();

            // Verificar si estamos en la escena "CaidaEntorno"
            if (SceneManager.GetActiveScene().name == "CaidaEntorno")
            {
                StartCoroutine(PlayAdditionalNarrationForCaidaEntorno());
            }
            return;
        }
        

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

        // Iniciar la narración con un retraso
        StartCoroutine(PlayNarrationWithDelay());
    }
    
    IEnumerator PlayNarrationWithDelay()
    {
        // Esperar 1 segundo
        yield return new WaitForSeconds(1f);

        // Iniciar la primera narración
        narrationManager.PlayNextNarration();

        // Verificar si estamos en la escena "CaidaEntorno"
        if (SceneManager.GetActiveScene().name == "CaidaEntorno")
        {
            // Esperar hasta que la primera narración termine
            yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
            
            narrationManager.PlayNextNarration();
        }
    }
    
    IEnumerator PlayAdditionalNarrationForCaidaEntorno()
    {
        // Esperar hasta que la primera narración termine
        yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
        
        narrationManager.PlayNextNarration();
    }
}
