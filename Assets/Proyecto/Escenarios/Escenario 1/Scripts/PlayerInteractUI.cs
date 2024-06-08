using System;
using UnityEngine;
using TMPro;

public class PlayerInteractUI : MonoBehaviour
{
    public PlayVoice playVoice;
    [SerializeField] private GameObject containerUI;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Transform playerHead;
    [SerializeField] private TextMeshProUGUI interactableText;
    private bool isAudioPlaying = false;
    private bool hasInteractedWithNPC = false;

    private void Start() {
        playVoice.clipStarted += HideUiOnAudioPlay;
        playVoice.clipFinished += ShowUiOnAudioEnd;
    }

    private void Update() {
        if (playerInteract.GetInteractableObject() != null && !isAudioPlaying && !playVoice.IsLastClip())
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
        if (hasInteractedWithNPC)
        {
            interactableText.text = "Volver a interactuar";
        }
        containerUI.SetActive(true);
        containerUI.transform.LookAt(playerHead);
        containerUI.transform.forward *= -1;
    }

    private void Hide()
    {
        containerUI.SetActive(false);
    }

    private void HideUiOnAudioPlay()
    {
        isAudioPlaying = true;
        hasInteractedWithNPC = true;
    }

    private void ShowUiOnAudioEnd()
    {
        isAudioPlaying = false;
        if (playVoice.IsLastClip())
        {
            Hide();
        }
    }
}