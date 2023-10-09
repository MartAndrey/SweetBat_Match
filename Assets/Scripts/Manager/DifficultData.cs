using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the data for each level in the game.
/// </summary>
[System.Serializable]
public class LevelData
{
    public int minMoves;
    public int maxMoves;
    public int minTime;
    public int maxTime;
    public float penalty;

    public FeedingData feedingData;
    public ScoringData scoringData;
    public TimeData timeData;
    public CollectionData collectionData;
}

/// <summary>
/// Represents data related to feeding objectives.
/// </summary>
[System.Serializable]
public class FeedingData
{
    public int minFeeding;
    public int maxFeeding;
}

/// <summary>
/// Represents data related to scoring objectives.
/// </summary>
[System.Serializable]
public class ScoringData
{
    public int minScoring;
    public int maxScoring;
    public float scoringProbability;
}

/// <summary>
/// Represents data related to time-based objectives.
/// </summary>
[System.Serializable]
public class TimeData
{
    public int minTime;
    public int maxTime;
    public int time;
}

/// <summary>
/// Represents data related to collection objectives.
/// </summary>
[System.Serializable]
public class CollectionData
{
    public int minCollection;
    public int maxCollection;
    public float collectionProbability;
}

/// <summary>
/// Scriptable Object containing level data for different game difficulties.
/// </summary>
[CreateAssetMenu(fileName = "New DifficultData", menuName = "DifficultData")]
public class DifficultData : ScriptableObject
{
    public List<LevelData> LevelData { get { return levelData; } }

    [SerializeField] List<LevelData> levelData;
}