using System.Collections;
using System;
using UnityEngine;

public class PlayVoice : MonoBehaviour
{
    public Action clipFinished;
    public Action clipStarted;
    private AudioSource npcAudioSource;
    private AudioSource playerAudioSource;
    private bool hasTalkedFirstTime = false;
    public AudioClip[] playerClips;
    public AudioClip[] npcClips;
    private int currentClipIndex = 0;
    private bool isPlayerTurn = true;
    public NPCInteractable npcInteractable;  // Referencia al script NPCInteractable

    // Start is called before the first frame update
    void Start()
    {
        npcAudioSource = GetComponent<AudioSource>();
        playerAudioSource = GameObject.FindWithTag("Player").GetComponent<AudioSource>(); // Asegúrate de que el GameObject del personaje tiene el tag "Player"
    }

    // Update is called once per frame
    void Update()
    {
        if (!npcAudioSource.isPlaying && !playerAudioSource.isPlaying && hasTalkedFirstTime)
        {
            clipFinished?.Invoke();
        }
    }

    public void PlaySound()
    {
        hasTalkedFirstTime = true;
        if (isPlayerTurn)
        {
            if (currentClipIndex < playerClips.Length)
            {
                playerAudioSource.clip = playerClips[currentClipIndex];
                playerAudioSource.Play();
                clipStarted?.Invoke();
                isPlayerTurn = false;
            }
        }
        else
        {
            if (currentClipIndex < npcClips.Length)
            {
                npcAudioSource.clip = npcClips[currentClipIndex];
                npcAudioSource.Play();
                clipStarted?.Invoke();
                npcInteractable.StartTalking();  // Notifica al NPCInteractable que el NPC está hablando
                isPlayerTurn = true;
                currentClipIndex++;
            }
        }
    }
    
    public bool IsLastClip()
    {
        return currentClipIndex >= playerClips.Length && currentClipIndex >= npcClips.Length;
    }
}