using UnityEngine;
using TMPro;
using System.Collections;

public class UISocketCounter : MonoBehaviour
{
    public GameObject paño;
    public int totalObjects = 5; // Total de objetos a colocar
    private int currentCount = 0; // Conteo actual
    private AudioSource audioSource;
    public TextMeshProUGUI counterText; // El texto de la UI
    public GameObject canvas; 
    public NarrationManager narrationManager;
    private float delayBeforeHide = 2f; 

    void Start()
    {
        UpdateCounterText();
        audioSource = GetComponent<AudioSource>();
    }
    

    public void IncrementCounter()
    {
        currentCount++;
        UpdateCounterText();
        audioSource.Play();
        
        
        if (currentCount >= totalObjects)
        {
            narrationManager.PlayNextNarration(true);
            StartCoroutine(HideCanvasAfterDelay());
        }
    }

    private void UpdateCounterText()
    {
        counterText.text = $"<size=60><b>Progreso de la Tarea</b></size>\n<size=48>Objetos posicionados: {currentCount} <color=#00FF00>/</color> {totalObjects}</size>";
    }
    
    private IEnumerator HideCanvasAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeHide);
        canvas.SetActive(false);
        paño.SetActive(true);
    }
}
