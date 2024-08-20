using UnityEngine;
using TMPro;
using System.Collections;

public class UISocketCounter : MonoBehaviour
{
    public Transform vrCamera; // La cámara VR
    public int totalObjects = 5; // Total de objetos a colocar
    private int currentCount = 0; // Conteo actual
    public TextMeshProUGUI counterText; // El texto de la UI
    public GameObject canvas; // Referencia al Canvas
    public float delayBeforeHide = 2f; // Tiempo de espera antes de desactivar el Canvas

    void Start()
    {
        UpdateCounterText();
    }

    void Update()
    {
        // Hacer que el Canvas siga la cámara
        Vector3 cameraPosition = vrCamera.position + vrCamera.forward * 1.0f; // Ajusta la distancia según sea necesario
        transform.position = cameraPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - vrCamera.position);
    }

    public void IncrementCounter()
    {
        currentCount++;
        UpdateCounterText();
        
        if (currentCount >= totalObjects)
        {
            StartCoroutine(HideCanvasAfterDelay());
        }
    }

    private void UpdateCounterText()
    {
        //counterText.text = $"<b>Objetos posicionados:</b> {currentCount} <color=#00FF00>/</color> {totalObjects}";
        counterText.text = $"<size=60><b>Progreso de la Tarea</b></size>\n<size=48>Objetos posicionados: {currentCount} <color=#00FF00>/</color> {totalObjects}</size>";
    }
    
    private IEnumerator HideCanvasAfterDelay()
    {
        // Esperar el tiempo especificado antes de desactivar el Canvas
        yield return new WaitForSeconds(delayBeforeHide);
        canvas.SetActive(false);
    }
}
