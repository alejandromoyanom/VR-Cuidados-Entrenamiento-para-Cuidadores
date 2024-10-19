using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectVR : MonoBehaviour
{
    public GameObject socket; 
    public UISocketCounter uiSocketCounter;
    public GameObject canvasGrab;
    public GameObject canvasGrab2;
    
    private XRGrabInteractable grabInteractable; 
    private bool isNearSocket = false; 
    private Outline outline;
    private Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private NarrationManager narrationManager;
    
    

    private static bool NarrationPlayed = false;
    void Start()
    {
        narrationManager = FindObjectOfType<NarrationManager>();
        
        
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
            
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        rb.constraints = RigidbodyConstraints.None;
        socket.SetActive(true);
        
        
        if (gameObject.name is "Ropa" or "Alcohol" && !NarrationPlayed)
        {
            narrationManager.PlayNextNarration(); 
            NarrationPlayed = true; 
            canvasGrab.SetActive(false);
            canvasGrab2.SetActive(false);
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            return;
        }
        
        if (isNearSocket)
        {
            transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            
            grabInteractable.enabled = false;
            
            if (uiSocketCounter != null)
            {
                uiSocketCounter.IncrementCounter();
            }
            
            if (outline != null)
            {
                outline.enabled = false;
            }
            
            Outline socketOutline = socket.GetComponent<Outline>();
            if (socketOutline != null)
            {
                socketOutline.enabled = false;
            }
            socket.SetActive(false);
        }
        else
        {
            // Si el objeto está suelto y no está cerca del socket, vuelve a la posición inicial
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            
            rb.constraints = RigidbodyConstraints.FreezeAll;

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
