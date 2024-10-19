using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterCleaning : MonoBehaviour
{
    [SerializeField] public GameObject canvas; 
    [SerializeField] private TMP_Text progressText; 
    [SerializeField] private Image fillImage; 
    [SerializeField] private float animationDuration = 5f; 

    public GameObject water; 
    public NarrationManager narrationManager;
    public GameObject punto;

    private bool isCleaning = false;
    private bool narrationPlayed = false; 
    private XRGrabInteractable grabInteractable;
    private BoxCollider waterCollider;
    private bool isInsideWaterCollider = false; 
    private AudioSource audiosource;

    void Start()
    {
        canvas.SetActive(false); 
        grabInteractable = GetComponent<XRGrabInteractable>();
        waterCollider = water.GetComponent<BoxCollider>();
        audiosource = GetComponent<AudioSource>();

        // Suscribirse al evento de selección
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        grabInteractable.selectExited.AddListener(OnSelectExit);
    }

    void Update()
    {
        if (isInsideWaterCollider && grabInteractable.isSelected)
        {
            StartCleaning();
        }
        else
        {
            StopCleaning();
        }
    }

    private void StartCleaning()
    {
        if (!isCleaning) 
        {
            isCleaning = true;
            canvas.SetActive(true);
            if (!audiosource.isPlaying) 
            {
                audiosource.Play();
            }
        }

        if (fillImage.fillAmount < 1f)
        {
            UpdateProgressValue(Mathf.Clamp01(fillImage.fillAmount + Time.deltaTime / animationDuration));
        }
        else
        {
            CompleteCleaning();
        }
    }

    private void StopCleaning()
    {
        if (isCleaning) 
        {
            isCleaning = false;
            canvas.SetActive(false);
            audiosource.Stop();
        }
    }

    private void CompleteCleaning()
    {
        fillImage.fillAmount = 1f;
        canvas.SetActive(false);
        water.SetActive(false);
        isCleaning = false;
        if (audiosource.isPlaying)
        {
            audiosource.Stop();
        }
        narrationManager.PlayNextNarration();
        
        StartCoroutine(PlayAdditionalAudios());
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Verificar si el paño está dentro del collider del agua cuando se recoge
        if (isInsideWaterCollider)
        {
            StartCleaning();
        }

        // Reproducir la narración solo una vez
        if (!narrationPlayed)
        {
            narrationManager.PlayNextNarration();
            punto.SetActive(true);
            narrationPlayed = true;
        }
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        StopCleaning();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == waterCollider)
        {
            isInsideWaterCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == waterCollider)
        {
            isInsideWaterCollider = false;
        }
    }

    // --- Métodos de Progress Bar ---

    private void ResetProgressBar()
    {
        UpdateProgressValue(0f);
    }

    private void UpdateProgressValue(float targetValue)
    {
        fillImage.fillAmount = targetValue;
        progressText.text = $"{(int)(fillImage.fillAmount * 100)}%";
    }
    
    IEnumerator PlayAdditionalAudios()
    {
        yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
        
        narrationManager.PlayFinalScene();
    }
}
