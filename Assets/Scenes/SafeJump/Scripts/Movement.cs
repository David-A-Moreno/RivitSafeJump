using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Velocidad de movimiento del sprite
    public float moveSpeed = 5f;

    [SerializeField]
    private GameObject cameraRef;

    // Movimiento hacia la posición objetivo
    public void MoveToTarget(Vector3 targetPosition)
    {
        StartCoroutine(MoveTowards(targetPosition));
    }

    // Corutina para mover el sprite
    private IEnumerator MoveTowards(Vector3 targetPosition)
    {
        // Mientras la distancia entre el jugador y el objetivo sea mayor a un umbral
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Mover el sprite hacia la posición objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Mover la cámara solo en el eje Y
            if (cameraRef != null)
            {
                // Actualizar la posición de la cámara, solo en el eje Y
                Vector3 cameraPosition = cameraRef.transform.position;
                cameraPosition.y = Mathf.MoveTowards(cameraPosition.y, targetPosition.y, moveSpeed * Time.deltaTime);
                cameraRef.transform.position = cameraPosition;
            }

            yield return null; // Espera hasta el siguiente frame
        }

        // Aseguramos que el sprite llegue exactamente a la posición
        transform.position = targetPosition;

        // Aseguramos que la cámara también se quede en la altura correcta cuando se llega al objetivo
        if (cameraRef != null)
        {
            Vector3 cameraPosition = cameraRef.transform.position;
            cameraPosition.y = transform.position.y;
            cameraRef.transform.position = cameraPosition;
        }
    }
}
