using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private int handsGrabbing = 0;
    private bool isLeftHandFirst = false; // Estado para verificar si la mano izquierda es la primera
    private bool hasStoodUp = false; // Indica si ya se ha levantado
    private bool adjustedAttachPoint = false;
    private bool isSeated = false; // Indica si el personaje está en la silla
    private bool canAnimate = true;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Inicialmente, congelar todas las posiciones y rotaciones
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Suscribirse a los eventos de selección del XRGrabInteractable
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Activar colliders iniciales (asumimos que inicia sentado)
        SetCollidersActive(sittingColliders, true);
        SetCollidersActive(standingColliders, false);
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
        if (handsGrabbing > 0) // Verificar que no sea menor que cero antes de restar
        {
            handsGrabbing--;
        }
       
        if (handsGrabbing <= 0)
        {
            if (isSeated)
            {
                // Si está sentado, mantenerlo en la silla
                //rb.constraints = RigidbodyConstraints.FreezeAll;
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
        if (handsGrabbing >= 2 && !isSeated) // Verificar que no esté en la silla
        { 
            // Permitir el movimiento cuando ambas manos estén sujetando
            rb.constraints = RigidbodyConstraints.None;
            
            if (canAnimate) // Solo activar si las animaciones están permitidas
            {
                animator.SetTrigger("StartLegMovement");
                Debug.Log("Animación de movimiento de piernas activada.");
            }

            // Desmarcar el isTrigger del collider específico al agarrar
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
        }
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
            isSeated = true; // Marcar como sentado
            
            canAnimate = false;


            // Cambiar a la animación de estar sentado
            animator.SetTrigger("SitInChair");
            animator.SetBool("isQuiet", false);
            
            Debug.Log("isQuiet set to: " + animator.GetBool("isQuiet"));
            
            

            // Marcar el isTrigger del collider específico al sentarse en la silla
            if (specificCollider != null)
            {
                specificCollider.isTrigger = true;

                // Mantener los colliders de sentado activos
                SetCollidersActive(sittingColliders, true);
                SetCollidersActive(standingColliders, false);
            }
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
        // Subir la posición del punto de unión derecho en el eje Y por 0.3
        rightAttachPoint.localPosition += new Vector3(0, 0.3f, 0);
    }
}