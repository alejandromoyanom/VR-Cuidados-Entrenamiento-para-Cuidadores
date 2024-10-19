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
    private ContinuousMoveProviderBase moveProvider;
    private float originalMoveSpeed;

    private const string TutorialCompletedKey = "TutorialCompleted";
    private GameObject canvasObjeto;

    void Start()
    {
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
        {
            faderScreen.SetActive(true);
            narrationManager.PlayNextNarration();
            if (SceneManager.GetActiveScene().name == "CaidaEntorno")
            {
                StartCoroutine(PlayAdditionalNarrationForCaidaEntorno());
            }
            return;
        }
        
        moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
        
        if (moveProvider != null)
        {
            originalMoveSpeed = moveProvider.moveSpeed;
        }
        
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += SyncAndPlay;
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void SyncAndPlay(VideoPlayer vp)
    {
        videoCanvas.gameObject.SetActive(true);
        audioSource.Play();
        videoPlayer.Play();
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = 0f;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.Stop();
        audioSource.Stop();
        videoCanvas.gameObject.SetActive(false);
        
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = originalMoveSpeed;
        }
        
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();
        
        StartCoroutine(PlayNarrationWithDelay());
    }
    
    IEnumerator PlayNarrationWithDelay()
    {
        yield return new WaitForSeconds(1f);
        
        narrationManager.PlayNextNarration();
        
        if (SceneManager.GetActiveScene().name == "CaidaEntorno")
        {
            yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
            
            narrationManager.QueueNarration();
        }
    }
    
    IEnumerator PlayAdditionalNarrationForCaidaEntorno()
    {
        yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
        
        narrationManager.QueueNarration();
    }
}
