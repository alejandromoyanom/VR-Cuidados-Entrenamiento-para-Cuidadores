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

        // Subscribir al evento cuando el video termina
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    
    void SyncAndPlay(VideoPlayer vp)
    {
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
        SceneManager.LoadScene("SelectionScene");
    }
    
}
