using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteClickable : MonoBehaviour
{
    private GameObject spriteToMove;

    private ProgressiveBuild progressiveBuild;

    private void Awake()
    {
        spriteToMove = GameObject.FindGameObjectWithTag("Frog");
        progressiveBuild = FindObjectOfType<ProgressiveBuild>();
    }

    private void OnMouseDown()
    { 
        Movement movement = spriteToMove.GetComponent<Movement>();
        if (!movement.move)
        {
            progressiveBuild.DissapearOtherOptions(this.gameObject);
            movement.MoveToTarget(transform.position, transform.tag, false);
            movement.currentLilyPad = this.gameObject;

            // Calcula tiempo de reacción
            if (progressiveBuild.firstMoveProcessed)
            {
                float reactionTime = Time.time - progressiveBuild.lastOptionAppearTime;
                Debug.Log("Reaction time: " + reactionTime);
                if (gameObject.CompareTag("Thorns"))
                {
                    progressiveBuild.reactionTimeNogo = reactionTime;
                }
                else
                {
                    progressiveBuild.totalGoReactionTime += reactionTime;
                    progressiveBuild.goReactionCount++;
                }
            }
        }

    }
}
