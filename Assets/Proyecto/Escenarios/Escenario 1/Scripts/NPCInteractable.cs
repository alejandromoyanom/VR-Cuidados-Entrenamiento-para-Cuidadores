using System;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public PlayVoice playVoice;
    public Action<bool> isTalking;

    [SerializeField] private Transform targetVRPosition;
    [SerializeField] private Transform NPCHead;
    [SerializeField] private Transform NPCLookAt;

    private Vector3 NPCLookAtInitialPosition;
    private Animator animator;
    private bool isNPCTalking = false;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        playVoice.clipStarted += NPCTalking;
        playVoice.clipFinished += NPCTalkingEnd;
        NPCLookAtInitialPosition = NPCLookAt.position;
    }

    private void Update()
    {
       if (isNPCTalking)
       {
           NPCLookAt.position = targetVRPosition.position;
       }
    }

    public void Interact()
    {
        if (!isNPCTalking)
        {
            playVoice.PlaySound();
            animator.SetBool("isTalking", true);
            isTalking?.Invoke(true);
        }
    }

    private void ReturnToIdle()
    {
        animator.SetBool("isTalking", false);
        NPCLookAt.position = NPCLookAtInitialPosition;
    }

    private void NPCTalking()
    {
        isNPCTalking = true;
    }

    private void NPCTalkingEnd()
    {
        isNPCTalking = false;
        ReturnToIdle();
        isTalking?.Invoke(false);
    }
}
