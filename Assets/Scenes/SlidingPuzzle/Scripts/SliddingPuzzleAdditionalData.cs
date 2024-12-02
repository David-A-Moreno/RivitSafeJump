using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    [System.Serializable]
    public class SliddingPuzzleAdditionalData
    {
        public int RightAnswers { get; set; } //RightAnswers represents Moves
        public float AverageTime { get; set; }
    }
}