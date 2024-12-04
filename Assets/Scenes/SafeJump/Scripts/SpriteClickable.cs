using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteClickable : MonoBehaviour
{
    private GameObject spriteToMove;
    private void Awake()
    {
        spriteToMove = GameObject.FindGameObjectWithTag("Frog");
    }

    private void OnMouseDown()
    {
        spriteToMove.GetComponent<Movement>().MoveToTarget(transform.position);
    }
}
