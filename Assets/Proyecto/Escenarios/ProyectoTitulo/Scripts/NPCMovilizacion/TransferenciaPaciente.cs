using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class TransferenciaPaciente : MonoBehaviour
{
    public Transform wheelchairPosition;
    public Transform leftAttachPoint;
    public Transform rightAttachPoint;
    public InputActionProperty selectButtonLeft;
    public InputActionProperty selectButtonRight;

    private XRGrabInteractable grabInteractable;
    private bool isLeftHandGrabbing = false;
    private bool isRightHandGrabbing = false;
    private bool isTransferStarted = false;

    private Rigidbody patientRigidbody;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        patientRigidbody = GetComponent<Rigidbody>();
        patientRigidbody.isKinematic = true; // Start with kinematic

        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        grabInteractable.selectExited.AddListener(OnSelectExit);
    }

    void Update()
    {
        // Check if both hands are grabbing
        isLeftHandGrabbing = IsGrabbing(selectButtonLeft, leftAttachPoint);
        isRightHandGrabbing = IsGrabbing(selectButtonRight, rightAttachPoint);

        if (isLeftHandGrabbing && isRightHandGrabbing)
        {
            if (!isTransferStarted)
            {
                StartTransfer();
            }
        }
    }

    bool IsGrabbing(InputActionProperty selectButton, Transform attachPoint)
    {
        float pressed = selectButton.action.ReadValue<float>();
        if (pressed > 0.5f)
        {
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTransferStarted && other.CompareTag("Wheelchair"))
        {
            SitPatient();
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        var interactorTransform = args.interactorObject.transform;

        if (interactorTransform == leftAttachPoint)
        {
            isLeftHandGrabbing = true;
        }
        else if (interactorTransform == rightAttachPoint)
        {
            isRightHandGrabbing = true;
        }

        if (isLeftHandGrabbing && isRightHandGrabbing && !isTransferStarted)
        {
            StartTransfer();
        }
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        var interactorTransform = args.interactorObject.transform;

        if (interactorTransform == leftAttachPoint)
        {
            isLeftHandGrabbing = false;
        }
        else if (interactorTransform == rightAttachPoint)
        {
            isRightHandGrabbing = false;
        }

        if (!isLeftHandGrabbing && !isRightHandGrabbing && isTransferStarted)
        {
            // If both hands are released during the transfer, stop the transfer
            isTransferStarted = false;
            patientRigidbody.isKinematic = true;
        }
    }

    void StartTransfer()
    {
        isTransferStarted = true;
        patientRigidbody.isKinematic = false;
        Debug.Log("Transfer Started");
    }

    void SitPatient()
    {
        transform.position = wheelchairPosition.position;
        transform.rotation = wheelchairPosition.rotation;
        isTransferStarted = false;
        patientRigidbody.isKinematic = true; // Set kinematic when seated
        Debug.Log("Patient Seated");
    }
}