using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonFollowVisual : MonoBehaviour
{
    public TemperatureControl temperatureControl; // Referencia al script que controla la temperatura
    public bool increase; // Si es verdadero, aumentará la temperatura; si es falso, la disminuirá.

    private XRBaseInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed(BaseInteractionEventArgs args)
    {
        if (increase)
        {
            temperatureControl.IncreaseTemperature();
        }
        else
        {
            temperatureControl.DecreaseTemperature();
        }
    }
}
