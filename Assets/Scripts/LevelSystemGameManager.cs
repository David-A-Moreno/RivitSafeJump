using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class LevelSystemGameManager : GameManager
{
    [SerializeField] private LevelsHandler levelsHandler;

    public virtual int Steps
    {
        get { return _steps; }
        set{ _steps = value; }
    }

    public virtual int Flows
    {
        get { return _flows; }
        set{ _flows = value; }
    }

    public virtual int Pipelines
    {
        get { return _pipelines; }
        set{ _pipelines = value; }
    }

    public virtual int Goal
    {
        get { return _goal; }
        set{ _goal = value; }
    }

    public virtual int Score
    {
        get { return _score; }
        set { _score = value; }
    }

    public virtual bool IsQuitGame
    {
        get { return _isQuitGame; }
        set { _isQuitGame = value; }
    }

    public int NumberOfLevels { get => levelsHandler.NumberOfLevels; }
    
    public void InitializeGame(int level)
    {
        levelsHandler.GenerateLevel(level);
        RecordStartTimeAndStartGame("Nivel" + level);
        Score = 0;
    }

    public override void RestartGame()
    {
        EndGame();
        InitializeGame(levelsHandler.CurrentLevel);
        StartGame();
    }

    public void ToNextLevel()
    {
        levelsHandler.ToNextLevel();
    }

    public void ToPreviousLevel()
    {
        levelsHandler.ToPreviousLevel();
    }

    public string LoadRankingData()
    {
        return _userDataManager.GetTopScoresOfGame(Name);
    }

    public List<List<int>> GetRankingOfEveryMode()
    {
        return JsonConversor.ConvertJsonToRanking(LoadRankingData());
    }

    public List<int> GetCurrentRanking()
    {
        return JsonConversor.ConvertJsonToRanking(LoadRankingData())[0];
    }

    public int[] GetStandardsOfCurrentLevel()
    {
        var standards = new int[3] {15, 50, 100};
        return standards;
    }

    public int AddCoins()
    {
        var standards = GetStandardsOfCurrentLevel();
        int coins = 0;


        if (_score >= standards[2])
            coins = 25;
        else if (_score >= standards[1])
            coins = 10;
        else if (_score >= standards[0])
            coins = 3;
        else if (_score >= 1)
            coins = 1;

        Coins = coins;
        return coins;
    }

    public void RecordScore()
    {
        string highScores = RankingManager.RecordScore(LoadRankingData(), 0, Score);
        _userDataManager.UpdateTopScoresOfGame(Name, highScores);
    }
}
