using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInteract2 : MonoBehaviour
{
    [SerializeField] private InputActionProperty selectButton;
    public Action npcInteract;
    public PlayerInteractUI2 playerInteractUI;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private XRRayInteractor rightHandRayInteractor; // Ray Interactor de la mano derecha
    [SerializeField] private XRRayInteractor leftHandRayInteractor;

    private bool canvasActive = false; 

    void Start()
    {
        uiCanvas.SetActive(false);
        rightHandRayInteractor.gameObject.SetActive(false);
        leftHandRayInteractor.gameObject.SetActive(false);
    }
    void Update()
    {
        float pressed = selectButton.action.ReadValue<float>();
        if (pressed > 0.5f && !canvasActive)
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out DialogueManager dialogueManager))
                {
                    dialogueManager.Interact();
                    npcInteract?.Invoke();
                    playerInteractUI.OnInteract();
                    ActivateCanvas();
                }
            }
        }
    }
    
    public void ActivateCanvas()
    {
        uiCanvas.SetActive(true);
        rightHandRayInteractor.gameObject.SetActive(true);
        leftHandRayInteractor.gameObject.SetActive(true);
        canvasActive = true;
    }

    public void DeactivateCanvas()
    {
        uiCanvas.SetActive(false);
        rightHandRayInteractor.gameObject.SetActive(false);
        leftHandRayInteractor.gameObject.SetActive(false);
        canvasActive = false;
    }
    public DialogueManager GetDialogueManager()
    {
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out DialogueManager dialogueManager))
            {
                return dialogueManager;
            }
        }
        return null;
    }
}