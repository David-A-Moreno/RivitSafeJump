using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class StepsComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI steps;
    [Inject] private GameManager gameManager;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        steps.text = "Pasos: " + (gameManager as LevelSystemGameManager).Steps;
    }
}
