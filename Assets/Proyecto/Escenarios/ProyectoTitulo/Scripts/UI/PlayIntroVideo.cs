using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayIntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Canvas videoCanvas;
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    void Start()
    {
        // Desactivar el movimiento y los rayos al iniciar el video
        leftRayInteractor.enabled = false;
        rightRayInteractor.enabled = false;

        videoCanvas.gameObject.SetActive(true);
        videoPlayer.Play();

        // Detectar cuando el video ha terminado
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        leftRayInteractor.enabled = true;
        rightRayInteractor.enabled = true;

        // Desactivar el Canvas para ocultar el video
        videoCanvas.gameObject.SetActive(false);
    }
}