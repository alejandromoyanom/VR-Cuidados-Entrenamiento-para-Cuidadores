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
    public Transform vrCamera; // La cámara VR
    
    
    private float valuePercentage = 0f;
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
            canvas.SetActive(false);
            isCleaning = false;
        }
        
        if (isCleaning && grabInteractable.isSelected)
        {
            if (valuePercentage >= 100f)
            {
                valuePercentage = 100f;
                canvas.SetActive(false);
                RemoveWater();
            }
            else
            {
                valuePercentage += 0.3f;
                barPercentage.rectTransform.sizeDelta = new Vector2(valuePercentage, barPercentage.rectTransform.sizeDelta.y);
                percentage.text = Math.Floor(valuePercentage).ToString() + "%";
            }
        }
    }

    public void StartCleaning()
    {
        Vector3 cameraPosition = vrCamera.position + vrCamera.forward * 1.0f; // Ajusta la distancia según sea necesario
        transform.position = cameraPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - vrCamera.position);
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
    
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Verificar si el paño está dentro del collider del agua cuando se recoge
        if (waterCollider != null && waterCollider.bounds.Contains(transform.position))
        {
            StartCleaning();
        }
    }
    
    public void SetWaterCollider(Collider collider)
    {
        waterCollider = collider;
    }
}