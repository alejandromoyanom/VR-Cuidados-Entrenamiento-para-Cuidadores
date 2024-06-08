using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterCleaning : MonoBehaviour
{
    [SerializeField] public GameObject canvas; // Haz esto público para que sea accesible desde ProximityTrigger
    [SerializeField] private TMP_Text percentage;
    [SerializeField] private RawImage barPercentage;
    
    public GameObject water; // Referencia al objeto de agua
    private float valuePercentage = 0f;
    private bool isCleaning = false;

    void Start()
    {
        canvas.SetActive(false); // Asegúrate de que el canvas esté desactivado al iniciar el juego
    }

    void Update()
    {
        if (isCleaning)
        {
            if (valuePercentage >= 100f)
            {
                valuePercentage = 100f;
                canvas.SetActive(false);
                RemoveWater();
            }
            else
            {
                valuePercentage += 0.25f;
                barPercentage.rectTransform.sizeDelta = new Vector2(valuePercentage, barPercentage.rectTransform.sizeDelta.y);
                percentage.text = Math.Floor(valuePercentage).ToString() + "%";
            }
        }
    }

    public void StartCleaning()
    {
        isCleaning = true;
    }

    public void StopCleaning()
    {
        isCleaning = false;
    }

    private void RemoveWater()
    {
        water.SetActive(false); // O destruir el objeto: Destroy(water);
        isCleaning = false;
    }
}