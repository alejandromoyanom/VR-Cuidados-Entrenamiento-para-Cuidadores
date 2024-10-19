using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer; 
    private AudioSource audioSource;  

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += SyncAndPlay;
        
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    
    void SyncAndPlay(VideoPlayer vp)
    {
        audioSource.Play();
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(WaitForAudioToFinish());
    }
    
    IEnumerator WaitForAudioToFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null; 
        }
        SceneManager.LoadScene("SelectionScene");
    }
    
}
