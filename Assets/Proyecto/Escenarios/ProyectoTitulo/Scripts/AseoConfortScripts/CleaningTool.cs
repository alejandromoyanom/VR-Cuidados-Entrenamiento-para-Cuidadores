using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class CleaningTool : MonoBehaviour
{
    public GameObject targetObject;  // Objeto objetivo (e.g., los pies)
    public float cleaningTime = 3f;  // Tiempo necesario para completar la limpieza
    public GameObject canvas;        // Canvas con la barra de progreso
    public Image fillImage;          // Imagen de la barra de progreso
    public TMP_Text progressText;    // Texto del progreso

    private XRGrabInteractable grabInteractable;
    private Collider targetCollider;
    private bool isCleaning = false;
    private float currentCleaningProgress = 0f;
    private CleaningSequenceManager sequenceManager;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        targetCollider = targetObject.GetComponent<Collider>();

        // Encontrar el manager
        sequenceManager = FindObjectOfType<CleaningSequenceManager>();

        // Iniciar con el grab interactable desactivado
        //SetInteractable(false);
    }

    void Update()
    {
        if (isCleaning)
        {
            UpdateCleaningProcess();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == targetCollider && grabInteractable.isSelected)
        {
            StartCleaning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == targetCollider)
        {
            StopCleaning();
        }
    }

    private void StartCleaning()
    {
        isCleaning = true;
        canvas.SetActive(true); // Reactivar el canvas al iniciar la limpieza
    }

    private void StopCleaning()
    {
        isCleaning = false;
        canvas.SetActive(false); 
    }

    private void UpdateCleaningProcess()
    {
        if (fillImage.fillAmount >= 1f)
        {
            CompleteCleaning();
        }
        else
        {
            currentCleaningProgress += Time.deltaTime / cleaningTime;
            UpdateProgressValue(currentCleaningProgress);
        }
    }

    private void CompleteCleaning()
    {
        fillImage.fillAmount = 1f;
        progressText.text = "100%";
        canvas.SetActive(false);
        sequenceManager.ToolCompleted(this);
    }

    public void SetInteractable(bool state)
    {
        grabInteractable.enabled = state;
    }

    private void UpdateProgressValue(float progress)
    {
        fillImage.fillAmount = progress;
        progressText.text = $"{(int)(fillImage.fillAmount * 100)}%";
    }
}
