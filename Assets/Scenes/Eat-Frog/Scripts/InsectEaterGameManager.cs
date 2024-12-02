using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Eat_frog_Game
{
    public class InsectEaterGameManager : ModeSystemGameManager
    {

        public bool Active { get; set; }
        public bool limitreached, paused;
        public bool GameOver { get; set; }
        private FrogController frog;
        private AdditionalData additionalData;

        public override string Name => "InsectEater";

        void Awake()
        {
            frog = FindObjectOfType<FrogController>();
        }


        public override void StartGame()
        {
            Active = true;
            frog.Velocity = 0.1F;
            frog.curhealth = 100;
            additionalData = new AdditionalData();
        }

        public override void EndGame()
        {
        }

        public override string RegisterAdditionalData()
        {
            additionalData.Score = this.Score;
            return JsonConvert.SerializeObject(additionalData);
        }

        public override void EndGameLevels()
        {

        }
    }
}