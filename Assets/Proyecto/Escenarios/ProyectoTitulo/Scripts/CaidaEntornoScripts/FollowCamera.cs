using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform vrCamera; // La cámara VR
    private Vector3 offset = new Vector3(0, 0, 1); // Ajusta la distancia según sea necesario

    void Update()
    {
        // Actualiza la posición del objeto vacío para que siga a la cámara
        transform.position = vrCamera.position + vrCamera.forward * offset.z + vrCamera.up * offset.y;
        transform.rotation = Quaternion.LookRotation(transform.position - vrCamera.position);
    }
}