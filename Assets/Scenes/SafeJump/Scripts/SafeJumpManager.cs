using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeJumpManager : ModeSystemGameManager
{
    private int score = 5;

    [SerializeField]
    private ProgressiveBuild progressiveBuild;

    public override string Name => "SafeJump";  // Nombre del juego

    public SafeJumpAdditionalData AdditionalData { get => additionalData; set => additionalData = value; }

    private SafeJumpAdditionalData additionalData;

    public override int Score
    {
        get => score;
        set
        {
            score = value;
            // Llamar a IncreaseDifficulty cuando el puntaje cambie
            //IncreaseDifficulty();
        }
    }

    public override void StartGame()
    {
        // Lógica para iniciar el juego
        //InitializeGame(_gameMode);
        progressiveBuild.StartGame();
        additionalData = new SafeJumpAdditionalData();
    }

    public override string RegisterAdditionalData()
    {
        Debug.Log("Datos adicionales:" +additionalData);
        return JsonConvert.SerializeObject(additionalData);
    }

    public override void EndGame()
    {
        
    }

    public void FinishGame()
    {
        // Lógica para finalizar el juego
        LeanTween.delayedCall(gameObject, 0.5F, () => NotifyGameOver());
    }

    public override void RestartGame()
    {
        // Reinicia el juego
        Debug.Log("Reiniciando el juego...");
        Score = 0;
        //StartGame();
        RestartCurrentScene();
    }

    public void RestartCurrentScene()
    {
        // Obtiene el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reinicia la escena actual cargándola de nuevo
        SceneManager.LoadScene(currentSceneName);
    }

    public override void IncreaseDifficulty()
    {
        // Lógica de aumento de dificultad
        base.IncreaseDifficulty();
        // Podrías también actualizar otras partes del juego según la dificultad
    }

    public override void EndGameLevels()
    {
    }
}
