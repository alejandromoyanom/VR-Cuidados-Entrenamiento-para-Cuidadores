using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectInteraction : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        // Guardar la posición y rotación inicial
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        // Suscribirse a los eventos
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            // Si aún está siendo agarrado por otra mano, no hacer nada
            return;
        }
        else
        {
            // Restaurar la posición y rotación iniciales al soltar
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

    }
}