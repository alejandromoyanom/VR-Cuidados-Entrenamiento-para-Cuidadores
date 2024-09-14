using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterCleaning : MonoBehaviour
{
    [SerializeField] public GameObject canvas; // Haz esto público para que sea accesible desde ProximityTrigger
    [SerializeField] private TMP_Text progressText; // Texto que muestra el porcentaje
    [SerializeField] private Image fillImage; // Imagen que representa la barra de progreso
    [SerializeField] private float animationDuration = 5f; // Duración de la animación

    public GameObject water; // Referencia al objeto de agua
    public NarrationManager narrationManager;
    public GameObject punto;

    private bool isCleaning = false;
    private bool narrationPlayed = false; // Para asegurar que la narración se reproduce solo una vez
    private XRGrabInteractable grabInteractable;
    private BoxCollider waterCollider;
    private bool isInsideWaterCollider = false; // Para verificar si el paño está dentro del agua
    private AudioSource audiosource;

    void Start()
    {
        canvas.SetActive(false); // Asegúrate de que el canvas esté desactivado al iniciar el juego
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
        if (!isCleaning) // Solo iniciar si no se está limpiando ya
        {
            isCleaning = true;
            canvas.SetActive(true);
            audiosource.Play();
        }

        if (fillImage.fillAmount < 1f)
        {
            UpdateProgressValue(Mathf.Clamp01(fillImage.fillAmount + Time.deltaTime / animationDuration));
            audiosource.Play();
        }
        else
        {
            CompleteCleaning();
        }
    }

    private void StopCleaning()
    {
        if (isCleaning) // Solo detener si se estaba limpiando
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
        audiosource.Stop();
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
        UpdateProgressValue(0f); // Resetea la barra de progreso a 0
    }

    private void UpdateProgressValue(float targetValue)
    {
        fillImage.fillAmount = targetValue;
        progressText.text = $"{(int)(fillImage.fillAmount * 100)}%";
    }
    
    IEnumerator PlayAdditionalAudios()
    {
        // Esperar hasta que la primera narración termine
        yield return new WaitForSeconds(narrationManager.GetCurrentNarrationDuration() + 1f);
        
        narrationManager.PlayNarrationSequenceFromCurrent();
    }
}
