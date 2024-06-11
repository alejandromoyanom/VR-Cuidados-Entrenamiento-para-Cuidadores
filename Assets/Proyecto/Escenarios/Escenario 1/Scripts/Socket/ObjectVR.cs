using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectVR : MonoBehaviour
{
    public GameObject socket; // Referencia al socket específico asociado al objeto
    private XRGrabInteractable grabInteractable; // Componente para detectar si está siendo agarrado
    private bool isNearSocket = false; // Bandera para verificar si el objeto está cerca del socket
     private Outline outline;

    void Start()
    {
        // Obtener el componente XRGrabInteractable si está presente
        grabInteractable = GetComponent<XRGrabInteractable>();
        outline = GetComponent<Outline>();

        if (grabInteractable == null)
        {
            Debug.LogWarning("No se encontró el componente XRGrabInteractable en este objeto.");
        }
        else
        {
            // Suscribirse al evento de agarrado
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            // Suscribirse al evento de soltado
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        
        // Activar el socket al agarrar el objeto
        socket.SetActive(true);
    }

    void OnReleased(SelectExitEventArgs args)
    {

        if (isNearSocket)
        {
            // Mantener el socket activo si está cerca de él al soltar
            socket.SetActive(true);

            // Posicionar el objeto en el socket
            transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;
            
            if (outline != null)
            {
                outline.enabled = false;
            }

        }
        else
        {
            // Desactivar el socket si no está cerca
            socket.SetActive(false);
            
            if (outline != null)
            {
                outline.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == socket)
        {
            isNearSocket = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == socket)
        {
            isNearSocket = false;
        }
    }
}
