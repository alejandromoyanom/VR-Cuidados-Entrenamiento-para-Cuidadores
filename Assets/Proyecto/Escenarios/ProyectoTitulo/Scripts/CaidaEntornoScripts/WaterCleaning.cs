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
    private Collider waterCollider;

    void Start()
    {
        canvas.SetActive(false); // Asegúrate de que el canvas esté desactivado al iniciar el juego
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
    }

    void Update()
    {
        if (!grabInteractable.isSelected)
        {
            StopCleaning(); // Detenemos la limpieza si el objeto no está seleccionado
        }

        if (isCleaning && grabInteractable.isSelected)
        {
            if (fillImage.fillAmount >= 1f)
            {
                RemoveWater();
            }
            else
            {
                UpdateProgressValue(Mathf.Clamp01(fillImage.fillAmount + Time.deltaTime / animationDuration));
            }
        }
    }

    public void StartCleaning()
    {
        isCleaning = true;
        canvas.SetActive(true);
    }

    public void StopCleaning()
    {
        isCleaning = false;
        canvas.SetActive(false);
    }

    private void RemoveWater()
    {
        water.SetActive(false); // O destruir el objeto: Destroy(water);
        isCleaning = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CleaningTool") && grabInteractable.isSelected)
        {
            StartCleaning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CleaningTool"))
        {
            StopCleaning();
        }
    }
    
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Si el paño está dentro del área de agua cuando se recoge, comienza la limpieza
        if (waterCollider != null && waterCollider.bounds.Contains(transform.position))
        {
            StartCleaning();
        }
    }
    
    public void SetWaterCollider(Collider collider)
    {
        waterCollider = collider;
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
