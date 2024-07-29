using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro; // Si estás usando TextMeshPro para el texto de la temperatura

public class TemperatureControl : MonoBehaviour
{
    public TextMeshProUGUI temperatureText; // Referencia al texto que muestra la temperatura
    public float temperature = 18f; // Temperatura inicial
    private float minTemperature = 18f; // Temperatura mínima
    private float maxTemperature = 24f; // Temperatura máxima

    public XRSimpleInteractable increaseButton; // Botón para aumentar la temperatura
    public XRSimpleInteractable decreaseButton; // Botón para disminuir la temperatura

    void Start()
    {
        // Asignar las funciones a los eventos de interacción
        increaseButton.activated.AddListener(IncreaseTemperature);
        decreaseButton.activated.AddListener(DecreaseTemperature);
        
        // Actualizar el texto de la temperatura al inicio
        UpdateTemperatureText();
    }

    private void IncreaseTemperature(ActivateEventArgs args)
    {
        if (temperature < maxTemperature)
        {
            temperature++;
            UpdateTemperatureText();
            if (temperature >= maxTemperature)
            {
                increaseButton.enabled = false;
                decreaseButton.enabled = false;
            }
        }
    }

    private void DecreaseTemperature(ActivateEventArgs args)
    {
        if (temperature > minTemperature)
        {
            temperature--;
            UpdateTemperatureText();
        }
    }

    private void UpdateTemperatureText()
    {
        temperatureText.text = temperature.ToString() + "°";
    }
}