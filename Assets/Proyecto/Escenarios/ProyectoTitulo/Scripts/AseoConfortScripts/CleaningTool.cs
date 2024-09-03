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
    public AudioSource audioSource;
    public AudioClip cleaningSound;
    public NarrationManager narrationManager;

    private XRGrabInteractable grabInteractable;
    private Collider targetCollider;
    private bool isCleaning = false;
    private bool isCompleted = false; // Indica si la limpieza ya se completó
    private float currentCleaningProgress = 0f;
    private CleaningSequenceManager sequenceManager;
    private Outline outline;
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        targetCollider = targetObject.GetComponent<Collider>();
        
        sequenceManager = FindObjectOfType<CleaningSequenceManager>();
        
        audioSource.clip = cleaningSound;
        
        outline = GetComponent<Outline>();

        // Si el objeto no es el primero en la secuencia, desactivarlo
        if (sequenceManager.GetCurrentTool() != this)
        {
            SetInteractable(false);
        }
        else
        {
            // Activar outline para la primera herramienta en la secuencia
            if (outline != null)
            {
                outline.enabled = true;
            }
        }
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
        if (!isCompleted) // Solo reiniciar el progreso si no se ha completado
        {
            isCleaning = true;
            canvas.SetActive(true);
            
            if (outline != null)
            {
                outline.enabled = false;
            }
            
            if (audioSource != null && cleaningSound != null)
            {
                audioSource.Play();
            }
            
        }
    }

    private void StopCleaning()
    {
        isCleaning = false;
        canvas.SetActive(false); // Opcional, ocultar el canvas si lo deseas al detener la limpieza
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
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
        isCompleted = true; // Marcar la herramienta como completada
        sequenceManager.ToolCompleted(this);
        
        // Detener el sonido al completar la limpiezas
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        if (gameObject.name == "Toalla")
        {
            narrationManager.ContinueWithCurrentNarration(true); // Reproducir todas las narraciones en secuencia
        }
        else
        {
            narrationManager.PlayNextNarration();
        }
        
    }

    public void SetInteractable(bool state)
    {
        if (grabInteractable != null)
        {
            grabInteractable.enabled = state;
            
            if (outline != null)
            {
                outline.enabled = state;
            }

            if (state)
            {
                // Resetear el progreso al activar la herramienta
                ResetProgress();
            }
        }
    }

    private void ResetProgress()
    {
        currentCleaningProgress = 0f;
        fillImage.fillAmount = 0f;
        progressText.text = "";
        isCompleted = false;  
    }
    private void UpdateProgressValue(float progress)
    {
        fillImage.fillAmount = progress;
        progressText.text = $"{(int)(fillImage.fillAmount * 100)}%";
    }
}
