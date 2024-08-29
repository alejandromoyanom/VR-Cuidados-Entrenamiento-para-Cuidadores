using System.Collections.Generic;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    
    public List<AudioClip> narrations; // Lista de clips de narración
    private int currentNarrationIndex = -1; // Índice de la narración actual

    private AudioSource narrationAudioSource;
    public AudioSource backgroundAudioSource; // AudioSource para el audio de fondo

    void Start()
    {
        narrationAudioSource = GetComponent<AudioSource>();
        if (narrationAudioSource == null)
        {
            Debug.LogError("No AudioSource found on this GameObject.");
        }
    }

    public void PlayNarration(int index)
    {
        if (index >= 0 && index < narrations.Count)
        {
            currentNarrationIndex = index;
            PlayCurrentNarration();
        }
        else
        {
            Debug.LogWarning("Index out of range.");
        }
    }

    public void PlayNextNarration()
    {
        if (currentNarrationIndex < narrations.Count - 1)
        {
            currentNarrationIndex++;
            PlayCurrentNarration();
        }
        else
        {
            Debug.Log("No more narrations to play.");
        }
    }

    private void PlayCurrentNarration()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Pause(); // Pausar el audio de fondo
        }

        narrationAudioSource.clip = narrations[currentNarrationIndex];
        narrationAudioSource.Play();

        Invoke("ResumeBackgroundAudio", narrationAudioSource.clip.length);
    }

    private void ResumeBackgroundAudio()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.UnPause(); // Reanudar el audio de fondo
        }
    }
}