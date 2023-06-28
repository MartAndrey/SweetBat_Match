using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using System.Collections;

public class CloudFirestore : MonoBehaviour
{
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
            { Errors.CNU_CF_70,  RetryErrorCreatedNewUser },
            { Errors.GUD_CF_103,  RetryErrorGetUserData },
        };

        errorsCloseHandler = new Dictionary<Errors, Action>()
        {
            { Errors.CNU_CF_70,  CloseErrorCreatedNewUser },
            { Errors.GUD_CF_103,  CloseErrorGetUserData },
        };
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
                StartCoroutine(ShowErrorUIRutiner(Errors.CNU_CF_70));
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
                StartCoroutine(ShowErrorUIRutiner(Errors.GUD_CF_103));

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
}