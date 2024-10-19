using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections;

public class InstructionManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public NarrationManager narrationManager; 
    public DynamicMoveProvider moveProvider; 
    public Canvas videoCanvas;

    void Start()
    {
        
        moveProvider.enabled = false;
        videoPlayer.loopPointReached += OnVideoEnd;
        videoCanvas.gameObject.SetActive(true);
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.Stop();
        moveProvider.enabled = true;
        videoCanvas.gameObject.SetActive(false);
        
        StartCoroutine(PlayNarrationWithDelay());
    }

    IEnumerator PlayNarrationWithDelay()
    {
        
        yield return new WaitForSeconds(2f);
        narrationManager.PlayNextNarration();
    }
}