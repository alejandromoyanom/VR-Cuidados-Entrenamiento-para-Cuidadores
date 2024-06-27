using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public List<Dialogue> dialogues; // Lista de diálogos
    public TextMeshProUGUI npcText; // Componente de texto para mostrar el diálogo del NPC
    public GameObject questionPanel; // Panel de UI que contiene las preguntas y opciones
    public TextMeshProUGUI questionText; // Componente de texto para mostrar la pregunta
    public List<Button> optionButtons; // Lista de botones para las opciones de respuesta
    public TextMeshProUGUI feedbackText; // Texto para mostrar retroalimentación de respuestas incorrectas
    public Animator npcAnimator; // Componente de animación del NPC
    public AudioSource audioSource; // Componente de audio del NPC

    private int currentDialogueIndex = 0; // Índice del diálogo actual
    private int currentQuestionIndex = 0; // Índice de la pregunta actual

    void Start()
    {
        ShowDialogue(); // Inicia mostrando el primer diálogo
    }

    void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            // Muestra la línea de diálogo del NPC
            npcText.text = dialogues[currentDialogueIndex].npcLine;
            
            // Reproduce el audio del NPC y muestra la animación
            audioSource.clip = dialogues[currentDialogueIndex].audioClip;
            audioSource.Play();
            npcAnimator.SetTrigger("Talk"); // Asegúrate de tener un trigger llamado "Talk" en el Animator

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
        if (currentQuestionIndex < dialogues[currentDialogueIndex].questions.Count)
        {
            // Obtiene la pregunta actual
            Question currentQuestion = dialogues[currentDialogueIndex].questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText; // Muestra el texto de la pregunta

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
            currentQuestionIndex++; // Pasa a la siguiente pregunta
            if (currentQuestionIndex >= dialogues[currentDialogueIndex].questions.Count)
            {
                currentQuestionIndex = 0;
                currentDialogueIndex++;
            }
            ShowDialogue(); // Muestra el siguiente diálogo o pregunta
        }
        else
        {
            // Respuesta incorrecta, muestra retroalimentación
            feedbackText.text = $"Incorrecto. La respuesta correcta es: {currentQuestion.options[currentQuestion.correctOptionIndex]}. {currentQuestion.explanation}";
            feedbackText.gameObject.SetActive(true);
            Invoke("HideFeedbackAndShowQuestion", 5f); // Muestra la retroalimentación durante 5 segundos
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
        Debug.Log("El diálogo ha terminado.");
        // Puedes ocultar el UI del diálogo o realizar otras acciones
    }
}
