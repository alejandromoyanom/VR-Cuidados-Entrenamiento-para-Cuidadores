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


    // Variables públicas para el XR Origin y la posición objetivo
    public GameObject xrOrigin; // El XR Origin que se moverá
    public Transform targetPosition; // La posición frente al NPC
    public float moveSpeed = 1.0f; // Velocidad de movimiento
    public float rotationSpeed = 1.0f; // Velocidad de rotación
    public float distanceThreshold = 0.1f; // Umbral de distancia para considerar que ha llegado al destino
    
    public NarrationManager narrationManager;

    private bool isMoving = false; // Control del movimiento en progreso


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

        // Si está en movimiento, mover y rotar el XR Origin
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

    // Método llamado cuando el jugador interactúa con el NPC
    public void OnInteract()
    {
        hasInteractedWithNPC = true;
        Hide();
        StartMoveToNPC(); // Iniciar el movimiento al interactuar
    }

    // Iniciar el movimiento hacia el NPC
    private void StartMoveToNPC()
    {
        isMoving = true; // Activar la bandera de movimiento
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = 0f;
        }
    }

    // Método para mover y rotar suavemente el XR Origin
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
