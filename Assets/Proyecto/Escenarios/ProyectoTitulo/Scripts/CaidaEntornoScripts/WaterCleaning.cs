using System;
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

    private bool isCleaning = false;
    private XRGrabInteractable grabInteractable;
    public BoxCollider waterCollider;
    private bool isInsideWaterCollider = false; // Para verificar si el paño está dentro del agua

    void Start()
    {
        canvas.SetActive(false); // Asegúrate de que el canvas esté desactivado al iniciar el juego
        grabInteractable = GetComponent<XRGrabInteractable>();

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
        if (isCleaning) // Solo detener si se estaba limpiando
        {
            isCleaning = false;
            canvas.SetActive(false);
        }
    }

    private void CompleteCleaning()
    {
        fillImage.fillAmount = 1f;
        canvas.SetActive(false);
        water.SetActive(false); // O destruir el objeto: Destroy(water);
        isCleaning = false;
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Verificar si el paño está dentro del collider del agua cuando se recoge
        if (isInsideWaterCollider)
        {
            StartCleaning();
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
}
