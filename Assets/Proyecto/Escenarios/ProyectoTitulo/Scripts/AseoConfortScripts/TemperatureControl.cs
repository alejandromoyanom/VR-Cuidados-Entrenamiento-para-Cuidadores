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

    public XRSimpleInteractable increaseButton; // Botón para aumentar la temperatura
    public XRSimpleInteractable decreaseButton; // Botón para disminuir la temperatura
    public NarrationManager NarrationManager;

    void Start()
    {
        UpdateTemperatureText();
        audioSource = GetComponent<AudioSource>();
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