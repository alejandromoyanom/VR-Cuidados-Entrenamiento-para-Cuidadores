using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText; // Texto de la pregunta
    public List<string> options; // Opciones de respuesta
    public int correctOptionIndex; // Índice de la opción correcta
    public string explanation; // Explicación de por qué la respuesta correcta es la correcta
}