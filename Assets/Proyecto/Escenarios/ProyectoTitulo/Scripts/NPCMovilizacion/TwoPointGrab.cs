using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TwoPointGrab : MonoBehaviour
{
    public Transform seatPosition; // Posición de la silla
    public Animator animator; // Animator para las piernas del NPC
    public Vector3 seatOffset; // Offset para ajustar la posición en la silla
    public Collider specificCollider; // Collider específico para manejar el estado de isTrigger

    public Collider[] sittingColliders; // Colliders usados cuando está sentado
    public Collider[] standingColliders; // Colliders usados cuando está de pie
    public Transform[] snapPoints;
    
    public Transform rightAttachPoint;
    public NarrationManager narrationManager;
    public GameObject canvasGrab;
    public GameObject canvasGrab2;
    
    private float slowMoveSpeed = 0.3f; // Velocidad reducida al levantar el objeto
    private float slowTurnSpeed = 20f; // Velocidad de giro reducida al levantar el objeto
    private float originalMoveSpeed;
    private float originalTurnSpeed;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private int handsGrabbing = 0;
    private bool isLeftHandFirst = false; // Estado para verificar si la mano izquierda es la primera
    private bool hasStoodUp = false; // Indica si ya se ha levantado
    private bool adjustedAttachPoint = false;
    private bool isSeated = false; // Indica si el personaje está en la silla
    private bool canAnimate = true;
    private bool narracion = false;

    private ContinuousMoveProviderBase moveProvider;
    private ContinuousTurnProviderBase turnProvider;
    private bool shouldReduceSpeed = false; // Bandera para reducir la velocidad
    private bool shouldRestoreSpeed = false; // Bandera para restaurar la velocidad

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Obtener los componentes de movimiento y giro del XR Origin
        moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
        turnProvider = FindObjectOfType<ContinuousTurnProviderBase>();
        
        
        // Guardar las velocidades originales
        if (moveProvider != null)
        {
            originalMoveSpeed = moveProvider.moveSpeed;
        }

        if (turnProvider != null)
        {
            originalTurnSpeed = turnProvider.turnSpeed;
        }

        // Inicialmente, congelar todas las posiciones y rotaciones
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Suscribirse a los eventos de selección del XRGrabInteractable
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Activar colliders iniciales
        SetCollidersActive(sittingColliders, true);
        SetCollidersActive(standingColliders, false);
    }
    
    void Update()
    {
        if (shouldReduceSpeed && moveProvider != null)
        {
            moveProvider.moveSpeed = slowMoveSpeed;
            turnProvider.turnSpeed = slowTurnSpeed;
        }

        if (shouldRestoreSpeed && moveProvider != null)
        {
            moveProvider.moveSpeed = originalMoveSpeed;
            turnProvider.turnSpeed = originalTurnSpeed;
        }
    }
    

    void OnGrab(SelectEnterEventArgs args)
    {
        if (isSeated)
        {
            return; // No permitir el agarre si está sentado
        }
        
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            // La mano izquierda ha sido la primera en agarrar
            isLeftHandFirst = true;
            handsGrabbing++;
        }
        else if (isLeftHandFirst)
        {
            // Permitir que la mano derecha agarre solo si la izquierda ya lo ha hecho
            handsGrabbing++;
        }
        else
        {
            // Si la mano derecha intenta agarrar primero, liberar el agarre
            args.interactorObject.transform.GetComponent<XRBaseInteractor>().interactionManager.SelectExit(args.interactorObject, grabInteractable);
            return;
        }

        // Comprobar si ambas manos están agarrando
        CheckBothHandsGrabbing();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (handsGrabbing > 0) 
        {
            handsGrabbing--;
        }
       
        if (handsGrabbing <= 0)
        {
            if (isSeated)
            {
                return;
            }
            if (!hasStoodUp)
            {
                // Resetear bandera de mano izquierda
                isLeftHandFirst = false;
            }
            else if (hasStoodUp && !isSeated)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                

                Transform nearestSnapPoint = GetNearestSnapPoint();
                if (nearestSnapPoint != null)
                {
                    transform.position = nearestSnapPoint.position;
                    transform.rotation = nearestSnapPoint.rotation;
                }
                
                // Cambiar a la animación de estar parado solo si ambas manos se sueltan
                animator.SetBool("isQuiet", true);
               
                rb.constraints = RigidbodyConstraints.FreezeAll;
                RestoreMovementAndTurnSpeeds();

                // Resetear bandera de mano izquierda
                isLeftHandFirst = false;

                // Cambiar colliders a los de pie
                SetCollidersActive(sittingColliders, false);
                SetCollidersActive(standingColliders, true);
            }
        }
    }

    void CheckBothHandsGrabbing()
    {
        if (handsGrabbing >= 2 && !isSeated) 
        { 
            rb.constraints = RigidbodyConstraints.None;
            canvasGrab.SetActive(false);
            canvasGrab2.SetActive(false);

            if (!narracion)
            {
                narrationManager.PlayThreeNarrations();
                narracion = true;
            }
            
            if (canAnimate) 
            {
                
                animator.SetBool("StartLegMovement", true);
            }

           
            if (specificCollider != null)
            {
                specificCollider.isTrigger = false;
            }

            hasStoodUp = true;
            
            if (!adjustedAttachPoint)
            {
                AdjustAttachPoint();
                adjustedAttachPoint = true;
            }
            
            shouldReduceSpeed = true; 
            shouldRestoreSpeed = false; 
        }
    }
    


    void RestoreMovementAndTurnSpeeds()
    {
        shouldReduceSpeed = false; // Desactivar la reducción de velocidad
        shouldRestoreSpeed = true; // Activar la restauración de velocidad
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wheelchair"))
        {
            // Colocar al personaje en la posición de la silla con un pequeño ajuste
            transform.position = seatPosition.position + seatOffset;
            transform.rotation = seatPosition.rotation;

            // Congelar el movimiento en la silla
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
            handsGrabbing = 0;
            isLeftHandFirst = false;
            isSeated = true;
            
            canAnimate = false;
            
            RestoreMovementAndTurnSpeeds();


            // Cambiar a la animación de estar sentado
            animator.SetTrigger("SitInChair");
            animator.SetBool("StartLegMovement", false);
            animator.SetBool("isQuiet", false);
            
            

            
            if (specificCollider != null)
            {
                specificCollider.isTrigger = true;
                
                SetCollidersActive(sittingColliders, true);
                SetCollidersActive(standingColliders, false);
            }
            narrationManager.PlayFinalScene();
        }
    }

    void SetCollidersActive(Collider[] colliders, bool isActive)
    {
        foreach (Collider col in colliders)
        {
            col.enabled = isActive;
        }
    }
    
    Transform GetNearestSnapPoint()
    {
        if (isSeated) return null; // No buscar snap points si está en la silla

        Transform nearestPoint = null;
        float nearestDistance = float.MaxValue;

        foreach (Transform point in snapPoints)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPoint = point;
            }
        }

        return nearestPoint;
    }
    
    void AdjustAttachPoint()
    {
        rightAttachPoint.localPosition += new Vector3(0, 0.3f, 0);
    }
}
