using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class DialogueManager : MonoBehaviour
{
    public Action<bool> isTalking;

    [SerializeField] private Transform targetVRPosition;
    [SerializeField] private Transform NPCHead;
    [SerializeField] private Transform NPCLookAt;
    [SerializeField] private PlayerInteract2 playerInteract;

    private Vector3 NPCLookAtInitialPosition;
    private Animator animator;
    private bool isNPCTalking = false;

    public List<Dialogue> dialogues; // Lista de diálogos
    public TextMeshProUGUI npcText; // Componente de texto para mostrar el diálogo del NPC
    public GameObject questionPanel; // Panel de UI que contiene las preguntas y opciones
    public TextMeshProUGUI questionText; // Componente de texto para mostrar la pregunta
    public List<Button> optionButtons; // Lista de botones para las opciones de respuesta
    public TextMeshProUGUI feedbackText; // Texto para mostrar retroalimentación de respuestas incorrectas
    public AudioSource audioSource; // Componente de audio del NPC

    private int currentDialogueIndex = 0; // Índice del diálogo actual
    private int currentQuestionIndex = 0; // Índice de la pregunta actual

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        NPCLookAtInitialPosition = NPCLookAt.position;
        questionPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
    }

    void Update()
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
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        currentDialogueIndex = 0;
        currentQuestionIndex = 0;
        ShowDialogue(); // Inicia mostrando el primer diálogo
    }

    void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            npcText.gameObject.SetActive(true);
            // Muestra la línea de diálogo del NPC
            npcText.text = dialogues[currentDialogueIndex].npcLine;

            // Reproduce el audio del NPC y muestra la animación
            audioSource.clip = dialogues[currentDialogueIndex].audioClip;
            audioSource.Play();
            isNPCTalking = true; // NPC está hablando
            animator.SetBool("isTalking", true);
            isTalking?.Invoke(true);

            // Espera a que termine el audio antes de mostrar la pregunta
            StartCoroutine(WaitForAudio());
        }
        else
        {
            EndDialogue(); // Finaliza el diálogo si no hay más diálogos
        }
    }

    IEnumerator WaitForAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        ShowQuestion(); // Muestra la pregunta correspondiente
    }

    void ShowQuestion()
    {
        isNPCTalking = false; // NPC ha terminado de hablar
        npcText.gameObject.SetActive(false);
        if (currentQuestionIndex < dialogues[currentDialogueIndex].questions.Count)
        {
            // Obtiene la pregunta actual
            Question currentQuestion = dialogues[currentDialogueIndex].questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText; // Muestra el texto de la pregunta
            
            foreach (var button in optionButtons)
            {
                button.GetComponent<Button>().interactable = true;
                button.GetComponent<Button>().OnDeselect(null);
            }

            // Configura los botones para las opciones de respuesta
            for (int i = 0; i < optionButtons.Count; i++)
            {
                if (i < currentQuestion.options.Count)
                {
                    optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.options[i];
                    optionButtons[i].gameObject.SetActive(true);
                    int optionIndex = i;
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex));
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
            questionPanel.SetActive(true); // Muestra el panel de preguntas
        }
    }

    void OnOptionSelected(int index)
    {
        // Oculta el panel de preguntas
        questionPanel.SetActive(false);

        // Verifica si la respuesta es correcta o incorrecta
        Question currentQuestion = dialogues[currentDialogueIndex].questions[currentQuestionIndex];
        if (index == currentQuestion.correctOptionIndex)
        {
            // Respuesta correcta
            StartCoroutine(PlayFeedbackAudio(currentQuestion.positiveFeedbackAudio));
        }
        else
        {
            // Respuesta incorrecta, muestra retroalimentación
            feedbackText.text = $"Incorrecto. {currentQuestion.explanation}";
            feedbackText.gameObject.SetActive(true);
            StartCoroutine(PlayFeedbackAudio(currentQuestion.negativeFeedbackAudio, false));
        }
    }

    IEnumerator PlayFeedbackAudio(AudioClip feedbackAudioClip, bool isCorrect = true)
    {
        // Reproduce el audio de retroalimentación
        audioSource.clip = feedbackAudioClip;
        audioSource.Play();

        // Espera a que termine el audio antes de continuar
        yield return new WaitForSeconds(audioSource.clip.length);

        if (isCorrect)
        {
            // Pasa a la siguiente pregunta si la respuesta fue correcta
            currentQuestionIndex++;
            if (currentQuestionIndex >= dialogues[currentDialogueIndex].questions.Count)
            {
                currentQuestionIndex = 0;
                currentDialogueIndex++;
            }
            ShowDialogue(); // Muestra el siguiente diálogo o pregunta
        }
        else
        {
            HideFeedbackAndShowQuestion(); // Vuelve a mostrar la pregunta si la respuesta fue incorrecta
        }
    }

    void HideFeedbackAndShowQuestion()
    {
        feedbackText.gameObject.SetActive(false);
        ShowQuestion();
    }

    void EndDialogue()
    {
        // Aquí puedes manejar lo que sucede cuando el diálogo termina
        animator.SetBool("isTalking", false);
        NPCLookAt.position = NPCLookAtInitialPosition;
        isTalking?.Invoke(false);
        playerInteract.DeactivateCanvas();
        Debug.Log("El diálogo ha terminado.");
    }
}


[Serializable]
public class Question
{
    public string questionText;
    public List<string> options;
    public int correctOptionIndex;
    public string explanation; // Para feedback negativo
    public AudioClip positiveFeedbackAudio; // Audio para retroalimentación positiva
    public AudioClip negativeFeedbackAudio; // Audio para retroalimentación negativa
}

[Serializable]
public class Dialogue
{
    public string npcLine;
    public AudioClip audioClip;
    public List<Question> questions;
}

