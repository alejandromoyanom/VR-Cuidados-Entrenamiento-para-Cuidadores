using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string npcLine; // Línea de diálogo del NPC
    public AudioClip audioClip; // Clip de audio del diálogo
    public List<Question> questions; // Lista de preguntas asociadas a esta línea de diálogo
}