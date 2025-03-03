using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SafeJumpAdditionalData
{
    // Start is called before the first frame update
    public int nogoOptionsAvoided { get; set; }
    public int goOptionStreak { get; set; }
    public int lostLevel { get; set; }
}
