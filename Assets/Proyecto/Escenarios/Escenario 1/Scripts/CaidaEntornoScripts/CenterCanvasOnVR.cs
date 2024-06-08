using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCanvasOnVR : MonoBehaviour
{
    public Transform vrCameraTransform; // Campo público para el transform del visor o controlador de VR
    private float distanceFromCamera = 0.8f; // Distancia del canvas al visor de VR

    void Update()
    {
        // Calcular la posición del canvas centrado en el visor de VR
        Vector3 targetPosition = vrCameraTransform.position + vrCameraTransform.forward * distanceFromCamera;
        
        // Aplicar la posición al canvas
        transform.position = targetPosition;
        
        // Mantener la rotación del canvas igual a la del visor o controlador de VR si es necesario
        transform.rotation = vrCameraTransform.rotation;
    }
}