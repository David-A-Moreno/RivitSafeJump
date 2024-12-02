using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class TuberiaCounterComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pipelines;
    [Inject] private GameManager gameManager;

    void Start()
    {
    }

    void Update()
    {
        int goal = (gameManager as LevelSystemGameManager).Goal;
        int pipelinesCounter = (gameManager as LevelSystemGameManager).Pipelines;

        pipelines.text = "Tuberías: " + pipelinesCounter + "/" + goal;
    }
}
