using System.Collections.Generic;
using UnityEngine;

public class CleaningSequenceManager : MonoBehaviour
{
    public List<CleaningTool> toolsInSequence;
    private int currentToolIndex = 0;

    void Start()
    {
        // Activar solo la primera herramienta
        SetActiveTool(0);
    }

    public void ToolCompleted(CleaningTool tool)
    {
        // Desactivar la herramienta actual
        tool.SetInteractable(false);

        currentToolIndex++;

        if (currentToolIndex < toolsInSequence.Count)
        {
            // Activar la siguiente herramienta
            SetActiveTool(currentToolIndex);
        }
        else
        {
            Debug.Log("Limpieza completa.");
        }
    }

    private void SetActiveTool(int index)
    {
        // Activar la herramienta en el índice dado
        toolsInSequence[index].SetInteractable(true);
    }
}