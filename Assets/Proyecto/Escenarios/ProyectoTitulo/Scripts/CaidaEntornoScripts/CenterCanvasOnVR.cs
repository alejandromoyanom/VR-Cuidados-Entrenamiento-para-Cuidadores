using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCanvasOnVR : MonoBehaviour
{
    public Transform vrCameraTransform; 
    private float distanceFromCamera = 0.8f; 

    void Update()
    {
        Vector3 targetPosition = vrCameraTransform.position + vrCameraTransform.forward * distanceFromCamera;
        
        transform.position = targetPosition;
        
        transform.rotation = vrCameraTransform.rotation;
    }
}