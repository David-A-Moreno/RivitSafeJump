using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Velocidad de movimiento del sprite
    public int moveSpeed = 4;

    public bool move { get; set; } = false;

    [SerializeField]
    private GameObject cameraRef;

    [SerializeField]
    private ProgressiveBuild progressiveBuild;

    [SerializeField]
    private SafeJumpManager safeJumpManager;

    [SerializeField]
    private AudioFX audioFX;

    [SerializeField]
    private AudioSource music;

    // Movimiento hacia la posición objetivo
    public void MoveToTarget(Vector3 targetPosition, string targetTag)
    {
        StartCoroutine(MoveTowards(targetPosition, targetTag));
    }

    // Corutina para mover el sprite
    private IEnumerator MoveTowards(Vector3 targetPosition, string targetTag)
    {
        move = true;
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
                cameraPosition.y = Mathf.MoveTowards(cameraPosition.y, targetPosition.y+3f, moveSpeed * Time.deltaTime);
                cameraRef.transform.position = cameraPosition;
            }

            yield return null; // Espera hasta el siguiente frame
        }
        move = false;
        if (targetTag == "Thorns")
        {
            music.Stop();
            audioFX.PlaySound(4);
            
            safeJumpManager.FinishGame();
            progressiveBuild.gameOver = true;
            progressiveBuild.setLostLevel();
            progressiveBuild.SaveAdditionalData();

        }
        else if (targetTag == "Bonus1" || targetTag == "Bonus2" || targetTag == "Bonus3")
        {
            audioFX.PlaySound(1);
            if (targetTag == "Bonus1")
            {
                safeJumpManager.Score += 7;
            }
            else if (targetTag == "Bonus2")
            {
                safeJumpManager.Score += 4;
            }
            else if (targetTag == "Bonus3")
            {
                safeJumpManager.Score += 2;
            }
        }
        

        progressiveBuild.OneStep();
        progressiveBuild.AutomaticMoveTimer();

        // Aseguramos que el sprite llegue exactamente a la posición
        /*if (targetPosition.y < 0)
        {
            transform.position = targetPosition;
            Vector3 newPositionCamera = cameraRef.transform.position;
            newPositionCamera.y = transform.position.y - 0.5f;
            cameraRef.transform.position = newPositionCamera;
        }
        */
        /*
        // Aseguramos que la cámara también se quede en la altura correcta cuando se llega al objetivo
        if (cameraRef != null && targetPosition.y > 0)
        {
            Vector3 cameraPosition = cameraRef.transform.position;
            cameraPosition.y = transform.position.y;
            cameraRef.transform.position = cameraPosition;
        }*/
    }

    public void SetSpeed(int speed)
    {
        moveSpeed = speed;
    }

    public bool GetMove()
    {
        return move;
    }

    public void SetMove(bool shouldMove)
    {
        move = shouldMove;
        //waitForObjectDestruction = false;
        //forestManagerReference.GetComponent<ProgressiveBuild>().OneStep();
    }


}
