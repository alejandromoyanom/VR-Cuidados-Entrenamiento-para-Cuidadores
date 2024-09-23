using UnityEngine;

public class ShoeManager : MonoBehaviour
{
    public GameObject toolsGameObject; // El GameObject que contiene las herramientas
    public NarrationManager narrationManager;
    
    private int shoesPlaced = 0; // Contador de zapatos colocados

    void Start()
    {
        toolsGameObject.SetActive(false);
    }

    // Llamado cuando un zapato es colocado correctamente
    public void ShoePlaced()
    {
        shoesPlaced++;

        // Si ambos zapatos están colocados, activar las herramientas
        if (shoesPlaced >= 2)
        {
            toolsGameObject.SetActive(true);
            narrationManager.QueueNarration();
            
        }
    }
}