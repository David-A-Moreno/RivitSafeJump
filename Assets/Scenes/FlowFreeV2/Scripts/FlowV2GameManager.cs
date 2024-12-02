using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace FlowFreeV2
{
    public class FlowV2GameManager : LevelSystemGameManager
    {
        private AdditionalDataFlowFreeV2 additionalData;
        [SerializeField] Timer timer;
        public override string Name => "FlowFreeV2";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;

        private bool gameOver;
        public override void EndGame()
        {
            timer.Started = false;

            if(IsQuitGame || !GameOver())
                Score = 0;
            else
            {
                double movesCalification = Math.Log10(Steps);
                double timeCalification = Math.Log10(timer.CurrentTime / 10);
                Score = ((int)((int)100 * (1 / (movesCalification + timeCalification))));
            }
            //FlowFreeV2UIManager._instance.ActivePanelControlsFlow(false);
        }

        public override void EndGameLevels()
        {
            GenerateBoard._instance.CreateBoardVoid();

            if(timer.Started)
            {
                IsQuitGame = true;
                EndGame();
                LeanTween.delayedCall(gameObject, 0.2F, () => NotifyGameOver());
            }
        }

        public override void StartGame()
        {
            gameOver = false;
            InputMouse._instance.Steps = 0;
            Score = 0;
            Steps = 0;
            Flows = 0;
            Goal = GenerateBoard._instance.CantLines;
            IsQuitGame = false;
            additionalData = new AdditionalDataFlowFreeV2();

            timer.CurrentTime = 0;
            timer.Started = true;
            timer.IsIncrementing = true;
        }

        private void Update()
        {
            Steps = InputMouse._instance.Steps;
            Flows = LineManager._instance.CountFowCompleted();

            if (LineManager._instance.pathLineList.Count > 0)
            {
                gameOver = GameOver();

                DegugGUIUpdates();


                if (gameOver)
                {
                    GenerateBoard._instance.CreateBoardVoid();
                    EndGame();
                    LeanTween.delayedCall(gameObject, 0.2F, () => NotifyGameOver());
                }
                //FlowFreeV2UIManager._instance.ActivePanelGameOver(gameOver);
                
                //AudioManagerFlowV2._instance.PlayAudio("Completed");
            }
        }

        void DegugGUIUpdates()
        {
            DebugGUI._instance.flowCountGUI = "Flujos completos = " + LineManager._instance.CountFowCompleted();
            DebugGUI._instance.leveCompleted = "Nivel superado = " + GameOver();
        }

        public bool GameOver()
        {
            int cantFlowCompleted = LineManager._instance.CountFowCompleted();
            int cantFlowTotal = GenerateBoard._instance.CantLines;

            if (cantFlowCompleted == cantFlowTotal) return true;
            else return false;
        }

        public override string RegisterAdditionalData()
        {
            additionalData.RightAnswers = Steps;
            additionalData.AverageTime = timer.CurrentTime;
            return JsonConvert.SerializeObject(additionalData);
        }
    }
}

