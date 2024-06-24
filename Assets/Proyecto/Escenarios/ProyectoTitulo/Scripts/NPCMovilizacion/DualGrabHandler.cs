using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DualGrabHandler : MonoBehaviour
{
    public Transform specificReleaseLocation; // La ubicación específica donde se puede soltar el objeto
    private XRGrabInteractable grabInteractable;
    private bool isGrabbingWithBothHands = false;
    private XRBaseInteractor leftHandInteractor;
    private XRBaseInteractor rightHandInteractor;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            if (leftHandInteractor == null)
            {
                leftHandInteractor = interactor;
            }
            else if (rightHandInteractor == null)
            {
                rightHandInteractor = interactor;
                isGrabbingWithBothHands = true;
            }
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            if (interactor == leftHandInteractor)
            {
                leftHandInteractor = null;
            }
            else if (interactor == rightHandInteractor)
            {
                rightHandInteractor = null;
            }

            if (leftHandInteractor == null || rightHandInteractor == null)
            {
                isGrabbingWithBothHands = false;
            }
        }
    }

    void Update()
    {
        if (isGrabbingWithBothHands)
        {
            if (IsObjectInSpecificLocation())
            {
                // Liberar el objeto cuando esté en la ubicación específica
                ReleaseObject();
            }
        }
    }

    bool IsObjectInSpecificLocation()
    {
        // Comprobar si el objeto está en la ubicación específica
        float distance = Vector3.Distance(transform.position, specificReleaseLocation.position);
        return distance < 0.1f; // Ajustar el valor según sea necesario
    }

    void ReleaseObject()
    {
        // Forzar la liberación del objeto
        if (leftHandInteractor != null)
        {
            leftHandInteractor.interactionManager.SelectExit(leftHandInteractor, grabInteractable);
        }

        if (rightHandInteractor != null)
        {
            rightHandInteractor.interactionManager.SelectExit(rightHandInteractor, grabInteractable);
        }

        leftHandInteractor = null;
        rightHandInteractor = null;
        isGrabbingWithBothHands = false;
    }
}
