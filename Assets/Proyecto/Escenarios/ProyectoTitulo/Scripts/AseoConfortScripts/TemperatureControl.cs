using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro; // Si estás usando TextMeshPro para el texto de la temperatura

public class TemperatureControl : MonoBehaviour
{
    public TextMeshProUGUI temperatureText; // Referencia al texto que muestra la temperatura
    public float temperature = 18f; // Temperatura inicial
    private float minTemperature = 18f; // Temperatura mínima
    private float maxTemperature = 23f; // Temperatura máxima
    private AudioSource audioSource;
    private Outline outlineLeft;
    private Outline outlineRight;
    private XRGrabInteractable grabInteractableLeft;
    private XRGrabInteractable grabInteractableRight;

    public XRSimpleInteractable increaseButton; // Botón para aumentar la temperatura
    public XRSimpleInteractable decreaseButton; // Botón para disminuir la temperatura
    public NarrationManager NarrationManager;
    public GameObject leftShoe;
    public GameObject rightShoe;
    public GameObject canvasTrigger;
    public GameObject canvasGrab;
     
    

    void Start()
    {
        UpdateTemperatureText();
        audioSource = GetComponent<AudioSource>();
        outlineLeft = leftShoe.GetComponent<Outline>();
        outlineRight = rightShoe.GetComponent<Outline>();
        grabInteractableLeft = leftShoe.GetComponent<XRGrabInteractable>();
        grabInteractableRight = rightShoe.GetComponent<XRGrabInteractable>();
    }

    public void IncreaseTemperature()
    {
        if (temperature < maxTemperature)
        {
            temperature++;
            audioSource.Play();
            UpdateTemperatureText();
            if (temperature >= maxTemperature)
            {
                increaseButton.enabled = false;
                decreaseButton.enabled = false;
                NarrationManager.PlayNextNarration();
                outlineLeft.enabled = true;
                outlineRight.enabled = true;
                grabInteractableLeft.enabled = true;
                grabInteractableRight.enabled = true;
                canvasTrigger.SetActive(false);
                canvasGrab.SetActive(true);
            }
        }
    }

    public void DecreaseTemperature()
    {
        if (temperature > minTemperature)
        {
            temperature--;
            audioSource.Play();
            UpdateTemperatureText();
        }
    }

    private void UpdateTemperatureText()
    {
        temperatureText.text = temperature.ToString() + "°";
    }
}