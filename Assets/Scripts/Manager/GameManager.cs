using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// Define a set of game modes for different game objectives
public enum GameMode { FeedingObjective, ScoringObjective, TimeObjective, CollectionObjective }
public enum GamePlayMode { MovesLimited, TimedMatch }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    /// Observer Pattern
    [HideInInspector] public UnityEvent OnFeedingObjective;
    [HideInInspector] public UnityEvent OnScoringObjective;
    [HideInInspector] public UnityEvent OnTimeObjective;
    [HideInInspector] public UnityEvent OnCollectionObjective;

    public int Level { get { return level; } }  // Public getter for the current level
    // Gets or sets the objective game mode.
    public GameMode GameMode { get { return gameMode; } set { gameMode = value; } }
    // Gets the list of available fruits.
    public List<GameObject> AvailableFruits { get { return availableFruits; } }

    // Serialized game mode field
    [SerializeField] GameMode gameMode;

    [SerializeField] int level = 0; // Private field for the current level
    [SerializeField] List<GameObject> availableFruits;
    [SerializeField] List<GameObject> upcomingFruits;

    /// <summary>
    /// Subscribes to the SceneManager's sceneLoaded event when the script is enabled.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the SceneManager's sceneLoaded event when the script is disabled.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Increases the level count and starts the next level routine.
    /// </summary>
    public void NextLevel()
    {
        // Increases the level count
        level++;
        // Starts the next level routine
        StartCoroutine(NextLevelRutiner());
    }

    /// <summary>
    /// Waits for 2 seconds and finds the LevelManager object.
    /// </summary>
    IEnumerator NextLevelRutiner()
    {
        yield return new WaitForSeconds(2);

        // Finds the LevelManager object
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.NextLevel();
    }

    /// <summary>
    /// Called when a scene is loaded, checks if the scene is "Game" and sets the objective game mode.
    /// </summary>
    /// <param name="scene">The scene that was loaded</param>
    /// <param name="mode">The mode used to load the scene</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SetObjectiveGameMode(gameMode);
        }
    }

    /// <summary>
    /// Sets the objective game mode and updates the game play mode if the mode is "FeedingObjective".
    /// </summary>
    /// <param name="gameMode">The objective game mode</param>
    void SetObjectiveGameMode(GameMode gameMode)
    {
        if (gameMode == GameMode.FeedingObjective)
        {
            OnFeedingObjective?.Invoke();

        }
        else if (gameMode == GameMode.ScoringObjective)
        {

        }
        else if (gameMode == GameMode.TimeObjective)
        {

        }
        else if (gameMode == GameMode.CollectionObjective)
        {

        }
    }

    /// <summary>
    /// Gets a random game play mode from the GamePlayMode enum.
    /// </summary>
    /// <returns>A random game play mode</returns>
    public GamePlayMode GetRandomGamePlayMode()
    {
        int numberOfGamePlayMode = Enum.GetNames(typeof(GamePlayMode)).Length;

        return (GamePlayMode)UnityEngine.Random.Range(0, numberOfGamePlayMode);
    }
}