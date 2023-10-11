using UnityEngine;

public static class DifficultManager
{
    /// <summary>
    /// Generates a feeding objective for the game.
    /// </summary>
    public static void FeedingObjective()
    {
        LevelData levelData = GetLevelData();
        GameManager.Instance.MaxFeedingObjective = Random.Range(levelData.feedingData.minFeeding, levelData.feedingData.maxFeeding + 1);
    }

    /// <summary>
    /// Generates a scoring objective for the game and sets scoring probability.
    /// </summary>
    public static void ScoringObjective()
    {
        LevelData levelData = GetLevelData();
        GameManager.Instance.MaxScoreObjective = Random.Range(levelData.scoringData.minScoring, levelData.scoringData.maxScoring + 1);
        GameManager.Instance.ProbabilityMultiplicationFactor = levelData.scoringData.scoringProbability;
    }

    /// <summary>
    /// Generates a time-based objective for the game.
    /// </summary>
    public static void TimeObjective()
    {
        LevelData levelData = GetLevelData();
        GameManager.Instance.MatchObjectiveAmount = Random.Range(levelData.timeData.minTime, levelData.timeData.maxTime + 1);
        GameManager.Instance.TotalSeconds = levelData.timeData.time;
    }

    /// <summary>
    /// Generates a collection objective for the game and sets collection probability.
    /// </summary>
    public static void CollectionObjective()
    {
        LevelData levelData = GetLevelData();
        GameManager.Instance.FruitCollectionAmount = Random.Range(levelData.collectionData.minCollection, levelData.collectionData.maxCollection + 1);
        GameManager.Instance.FruitCollectionProbability = levelData.collectionData.collectionProbability;
    }

    /// <summary>
    /// Retrieves level data based on the current game difficulty.
    /// </summary>
    /// <returns>The level data for the current game difficulty.</returns>
    public static LevelData GetLevelData() => GameManager.Instance.DifficultData.LevelData[GameManager.Instance.Difficulty - 1];

    /// <summary>
    /// Sets the initial state data for the game based on the provided level data.
    /// </summary>
    /// <param name="levelData">The level data to set initial game state.</param>
    public static void SetInitialStateData(LevelData levelData)
    {
        GameManager.Instance.MoveCounter = Random.Range(levelData.minMoves, levelData.maxMoves + 1);
        GameManager.Instance.TimeToMatch = Random.Range(levelData.minTime, levelData.maxTime + 1);
        GameManager.Instance.TimeToMatchPenalty = levelData.penalty;
        GameManager.Instance.ScoreBar = Random.Range(levelData.minScoreBar, levelData.maxScoreBar + 1);
    }
}