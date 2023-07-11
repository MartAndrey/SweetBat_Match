using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using TMPro;

public class CloudFirestore : MonoBehaviour
{
    public TMP_Text text;
    public static CloudFirestore Instance;

    FirebaseFirestore db;
    Dictionary<Errors, Action> errorsRetryHandler;
    Dictionary<Errors, Action> errorsCloseHandler;

    void OnEnable()
    {
        FirebaseApp.Instance.OnSetFirebase.AddListener(InitializeData);
        GameManager.Instance.OnErrorRetry.AddListener(OnErrorRetry);
        GameManager.Instance.OnErrorClose.AddListener(OnErrorClose);
    }

    void OnDisable()
    {
        FirebaseApp.Instance.OnSetFirebase.RemoveListener(InitializeData);
        GameManager.Instance.OnErrorRetry.RemoveListener(OnErrorRetry);
        GameManager.Instance.OnErrorClose.RemoveListener(OnErrorClose);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Close the error related to user photo.
        errorsRetryHandler = new Dictionary<Errors, Action>()
        {
            { Errors.CNU_CF_80,  RetryErrorCreatedNewUser },
            { Errors.GUD_CF_118,  RetryErrorGetUserData },
            { Errors.GUL_CF_153,  RetryErrorGetUserLevels },
            { Errors.SUL_CF_188,  RetryErrorSetUserLevels },
            { Errors.GUC_CF_211,  RetryErrorGetUserCollectibles },
            { Errors.SUC_CF_223,  RetryErrorSetUserCollectibles },
        };

        errorsCloseHandler = new Dictionary<Errors, Action>()
        {
            { Errors.CNU_CF_80,  CloseErrorCreatedNewUser },
            { Errors.GUD_CF_118,  CloseErrorGetUserData },
            { Errors.GUL_CF_153,  CloseErrorGetUserLevels },
            { Errors.SUL_CF_188,  CloseErrorSetUserLevels },
            { Errors.GUC_CF_211,  CloseErrorGetUserCollectibles },
            { Errors.SUC_CF_223,  CloseErrorSetUserCollectibles },
        };

        DontDestroyOnLoad(gameObject);
    }

    void InitializeData()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Creates a new user in the database with the provided user data.
    /// </summary>
    /// <param name="userData">A dictionary containing user data.</param>
    public void CreateNewUser(Dictionary<string, object> userData)
    {
        DocumentReference userRef = db.Collection("Users").Document(userData["id"].ToString());

        userRef.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                StartCoroutine(ShowErrorUIRutiner(Errors.CNU_CF_80));
            }
        });
    }

    /// <summary>
    /// Checks if a user with the given user ID exists in the database.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    async public Task<bool> CheckUserExists(string userId)
    {
        DocumentReference userRef = db.Collection("Users").Document(userId);

        DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            GameManager.Instance.UserData = snapshot.ToDictionary();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves user data from the database using the specified user ID.
    /// </summary>
    /// <param name="userId">ID of the user to retrieve data for.</param>
    public void GetUserData(string userId)
    {
        DocumentReference docRef = db.Collection("Users").Document(userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                GameManager.Instance.UserData = task.Result.ToDictionary();
            else if (task.IsFaulted || task.IsCanceled)
                StartCoroutine(ShowErrorUIRutiner(Errors.GUD_CF_118));

        });
    }

    /// <summary>
    /// Retrieves the levels associated with a user from the database.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    public void UserLevels(string userId)
    {
        // Get the reference to the user's levels collection
        CollectionReference userRef = db.Collection("Users").Document(userId).Collection("Levels");

        // Get the snapshot of the user's levels
        userRef.OrderBy("order").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                List<Dictionary<string, object>> levels = new List<Dictionary<string, object>>();

                QuerySnapshot snapshot = task.Result;

                // Iterate through each document snapshot and convert it to a dictionary
                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                {
                    Dictionary<string, object> levelData = documentSnapshot.ToDictionary();
                    levels.Add(levelData);
                }

                // Assign the retrieved levels to the GameManager instance
                GameManager.Instance.LevelsData = levels;
            }
            else
            {
                StartCoroutine(ShowErrorUIRutiner(Errors.GUL_CF_153));
            }
        });
    }

    /// <summary>
    /// Sets the user levels in the database.
    /// </summary>
    /// <param name="userData">List of level data to set.</param>
    public void SetUserLevels(List<Dictionary<string, object>> userData)
    {
        // Get the current level
        int currentLevel = GameManager.Instance.Level + 1;
        // Get the reference to the user's levels collection
        CollectionReference userRef = db.Collection("Users").Document(GameManager.Instance.UserData["id"].ToString()).Collection("Levels");

        // Start a batch write operation
        WriteBatch batch = db.StartBatch();

        // Iterate over the level data and set them in the database
        userData.ForEach(level =>
        {
            // Get the document reference for the level
            DocumentReference newLevelDoc = userRef.Document($"level {currentLevel}");

            // Set the level data in the batch operation
            batch.Set(newLevelDoc, level);
            currentLevel++;
        });

        // Commit the batch operation asynchronously
        batch.CommitAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                StartCoroutine(ShowErrorUIRutiner(Errors.SUL_CF_188));
            }
        });
    }

    /// <summary>
    /// Retrieves the collectibles associated with a user from the database.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    public void UserCollectibles(string userId)
    {
        // Get the reference to the user's collectibles document
        DocumentReference userRef = db.Collection("Users").Document(userId).Collection("Collectibles").Document("collectable");

        // Get the snapshot of the user's collectibles
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                // Assign the retrieved collectibles to the GameManager instance
                GameManager.Instance.CollectiblesData = task.Result.ToDictionary();
            }
            else if (task.IsCanceled || task.IsFaulted)
                StartCoroutine(ShowErrorUIRutiner(Errors.GUC_CF_211));
        });
    }

    /// <summary>
    /// Sets the collectible data in the database.
    /// </summary>
    /// <param name="collectableData">The data to be set in the database.</p
    public void SetCollectible(Dictionary<string, object> collectableData)
    {
        DocumentReference docRef = db.Collection("Users").Document(GameManager.Instance.UserData["id"].ToString()).Collection("Collectibles").Document("collectable");

        docRef.SetAsync(collectableData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
         {
             if (task.IsCanceled && task.IsFaulted)
             {
                 StartCoroutine(ShowErrorUIRutiner(Errors.SUC_CF_223));
             }
         });
    }

    /// <summary>
    /// Updates the user's data with the provided userData dictionary.
    /// It uses a DocumentReference to update the data in Firestore and handles the completion status.
    /// </summary>
    /// <param name="UserLevel">The dictionary containing the updated user data.</param>
    public void UpdateLevelUser(Dictionary<string, object> UserLevel)
    {
        DocumentReference userRef = db.Collection("Users").Document(GameManager.Instance.UserData["id"].ToString());

        userRef.UpdateAsync(UserLevel).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // StartCoroutine(ShowErrorUIRutiner(Errors.CNU_CF_80));
            }
        });
    }

    /// <summary>
    /// Coroutine for displaying the error UI.
    /// </summary>
    /// <param name="error">The error to be displayed.</param>
    /// <returns>An enumerator.</returns>
    IEnumerator ShowErrorUIRutiner(Errors error)
    {
        yield return null;

        GameManager.Instance.HasFoundError(error);
    }

    /// <summary>
    /// Handles the retry action for a specific error.
    /// </summary>
    /// <param name="error">The error to retry.</param>
    void OnErrorRetry(Errors error)
    {
        if (errorsRetryHandler.ContainsKey(error)) errorsRetryHandler[error]();
    }

    /// <summary>
    /// Handles the close action for a specific error.
    /// </summary>
    /// <param name="error">The error to close.</param>
    void OnErrorClose(Errors error)
    {
        if (errorsCloseHandler.ContainsKey(error)) errorsCloseHandler[error]();
    }

    /// <summary>
    /// Retries the error handling process for user creation.
    /// </summary>
    void RetryErrorCreatedNewUser()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorCreatedNewUserRutiner());
    }

    /// <summary>
    /// Coroutine for retrying the error handling process for user creation.
    /// </summary>
    IEnumerator RetryErrorCreatedNewUserRutiner()
    {
        yield return new WaitForSeconds(.1f);
        CreateNewUser(FirebaseApp.Instance.UserData);
    }

    /// <summary>
    /// Closes the error handling process for user creation.
    /// </summary>
    void CloseErrorCreatedNewUser()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }

    /// <summary>
    /// Retries the error handling process for retrieving user data.
    /// </summary>
    void RetryErrorGetUserData()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorGetUserDataRutiner());
    }

    /// <summary>
    /// Retries the error handling process for retrieving user data.
    /// </summary>
    IEnumerator RetryErrorGetUserDataRutiner()
    {
        yield return new WaitForSeconds(1f);
        GetUserData(FirebaseApp.Instance.UserData["id"].ToString());
    }

    /// <summary>
    /// Closes the error handling process for retrieving user data.
    /// </summary>
    void CloseErrorGetUserData()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }

    /// <summary>
    /// Retries the process of getting user levels after an error occurred.
    /// </summary>
    void RetryErrorGetUserLevels()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorGetUserLevelsRutiner());
    }

    IEnumerator RetryErrorGetUserLevelsRutiner()
    {
        yield return new WaitForSeconds(.1f);
        UserLevels(GameManager.Instance.UserData["id"].ToString());
    }

    /// <summary>
    /// Closes the error display and resets the current scene and sign out the user after an error occurred while getting user levels.
    /// </summary>
    void CloseErrorGetUserLevels()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }

    /// <summary>
    /// Retries the process of setting user levels after an error occurred.
    /// </summary>
    void RetryErrorSetUserLevels()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorSetUserLevelsRutiner());
    }

    IEnumerator RetryErrorSetUserLevelsRutiner()
    {
        yield return new WaitForSeconds(.1f);
        SetUserLevels(GameManager.Instance.LevelsData);
    }

    /// <summary>
    /// Closes the error display and resets the current scene and sign out the user after an error occurred while setting user levels.
    /// </summary>
    void CloseErrorSetUserLevels()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }

    /// <summary>
    /// Retries the process of getting user collectibles after an error occurred.
    /// </summary>
    void RetryErrorGetUserCollectibles()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorGetUserCollectiblesRutiner());
    }

    IEnumerator RetryErrorGetUserCollectiblesRutiner()
    {
        yield return new WaitForSeconds(.1f);
        UserCollectibles(GameManager.Instance.UserData["id"].ToString());
    }

    /// <summary>
    /// Closes the error display and resets the current scene and sign out the user after an error occurred while getting user collectibles.
    /// </summary>
    void CloseErrorGetUserCollectibles()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }

    /// <summary>
    /// Retries the process of setting user collectibles after an error occurred.
    /// </summary>
    void RetryErrorSetUserCollectibles()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorSetUserCollectiblesRutiner());
    }

    IEnumerator RetryErrorSetUserCollectiblesRutiner()
    {
        yield return new WaitForSeconds(.1f);
        SetCollectible(GameManager.Instance.CollectiblesData);
    }

    /// <summary>
    /// Closes the error display and resets the current scene and sign out the user after an error occurred while setting user collectibles.
    /// </summary>
    void CloseErrorSetUserCollectibles()
    {
        GameManager.Instance.ResetCurrentSceneAndSignOut();
    }
}