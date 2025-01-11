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
            movement.MoveToTarget(transform.position, transform.tag);
        }
        
    }
}
