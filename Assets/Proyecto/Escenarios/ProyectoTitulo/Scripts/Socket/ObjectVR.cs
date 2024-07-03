using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectVR : MonoBehaviour
{
    public GameObject socket; // Referencia al socket específico asociado al objeto
    private XRGrabInteractable grabInteractable; // Componente para detectar si está siendo agarrado
    private bool isNearSocket = false; // Bandera para verificar si el objeto está cerca del socket
    private Outline outline;
    public UISocketCounter uiSocketCounter; // Referencia al script UISocketCounter para incrementar el contador

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
            // Posicionar el objeto en el socket
            transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;

            // Desactivar el componente XRGrabInteractable para evitar que el objeto se agarre nuevamente
            grabInteractable.enabled = false;

            // Incrementar el contador en la UI
            if (uiSocketCounter != null)
            {
                uiSocketCounter.IncrementCounter();
            }

            // Desactivar el outline del objeto
            if (outline != null)
            {
                outline.enabled = false;
            }

            // Desactivar el outline y el socket
            Outline socketOutline = socket.GetComponent<Outline>();
            if (socketOutline != null)
            {
                socketOutline.enabled = false;
            }
            socket.SetActive(false);
        }
        else
        {
            // Desactivar el socket si no está cerca
            socket.SetActive(false);
            
            // Activar el outline del objeto
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
