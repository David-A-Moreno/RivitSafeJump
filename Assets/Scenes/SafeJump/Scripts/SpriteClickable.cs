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
        progressiveBuild.DissapearOtherOptions(this.gameObject);
        spriteToMove.GetComponent<Movement>().MoveToTarget(transform.position, transform.tag);
    }
}
