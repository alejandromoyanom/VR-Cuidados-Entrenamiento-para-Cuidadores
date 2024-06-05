using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayVoice : MonoBehaviour
{
    public Action clipFinished;
    public Action clipStarted;
    private AudioSource audioSource;
    private bool hasTalkedFirstTime = false;
    public int numberOfInteractions = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying && hasTalkedFirstTime)
        {
            clipFinished?.Invoke();
        }
    }


     public void PlaySound()
    {
        hasTalkedFirstTime = true;
        audioSource.Play();
        clipStarted?.Invoke();
        numberOfInteractions +=1;

    }
}
