using System;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInteractUI2 : MonoBehaviour
{
    [SerializeField] private GameObject containerUI;
    [SerializeField] private PlayerInteract2 playerInteract;
    [SerializeField] private Transform playerHead;
    [SerializeField] private TextMeshProUGUI interactableText;
    private bool hasInteractedWithNPC = false;
    private bool audioPlayed = false;
    private ContinuousMoveProviderBase moveProvider;
    private float originalMoveSpeed;
    
    public GameObject xrOrigin; 
    public Transform targetPosition; 
    public float moveSpeed = 1.0f; 
    public float rotationSpeed = 1.0f;
    public float distanceThreshold = 0.1f; 
    
    public NarrationManager narrationManager;

    private bool isMoving = false;


    private void Start()
    {
        moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
        
        if (moveProvider != null)
        {
            originalMoveSpeed = moveProvider.moveSpeed;
        }
    }

    private void Update()
    {
        if (playerInteract.GetDialogueManager() != null && !hasInteractedWithNPC)
        {
            Show();
        }
        else
        {
            Hide();
        }
        
        if (isMoving)
        {
            MoveAndRotatePlayer();
        }
    }

    private void Show()
    {
        if (!audioPlayed)
        {
            narrationManager.PlayNextNarration();
            audioPlayed = true;
        }
        interactableText.text = "Interactuar con Gerardo";
        containerUI.SetActive(true);
        containerUI.transform.LookAt(playerHead);
        containerUI.transform.forward *= -1;
    }

    private void Hide()
    {
        containerUI.SetActive(false);
    }
    public void OnInteract()
    {
        hasInteractedWithNPC = true;
        Hide();
        StartMoveToNPC(); 
    }

    // Iniciar el movimiento hacia el NPC
    private void StartMoveToNPC()
    {
        isMoving = true;
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = 0f;
        }
    }
    
    private void MoveAndRotatePlayer()
    {
        // Mover el XR Origin hacia la posición objetivo
        Vector3 currentPosition = xrOrigin.transform.position;
        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition.position, moveSpeed * Time.deltaTime);
        xrOrigin.transform.position = newPosition;

        // Rotar hacia el NPC
        Quaternion currentRotation = xrOrigin.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition.forward);
        xrOrigin.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Detener el movimiento si se alcanza la posición objetivo
        if (Vector3.Distance(xrOrigin.transform.position, targetPosition.position) < distanceThreshold)
        {
            isMoving = false;
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = originalMoveSpeed;
            }
        }
    }
}
