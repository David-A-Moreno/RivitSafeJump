using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class FlowCounterComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flows;
    [Inject] private GameManager gameManager;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int goal = (gameManager as LevelSystemGameManager).Goal;
        int flowCounter = (gameManager as LevelSystemGameManager).Flows;

        flows.text = "Flujos: " + flowCounter + "/" + goal;
    }
}
