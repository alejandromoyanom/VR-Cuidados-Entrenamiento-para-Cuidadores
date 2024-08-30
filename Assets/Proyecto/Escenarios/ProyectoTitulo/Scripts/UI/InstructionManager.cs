using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InstructionManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asignar el VideoPlayer
    public NarrationManager narrationManager; // Asignar el NarrationManager
    public DynamicMoveProvider moveProvider; // Asignar el LocomotionProvider del XR Origin

    void Start()
    {
        // Desactivar el movimiento al inicio
        moveProvider.enabled = false;

        // Subscribir un método al evento cuando el video termine
        videoPlayer.loopPointReached += OnVideoEnd;

        // Iniciar el video
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Detener el video
        videoPlayer.Stop();

        // Reactivar el movimiento
        moveProvider.enabled = true;

        // Iniciar la primera narración
        narrationManager.PlayNextNarration();
    }
}