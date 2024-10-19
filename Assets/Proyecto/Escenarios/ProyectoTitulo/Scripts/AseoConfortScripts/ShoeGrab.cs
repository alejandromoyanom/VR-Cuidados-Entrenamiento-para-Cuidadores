using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShoeGrab : MonoBehaviour
{
    public GameObject socket; // Referencia al socket específico asociado al zapato
    private AudioSource audioSource;
    private XRGrabInteractable grabInteractable; 
    private Rigidbody rb;
    private Collider objectCollider;
    public Transform attachPoint;
    private Transform neutralContainer; 
    private ShoeManager shoeManager; 
    private bool isNearSocket = false; // Bool para verificar si el zapato está cerca del socket
    private Outline outline;
    public GameObject canvasGrab;
    
    // Campos para almacenar la posición y rotación originales
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    
    private static bool narrationPlayed = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        neutralContainer = GameObject.Find("NeutralContainer").transform;
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        shoeManager = FindObjectOfType<ShoeManager>(); 
        audioSource = GetComponent<AudioSource>();
        

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
        
        // Almacenar la posición y rotación globales antes de cambiar el padre
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // Desvincular el zapato del personaje
        transform.SetParent(neutralContainer);

        // Restaurar la posición y rotación globales
        transform.position = savedPosition;
        transform.rotation = savedRotation;
        
        rb.constraints = RigidbodyConstraints.None;
        
        socket.SetActive(true);
        canvasGrab.SetActive(false);
        
        
        if (outline != null)
        {
            outline.enabled = false;
        }
        
        if (!narrationPlayed)
        {
            FindObjectOfType<NarrationManager>().QueueNarration(true);
            narrationPlayed = true;
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (isNearSocket)
        {

            objectCollider.enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.SetParent(neutralContainer);
            audioSource.Play();
            
            // Alinear la posición y rotación del zapato con el Attach Transform del socket
            transform.position = attachPoint.position;
            transform.rotation = attachPoint.rotation;
            
            // Desactivar el componente XRGrabInteractable para evitar que el zapato se agarre nuevamente
            grabInteractable.enabled = false;
            
            shoeManager.ShoePlaced();

            // Desactivar el socket
            socket.SetActive(false);
        }
        else
        {
            // Desactivar el socket si no está cerca
            socket.SetActive(false);
            transform.position = savedPosition;
            transform.rotation = savedRotation;
            rb.constraints = RigidbodyConstraints.FreezeAll;
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
