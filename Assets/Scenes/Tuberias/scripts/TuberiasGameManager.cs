using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using Tuberias;
using Newtonsoft.Json;

using Debug = UnityEngine.Debug;

namespace Tuberias
{
    public class TuberiasGameManager : LevelSystemGameManager
    {
        public override string Name => "Tuberias";
        [InjectOptional(Id = "SFXManager")] private AudioManager SFXManager;
        [SerializeField] Timer timer;
        private bool gameOver;
        public int levelIndexList;
        public List<int> listaVer;
        public GameObject[] listaObjetos;
        public List<TextAsset> levelsList2;
        public GameObject boton;
        public int clicks;
        private int movements;

        private TuberiasAdditionalData additionalData;

        public override void EndGame()
        {
            timer.Started = false;

            if(IsQuitGame)
                Score = 0;
            else
            {
                double movesCalification = Math.Log10(Steps);
                double timeCalification = Math.Log10(timer.CurrentTime / 10);
                Score = ((int)(50 * (1 / (Steps + (timer.CurrentTime / 10)))));
            }

            manejadorTablero.instance.angulos.Clear();
            //TuberiasUIManager.instance.ActivePanelControlsTuberias(false);
        }

        public override void EndGameLevels()
        {
            manejadorTablero.instance.CrearTableroVacio();

            if(timer.Started)
            {
                IsQuitGame = true;
                EndGame();

                listaVer.Clear();
                LeanTween.delayedCall(gameObject, 0.2F, () => NotifyGameOver());
            }
        }

        public override void StartGame()
        {
            additionalData = new TuberiasAdditionalData();
            Debug.Log("Metodo Start: Tuberias");
            gameOver = false;
            levelIndexList = 0;
            Pipelines = 0;
            Steps = 0;
            IsQuitGame = false;
            Goal = manejadorTablero.instance.Cols*manejadorTablero.instance.Cols-2;
            
            timer.CurrentTime = 0;
            timer.Started = true;
            timer.IsIncrementing = true;
            //TuberiasUIManager.instance.ActivePanelGameOver(gameOver);
            //TuberiasUIManager.instance.DesactiveBoard(!gameOver);
        }

        private void Update()
        {
            Verificacion();
            Pipelines = listaVer.Count;
                
            if(gameOver)
            {
                manejadorTablero.instance.CrearTableroVacio();
                //TuberiasUIManager.instance.ActivePanelGameOver(gameOver);
                manejadorTablero.instance.angulos.Clear();
                EndGame();
                LeanTween.delayedCall(gameObject, 0.2F, () => NotifyGameOver());
                //TuberiasUIManager.instance.DesactiveBoard(!gameOver);
                gameOver = false;
            }
        }

        void DegugGUIUpdates()
        {
            DebugGUITuberias.instance.movimientos = "Movimientos = " + clicks;
            DebugGUITuberias.instance.levelCompleted = "Nivel superado = " + gameOver;
        }


        private void Verificacion()
        {
            listaObjetos = GameObject.FindGameObjectsWithTag("boton");
            if (listaObjetos.Length > 0)
            {   
                for (int i = 0; i < listaObjetos.Length; i++)
                {
                    boton = listaObjetos[i];
                    manejadorTablero.instance.LlenarLista(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]), listaVer, i, boton);

                    if(i == 0)
                        Steps = 0;

                    Steps += boton.GetComponent<Giro>().clicks;
                }

                if (listaVer.Count == manejadorTablero.instance.Cols*manejadorTablero.instance.Cols-2)
                {
                    movements = Steps;
                    gameOver = true;
                }
            }
        }

        public override string RegisterAdditionalData()
        {
            additionalData.RightAnswers = movements;
            additionalData.AverageTime = timer.CurrentTime;
            return JsonConvert.SerializeObject(additionalData);
        }
    }
}