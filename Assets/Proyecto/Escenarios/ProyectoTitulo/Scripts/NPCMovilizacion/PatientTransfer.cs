using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PatientTransfer : MonoBehaviour
{
    public GameObject patient;
    public Transform wheelchairPosition;
    public Collider leftBackContactPoint;
    public Collider rightBackContactPoint;
    public Transform leftHandController;
    public Transform rightHandController;
    public InputActionProperty selectButtonLeft;
    public InputActionProperty selectButtonRight;
    public float moveSpeed = 1.0f;
    public float sitDistanceThreshold = 0.5f;

    private XRGrabInteractable grabInteractable;
    private bool isLeftHandGrabbing = false;
    private bool isRightHandGrabbing = false;
    private bool isTransferStarted = false;

    void Start()
    {
        // Agregar XRGrabInteractable al paciente si no está ya presente
        grabInteractable = patient.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = patient.AddComponent<XRGrabInteractable>();
        }

        // Configurar para permitir múltiples puntos de agarre
        grabInteractable.interactionLayers = LayerMask.GetMask("Default");
        grabInteractable.attachTransform = patient.transform;

        // Desactivar gravedad y asegurarse de que el Rigidbody es kinematic
        Rigidbody rb = patient.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = patient.AddComponent<Rigidbody>();
        }

        // Ajustar la masa del Rigidbody para simular el peso
        rb.mass = 70f; // Masa de una persona promedio

        // Ajustar parámetros de arrastre para simular resistencia
        rb.drag = 5f; // Arrastre lineal
        rb.angularDrag = 10f; // Arrastre angular
    }

    void Update()
    {
        // Verificar si los botones de los mandos VR están presionados
        isLeftHandGrabbing = IsGrabbing(selectButtonLeft, leftHandController, leftBackContactPoint);
        isRightHandGrabbing = IsGrabbing(selectButtonRight, rightHandController, rightBackContactPoint);

        if (isLeftHandGrabbing && isRightHandGrabbing)
        {
            if (!isTransferStarted)
            {
                Debug.Log("Starting Transfer");
                StartTransfer();
            }
            else
            {
                Debug.Log("Siguiendo al usuario");
                FollowUser();
            }
        }
        else if (isTransferStarted)
        {
            // Verificar la distancia antes de sentar al paciente
            float distanceToWheelchair = Vector3.Distance(patient.transform.position, wheelchairPosition.position);
            if (distanceToWheelchair < sitDistanceThreshold)
            {
                Debug.Log("Sitting Patient");
                SitPatient();
            }
        }
    }

    bool IsGrabbing(InputActionProperty selectButton, Transform handController, Collider contactPoint)
    {
        float pressed = selectButton.action.ReadValue<float>();
        if (pressed > 0.5f)
        {
            if (contactPoint.bounds.Contains(handController.position))
            {
                return true;
            }
        }
        return false;
    }

    void StartTransfer()
    {
        // Iniciar la interacción de agarre
        grabInteractable.interactorsSelecting.Add(leftHandController.GetComponent<XRBaseInteractor>());
        grabInteractable.interactorsSelecting.Add(rightHandController.GetComponent<XRBaseInteractor>());
        isTransferStarted = true;
    }

    void FollowUser()
    {
        // Aplicar una fuerza de resistencia al Rigidbody del NPC
        Vector3 directionToUser = (leftHandController.position - patient.transform.position).normalized;
        float distanceToUser = Vector3.Distance(leftHandController.position, patient.transform.position);

        Rigidbody rb = patient.GetComponent<Rigidbody>();
        if (distanceToUser > 0.1f)
        {
            Vector3 force = directionToUser * moveSpeed * rb.mass;
            rb.AddForce(force);
        }

        // Asegurarse de que el NPC se mueva con el usuario
        Vector3 targetPosition = leftHandController.position + leftHandController.forward * 0.5f;
        patient.transform.position = Vector3.MoveTowards(patient.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void SitPatient()
    {
        // Cambiar la posición del paciente a una postura sentada
        patient.transform.position = wheelchairPosition.position;
        patient.transform.rotation = wheelchairPosition.rotation;

        // Detener la interacción de agarre
        grabInteractable.interactorsSelecting.Clear();

        // Detener la transferencia
        isTransferStarted = false;
    }
}
