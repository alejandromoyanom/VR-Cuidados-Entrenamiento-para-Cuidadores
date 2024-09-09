using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectVR : MonoBehaviour
{
    public GameObject socket; // Referencia al socket específico asociado al objeto
    public UISocketCounter uiSocketCounter;
    public GameObject canvasGrab;
    
    private XRGrabInteractable grabInteractable; // Componente para detectar si está siendo agarrado
    private bool isNearSocket = false; // Bandera para verificar si el objeto está cerca del socket
    private Outline outline;
    private Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private NarrationManager narrationManager;
    
    

    private static bool NarrationPlayed = false;
    void Start()
    {
        narrationManager = FindObjectOfType<NarrationManager>();
        
        // Obtener el componente XRGrabInteractable si está presente
        grabInteractable = GetComponent<XRGrabInteractable>();
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        initialPosition = transform.position;
        initialRotation = transform.rotation;

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
        rb.constraints = RigidbodyConstraints.None;
        // Activar el socket al agarrar el objeto
        socket.SetActive(true);
        
        
        if (gameObject.name is "Ropa" or "Alcohol" && !NarrationPlayed)
        {
            narrationManager.PlayNextNarration(); // Reproduce la narración
            NarrationPlayed = true; // Marcar como reproducida
            canvasGrab.SetActive(false);
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (isNearSocket)
        {
            // Posicionar el objeto en el socket
            transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;
            rb.constraints = RigidbodyConstraints.FreezeAll;

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
            
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rb.constraints = RigidbodyConstraints.FreezeAll;
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
