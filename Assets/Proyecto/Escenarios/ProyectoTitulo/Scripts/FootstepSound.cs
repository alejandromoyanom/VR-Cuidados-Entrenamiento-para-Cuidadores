using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FootstepSound : MonoBehaviour
{
    public AudioClip footstepAudioClip; // Clip de audio de las pisadas
    public float footstepInterval = 0.5f; // Intervalo entre las pisadas

    private AudioSource audioSource;
    private CharacterController characterController;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Reproducir sonido de pisadas si el jugador se está moviendo
        if (characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= footstepInterval)
            {
                audioSource.PlayOneShot(footstepAudioClip);
                stepTimer = 0f;
            }
        }
        else
        {
            // Reiniciar el temporizador cuando el jugador se detiene
            stepTimer = footstepInterval;
        }
    }
}