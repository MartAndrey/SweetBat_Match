using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using System;
using System.Collections;

public class GoogleAuth : MonoBehaviour
{
    string token;

    Dictionary<Errors, Action> errorsRetryHandler;
    Dictionary<Errors, Action> errorsCloseHandler;

    void OnEnable()
    {
        GameManager.Instance.OnErrorRetry.AddListener(OnErrorRetry);
        GameManager.Instance.OnErrorClose.AddListener(OnErrorClose);
    }

    void OnDisable()
    {
        GameManager.Instance.OnErrorRetry.RemoveListener(OnErrorRetry);
        GameManager.Instance.OnErrorClose.RemoveListener(OnErrorClose);
    }

    void Awake()
    {
        InitializePlayGamesLogin();

        // Close the error related to user photo.
        errorsRetryHandler = new Dictionary<Errors, Action>()
        {
            { Errors.AUGG_GA_71,  RetryErrorLoginGoogle },
        };

        errorsCloseHandler = new Dictionary<Errors, Action>()
        {
            { Errors.AUGG_GA_71,  CloseErrorLoginGoogle },
        };
    }

    /// <summary>
    /// Initializes the Play Games login configuration.
    /// </summary>
    void InitializePlayGamesLogin()
    {
        var config = new PlayGamesClientConfiguration.Builder()
            // Requests an ID token be generated.  
            // This OAuth token can be used to
            // identify the player to other services such as Firebase.
            .RequestIdToken()
            .AddOauthScope("profile")
            .RequestEmail()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    /// <summary>
    /// Initiates the Google login process.
    /// </summary>
    public void LoginGoogle()
    {
        Social.localUser.Authenticate(OnGoogleLogin);
    }

    /// <summary>
    /// Callback function after Google login is completed.
    /// </summary>
    /// <param name="success">Boolean indicating the success of the login process.</param>
    void OnGoogleLogin(bool success)
    {
        if (success)
        {
            token = PlayGamesPlatform.Instance.GetIdToken();
            FirebaseApp.Instance.LoginWithGoogle(token, null);
        }
        else
        {
            StartCoroutine(ShowErrorUIRutiner(Errors.AUGG_GA_71));
        }
    }

    /// <summary>
    /// Signs out the user.
    /// </summary>
    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    /// <summary>
    /// Checks if the user is currently signed out.
    /// </summary>
    /// <returns>True if the user is signed out, false otherwise.</returns>
    public bool IsSignedOut() => !PlayGamesPlatform.Instance.localUser.authenticated;

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
    /// Retries the error handling process for Google login.
    /// </summary>
    void RetryErrorLoginGoogle()
    {
        SignOut();
        GameManager.Instance.HideDisplayError();
        StartCoroutine(RetryErrorLoginGoogleRutiner());
    }

    /// <summary>
    /// Coroutine for retrying the error handling process for Google login.
    /// </summary>
    IEnumerator RetryErrorLoginGoogleRutiner()
    {
        yield return new WaitForSeconds(.1f);
        LoginGoogle();
    }

    /// <summary>
    /// Closes the error handling process for Google login.
    /// </summary>
    void CloseErrorLoginGoogle()
    {
        SignOut();
    }
}