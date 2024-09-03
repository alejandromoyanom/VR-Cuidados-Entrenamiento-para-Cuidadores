using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections;

public class InstructionManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asignar el VideoPlayer
    public NarrationManager narrationManager; // Asignar el NarrationManager
    public DynamicMoveProvider moveProvider; // Asignar el LocomotionProvider del XR Origin
    public Canvas videoCanvas;

    void Start()
    {
        // Desactivar el movimiento al inicio
        moveProvider.enabled = false;

        // Subscribir un método al evento cuando el video termine
        videoPlayer.loopPointReached += OnVideoEnd;
        
        videoCanvas.gameObject.SetActive(true);

        // Iniciar el video
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Detener el video
        videoPlayer.Stop();

        // Reactivar el movimiento
        moveProvider.enabled = true;
        videoCanvas.gameObject.SetActive(false);

        // Iniciar la corrutina para esperar y luego reproducir el audio
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