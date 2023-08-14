using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine.Networking;
using System.Threading.Tasks;

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
    [Description("We're sorry, but you don't seem to have an internet connection right now. To access the online features, please check your Wi-Fi or mobile data connection and try again. Thank you!. If the problem persists, contact the game developer")]
    NNA_GM_THIS,
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
    CNU_CF_80,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    GUD_CF_118,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    GUL_CF_153,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    SUL_CF_188,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    GUC_CF_211,
    [Description("Something went wrong while synchronizing with the database. If the problem persists, contact the game developer.")]
    SUC_CF_223,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    /// Observer Pattern
    [HideInInspector] public UnityEvent<GameMode> OnGameMode;
    [HideInInspector] public UnityEvent OnUniqueMatches;
    [HideInInspector] public UnityEvent<Errors> OnErrorRetry;
    [HideInInspector] public UnityEvent<Errors> OnErrorClose;
    Dictionary<Errors, Action> errorsRetryHandler;
    Dictionary<Errors, Action> errorsCloseHandler;

    public int Level { get { return level; } set { level = value; } }  // Public getter for the current level
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

    public List<Dictionary<string, object>> LevelsData { get { return levelsData; } set { levelsData = value; } }
    public Dictionary<string, object> CollectiblesData { get { return collectiblesData; } set { collectiblesData = value; } }

    public int CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }  // Public getter for the current level
    public bool PowerUpActivate { get { return powerUpActivate; } set { powerUpActivate = value; } }
    public TypePowerUp CurrentPowerUp { get { return currentPowerUp; } set { currentPowerUp = value; } }
    public GameObject CurrentGameObjectPowerUp { get { return currentGameObjectPowerUp; } set { currentGameObjectPowerUp = value; } }

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

    List<Dictionary<string, object>> levelsData;
    Dictionary<string, object> collectiblesData;

    ErrorHandler errorHandler;

    int currentLevel;

    bool powerUpActivate;
    TypePowerUp currentPowerUp;
    GameObject currentGameObjectPowerUp;

    /// <summary>
    /// Subscribes to the SceneManager's sceneLoaded event when the script is enabled.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnErrorRetry.AddListener(ErrorRetry);
        OnErrorClose.AddListener(ErrorClose);
    }

    /// <summary>
    /// Unsubscribes from the SceneManager's sceneLoaded event when the script is disabled.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        OnErrorRetry.RemoveListener(ErrorRetry);
        OnErrorClose.RemoveListener(ErrorClose);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameMode == GameMode.ScoringObjective || gameMode == GameMode.TimeObjective)
            uniqueMatches = true;

        errorsRetryHandler = new Dictionary<Errors, Action>()
        {
            { Errors.NNA_GM_THIS,  RetryErrorNetworkAvailable },
        };

        errorsCloseHandler = new Dictionary<Errors, Action>()
        {
            { Errors.NNA_GM_THIS,  CloseErrorNetworkAvailable },
        };
    }

    /// <summary>
    /// Handles the retry action for a specific error.
    /// </summary>
    /// <param name="error">The error to retry.</param>
    void ErrorRetry(Errors error)
    {
        if (errorsRetryHandler.ContainsKey(error)) errorsRetryHandler[error]();
    }

    /// <summary>
    /// Handles the close action for a specific error.
    /// </summary>
    /// <param name="error">The error to close.</param>
    void ErrorClose(Errors error)
    {
        if (errorsCloseHandler.ContainsKey(error)) errorsCloseHandler[error]();
    }

    /// <summary>
    /// Initiates the process after a successful anonymous login.
    /// </summary>
    public void LoginSuccessAnonymous()
    {
        StartCoroutine(LoginSuccessAnonymousRutiner());
    }

    /// <summary>
    /// Coroutine to handle anonymous login success operations.
    /// </summary>
    IEnumerator LoginSuccessAnonymousRutiner()
    {
        // Check if the user already exists in the Cloud Firestore database
        Task<bool> checkUserTask = CloudFirestore.Instance.CheckUserExists(userData["id"].ToString());

        yield return new WaitUntil(() => checkUserTask.IsCompleted);

        bool userExists = checkUserTask.Result;

        if (userExists)
        {
            // If user exists, retrieve user-related data and yield break
            CloudFirestore.Instance.GetUserData(userData["id"].ToString());
            CloudFirestore.Instance.UserLevels(userData["id"].ToString());
            CloudFirestore.Instance.UserCollectibles(userData["id"].ToString());
            yield break;
        }

        // If user does not exist, create a new user
        CloudFirestore.Instance.CreateNewUser(userData);
    }

    /// <summary>
    /// Checks the internet connection and initiates the connection check routine if available.
    /// </summary>
    void CheckInternetConnection(Scene scene)
    {
        if (NetworkInterface.GetIsNetworkAvailable())
        {
            // Initiate the internet connection check routine.
            StartCoroutine(CheckInternetConnectionRutiner(scene));
        }
        else HasFoundError(Errors.NNA_GM_THIS); // Handle error when no network connection is available.
    }

    /// <summary>
    /// Coroutine to perform the internet connection check.
    /// </summary>
    IEnumerator CheckInternetConnectionRutiner(Scene scene)
    {
        Task<bool> result = IsConnectionNetwork();

        yield return new WaitUntil(() => result.IsCompleted);

        if (result.Result)
        {
            if (scene.name == "MainMenu")
            {
                // Start the Firebase service when network connection is available.
                FirebaseApp firebaseApp = FindObjectOfType<FirebaseApp>();
                firebaseApp.StartFirebaseService();
            }
        }
        else HasFoundError(Errors.NNA_GM_THIS);  // Handle error when network connection check fails.
    }

    /// <summary>
    /// Checks if a network connection to a specific URL is available.
    /// </summary>
    /// <returns>True if network connection is available, otherwise false.</returns>
    async Task<bool> IsConnectionNetwork()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://www.google.com");
        var asyncOperation = www.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Network connection is available.
            return true;
        }

        // Network connection is not available.
        return false;
    }

    /// <summary>
    /// Increases the level count and starts the next level routine.
    /// </summary>
    public void NextLevel(int stars)
    {
        Dictionary<string, object> currentLevel = levelsData[this.currentLevel];

        if (this.currentLevel == level)
        {
            UpdateLevelComplete(currentLevel, stars, true);
            // Starts the next level routine
            StartCoroutine(NextLevelRoutine());
        }
        else
        {
            if (stars > Convert.ToInt32(currentLevel["Stars"]))
            {
                UpdateLevelComplete(currentLevel, stars, false);
            }
        }
    }

    /// <summary>
    /// Updates the completion status of the current level.
    /// </summary>
    /// <param name="currentLevel">The dictionary representing the current level.</param>
    /// <param name="stars">The number of stars obtained by the player.</param>
    /// <param name="updateLevelDataBase">Indicates whether to update the level data in the database.</param>
    void UpdateLevelComplete(Dictionary<string, object> currentLevel, int stars, bool updateLevelDataBase)
    {
        currentLevel["Stars"] = stars;
        CloudFirestore.Instance.UpdateDocumentLevel($"level {this.currentLevel + 1}", currentLevel);

        if (updateLevelDataBase)
            CloudFirestore.Instance.UpdateLevelUser(new Dictionary<string, object> { { "level", this.currentLevel + 1 } });
    }

    /// <summary>
    /// Waits for 2 seconds and finds the LevelManager object.
    /// </summary>
    IEnumerator NextLevelRoutine()
    {
        yield return new WaitForSeconds(2.5f);

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
        {
            UpdateAvatars();
        }

        CheckInternetConnection(scene);
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
        while ((userPhoto == null && !FirebaseApp.Instance.User.IsAnonymous) || userData == null)
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
            if (!FirebaseApp.Instance.User.IsAnonymous)
                avatars[i].UpdateAvatar((GenderUser)Enum.Parse(typeof(GenderUser), userData["gender"].ToString()), userPhoto);
            else avatars[i].UpdateAvatarAnonymous();
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
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Retrieves the current user's level from the userData dictionary.
    /// If the level data is not available, it updates the user's level to 0.
    /// </summary>
    public void GetCurrentLevelUser()
    {
        if (userData.ContainsKey("level"))
            level = Convert.ToInt32(userData["level"]);
        else
        {
            CloudFirestore.Instance.UpdateLevelUser(new Dictionary<string, object> { { "level", 0 } });
            level = 0;
        }
    }

    /// <summary>
    /// Retry network error handling by rechecking the internet connection.
    /// </summary>
    void RetryErrorNetworkAvailable()
    {
        HideDisplayError();
        // Retry by initiating internet connection check.
        StartCoroutine(RetryErrorGetUserCollectiblesRutiner());
    }

    /// <summary>
    /// Coroutine to retry handling network error by rechecking the internet connection.
    /// </summary>
    IEnumerator RetryErrorGetUserCollectiblesRutiner()
    {
        // Retry the internet connection check after a short delay.
        yield return new WaitForSeconds(.1f);
        CheckInternetConnection(SceneManager.GetActiveScene());
    }

    /// <summary>
    /// Close the error handling due to a network error and reset the current scene while signing out.
    /// </summary>
    void CloseErrorNetworkAvailable()
    {
        // Close the error UI, reset the scene, and sign out the user.
        ResetCurrentSceneAndSignOut();
    }
}