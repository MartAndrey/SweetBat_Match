using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.ComponentModel;

// Define a set of game modes for different game objectives
public enum GameMode
{
    [Description("Feed the bat!")]
    FeedingObjective,
    [Description("Beat the score!")]
    ScoringObjective,
    [Description("Pick the fruits before the time runs out!")]
    TimeObjective,
    [Description("Collect all orders!")]
    CollectionObjective
}
public enum GamePlayMode { MovesLimited, TimedMatch }
/// <summary>
/// Enum representing different error types.
/// </summary>
public enum Errors
{
    [Description("Something went wrong with the database dependencies. Please try again. If the problem persists, contact the game developer")]
    F_FA_68,
    [Description("Something went wrong with authentication. If the problem persists, contact the game developer.")]
    AUGGC_FA_122,
    [Description("Something went wrong with authentication. If the problem persists, contact the game developer.")]
    AUGGF_FA_127,
    [Description("Something went wrong when downloading the avatar image. If the problem persists, contact the game developer.")]
    UP_FA_199,
    [Description("Something went wrong with authentication. If the problem persists, contact the game developer.")]
    AUGG_GA_71,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    CNU_CF_70,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    GUD_CF_103,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    /// Observer Pattern
    [HideInInspector] public UnityEvent<GameMode> OnGameMode;
    [HideInInspector] public UnityEvent OnUniqueMatches;
    [HideInInspector] public UnityEvent<Errors> OnErrorRetry;
    [HideInInspector] public UnityEvent<Errors> OnErrorClose;

    public int Level { get { return level; } }  // Public getter for the current level
    // Gets or sets the objective game mode.
    public GameMode GameMode { get { return gameMode; } set { gameMode = value; } }
    // Gets the list of available fruits.
    public List<GameObject> AvailableFruits { get { return availableFruits; } }

    // Gets the maximum feeding objective.
    public int MaxFeedingObjective { get { return maxFeedingObjective; } }
    // Gets the maximum score objective.
    public int MaxScoreObjective { get { return maxScoreObjective; } }

    // Gets the current move counter.
    public int MoveCounter { get { return moveCounter; } }
    // Gets the time to match.
    public float TimeToMatch { get { return timeToMatch; } }

    // Gets or sets whether the game objective is complete.
    public bool ObjectiveComplete { get { return objectiveComplete; } set { objectiveComplete = value; } }

    // Gets the total remaining seconds for the timer.
    public float TotalSeconds { get { return totalSeconds; } }
    // Gets the match objective amount.
    public int MatchObjectiveAmount { get { return matchObjectiveAmount; } }
    public int FruitCollectionAmount { get { return fruitCollectionAmount; } }
    // Gets or sets a value indicating whether unique matches are required.
    public bool UniqueMatches { get { return uniqueMatches; } set { uniqueMatches = value; } }

    public Dictionary<string, object> UserData { get { return userData; } set { userData = value; } }
    public Sprite UserPhoto { get { return userPhoto; } set { userPhoto = value; } }

    // Serialized game mode field
    [SerializeField] GameMode gameMode;

    // Private field for the current level
    [SerializeField] int level = 0;
    // List of available fruits
    [SerializeField] List<GameObject> availableFruits;
    // List of upcoming fruits
    [SerializeField] List<GameObject> upcomingFruits;

    [Header("Game Mode")]
    // Move counter for the game mode
    [SerializeField] int moveCounter;
    // Time to match for the game mode
    [SerializeField] float timeToMatch;

    [Header("Feeding Objective")]
    // Maximum feeding objective
    [SerializeField] int maxFeedingObjective;

    [Header("Scoring Objective")]
    // Maximum score objective
    [SerializeField] int maxScoreObjective;

    [Header("Time Objective")]
    [SerializeField] float totalSeconds;
    [SerializeField] int matchObjectiveAmount;

    [Header("Collection Objective")]
    [SerializeField] int fruitCollectionAmount;

    // Indicates whether the game objective is complete
    bool objectiveComplete;

    bool uniqueMatches;

    // Dictionary containing user data.
    Dictionary<string, object> userData;
    // Sprite representing the user's photo.
    Sprite userPhoto;

    ErrorHandler errorHandler;

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

        if (gameMode == GameMode.ScoringObjective || gameMode == GameMode.TimeObjective)
            uniqueMatches = true;
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
        errorHandler = GameObject.FindObjectOfType<ErrorHandler>(true);

        if (scene.name == "Game")
            OnGameMode?.Invoke(gameMode);
        else if (scene.name == "LevelMenu")
            UpdateAvatars();
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

    /// <summary>
    /// Coroutine for loading the game.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator LoadingGameRutiner()
    {
        while ((userPhoto == null || userData == null))
        {
            yield return null;
        }
    }

    /// <summary>
    /// Updates the avatars for all players.
    /// </summary>
    public void UpdateAvatars()
    {
        StartCoroutine(UpdateAvatarsRutiner());
    }

    /// <summary>
    /// Coroutine for updating avatars.
    /// </summary>
    /// <returns>IEnumerator object.</returns>
    IEnumerator UpdateAvatarsRutiner()
    {
        yield return StartCoroutine(LoadingGameRutiner());

        // Find all AvatarController objects in the scene
        AvatarController[] avatars = Resources.FindObjectsOfTypeAll<AvatarController>();

        // Update the avatars with the user's gender and photo
        for (int i = 0; i < avatars.Length; i++)
        {
            avatars[i].UpdateAvatar((GenderUser)Enum.Parse(typeof(GenderUser), userData["gender"].ToString()), userPhoto);
        }
    }

    /// <summary>
    /// Start the coroutine for displaying the error UI.
    /// </summary>
    /// <param name="error">The error to display.</param>
    public void HasFoundError(Errors error)
    {
        StartCoroutine(HasFoundErrorRutiner(error));
    }

    /// <summary>
    /// Coroutine for handling the error UI display and interaction.
    /// </summary>
    /// <param name="error">The error to handle.</param>
    /// <returns>An enumerator.</returns>
    IEnumerator HasFoundErrorRutiner(Errors error)
    {
        // yield return null;

        while (errorHandler == null)
        {
            yield return null;
        }

        errorHandler.gameObject.SetActive(true);
        errorHandler.ShowErrorMessage(error);
    }

    /// <summary>
    /// Start the coroutine for displaying the error UI.
    /// </summary>
    /// <param name="error">The error to display.</param>
    public void HideDisplayError()
    {
        errorHandler.HideDisplayError();
    }

    /// <summary>
    /// Closes the error handling process for retrieving user data.
    /// </summary>
    public void ResetCurrentSceneAndSignOut()
    {
        FirebaseApp.Instance.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}