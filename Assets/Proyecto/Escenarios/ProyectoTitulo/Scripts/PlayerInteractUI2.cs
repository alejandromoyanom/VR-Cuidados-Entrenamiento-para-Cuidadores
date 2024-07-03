using System;
using UnityEngine;
using TMPro;

public class PlayerInteractUI2 : MonoBehaviour
{
    [SerializeField] private GameObject containerUI;
    [SerializeField] private PlayerInteract2 playerInteract;
    [SerializeField] private Transform playerHead;
    [SerializeField] private TextMeshProUGUI interactableText;
    private bool hasInteractedWithNPC = false;

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
    }

    private void Show()
    {
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
    }
}