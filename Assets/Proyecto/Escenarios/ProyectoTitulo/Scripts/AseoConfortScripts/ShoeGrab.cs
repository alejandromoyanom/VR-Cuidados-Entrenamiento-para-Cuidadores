using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShoeGrab : MonoBehaviour
{
    public GameObject socket; // Referencia al socket específico asociado al zapato
    private XRGrabInteractable grabInteractable; // Componente para detectar si está siendo agarrado
    private Rigidbody rb;
    private Collider objectCollider;
    public Transform attachPoint;
    private Transform neutralContainer; // Contenedor neutral en la escena
    private bool isNearSocket = false; // Bandera para verificar si el zapato está cerca del socket

    void Start()
    {
        // Obtener el componente XRGrabInteractable si está presente
        grabInteractable = GetComponent<XRGrabInteractable>();
        neutralContainer = GameObject.Find("NeutralContainer").transform;
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();

        if (grabInteractable == null)
        {
            Debug.LogWarning("No se encontró el componente XRGrabInteractable en este zapato.");
        }
        else
        {
            // Suscribirse al evento de agarre
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            // Suscribirse al evento de soltado
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        // Al agarrar, desbloquear las restricciones para que el zapato pueda moverse
        rb.constraints = RigidbodyConstraints.None;
        
        transform.SetParent(neutralContainer);
        
        // Activar el socket al agarrar el zapato
        socket.SetActive(true);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (isNearSocket)
        {

            objectCollider.enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.SetParent(neutralContainer);
            
            // Alinear la posición y rotación del zapato con el Attach Transform del socket
            transform.position = attachPoint.position;
            transform.rotation = attachPoint.rotation;
            
            // Desactivar el componente XRGrabInteractable para evitar que el zapato se agarre nuevamente
            grabInteractable.enabled = false;

            // Desactivar el socket
            socket.SetActive(false);
        }
        else
        {
            // Desactivar el socket si no está cerca
            socket.SetActive(false);
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
