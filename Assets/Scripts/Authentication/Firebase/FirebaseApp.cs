using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;

public class FirebaseApp : MonoBehaviour
{
    public static FirebaseApp Instance;

    public Dictionary<string, object> UserData { get { return userData; } }

    [HideInInspector] public UnityEvent OnSetFirebase;

    [SerializeField] LoginController loginController;
    [SerializeField] MainMenuController mainMenuController;
    [SerializeField] Sprite defaultUserPhoto;

    Dictionary<string, object> userData;

    Firebase.FirebaseApp app;
    Firebase.Auth.FirebaseUser user;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Success App Fire");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                OnSetFirebase?.Invoke();

                if (UserIsAuthenticated())
                {
                    user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
                    Invoke("GetUserData", .5f);
                }
                else
                {
                    Invoke("GameReadyToPlay", 1f);
                }
            }
            else
            {
                // Firebase Unity SDK is not safe to use here.
                StartCoroutine(ShowErrorUIRutiner(Errors.F_FA_68));
            }
        });

        // Close the error related to user photo.
        errorsRetryHandler = new Dictionary<Errors, Action>()
        {
            { Errors.F_FA_68,  RetryErrorFirebaseDependencies },
            { Errors.AUGGC_FA_122,  RetryErrorFirebaseAuthGoogle },
            { Errors.AUGGF_FA_127,  RetryErrorFirebaseAuthGoogle },
            { Errors.UP_FA_199, RetryErrorUserPhoto }
        };

        errorsCloseHandler = new Dictionary<Errors, Action>()
        {
            { Errors.F_FA_68,  CloseErrorFirebaseDependencies },
            { Errors.AUGGC_FA_122,  CloseErrorFirebaseAuthGoogle },
            { Errors.AUGGF_FA_127,  CloseErrorFirebaseAuthGoogle },
            { Errors.UP_FA_199, CloseErrorUserPhoto }
        };
    }

    /// <summary>
    /// Retrieves user data and performs necessary actions for game readiness.
    /// </summary>
    void GetUserData()
    {
        StartCoroutine(LoadAvatarImage(user.PhotoUrl.ToString()));
        CloudFirestore.Instance.GetUserData(user.UserId);
        CloudFirestore.Instance.UserLevels(user.UserId);
        CloudFirestore.Instance.UserCollectibles(user.UserId);
        mainMenuController.CheckLoadingGame();
    }

    /// <summary>
    /// Displays the UI for game readiness.
    /// </summary>
    void GameReadyToPlay()
    {
        mainMenuController.ShowUIGameReadyToPlay();
    }

    /// <summary>
    /// Logs in the user with Google credentials.
    /// </summary>
    /// <param name="googleIdToken">Google ID token.</param>
    /// <param name="googleAccessToken">Google access token.</param>
    public void LoginWithGoogle(string googleIdToken, string googleAccessToken)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                StartCoroutine(ShowErrorUIRutiner(Errors.AUGGC_FA_122));
                return;
            }
            if (task.IsFaulted)
            {
                StartCoroutine(ShowErrorUIRutiner(Errors.AUGGF_FA_127));
                return;
            }

            user = auth.CurrentUser;

            userData = new Dictionary<string, object>()
            {
                { "name", user.DisplayName },
                { "id", user.UserId },
                { "email", user.Email },
                { "url photo", user.PhotoUrl.ToString() }
            };

            GameManager.Instance.UserData = userData;
            loginController.LoginSuccess();
            StartCoroutine(LoadAvatarImage(user.PhotoUrl.ToString()));
        });
    }

    /// <summary>
    /// Logs in the user using the provided Facebook access token.
    /// </summary>
    /// <param name="accessToken">The Facebook access token.</param>
    public void LoginWithFacebook(string accessToken)
    {
        // Get the FirebaseAuth instance
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        // Create a Facebook credential using the access token
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
        // Sign in and retrieve user data with the Facebook credential
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled) // Check if the task was canceled
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted) // Check if the task encountered an error
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            // Get the current user from FirebaseAuth
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;

            // Call the loginController to handle successful login
            loginController.LoginSuccess();
            // Load the user's avatar image
            StartCoroutine(LoadAvatarImage(user.PhotoUrl.ToString()));
        });
    }

    /// <summary>
    /// Coroutine to load the avatar image from the given URL.
    /// </summary>
    /// <param name="url">URL of the image.</param>
    private IEnumerator LoadAvatarImage(string url)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            Sprite photoUser = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            GameManager.Instance.UserPhoto = photoUser;
            loginController.UserPhoto = photoUser;
        }
        else
        {
            StartCoroutine(ShowErrorUIRutiner(Errors.UP_FA_199));
        }
    }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated, false otherwise.</returns>
    bool UserIsAuthenticated() => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser != null;

    /// <summary>
    /// Signs out the user from Firebase.
    /// </summary>
    public void SignOut()
    {
        loginController.UserPhoto = null;
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
    }

    /// <summary>
    /// Checks if the user is currently signed out.
    /// </summary>
    /// <returns>True if the user is signed out, false otherwise.</returns>
    public bool IsSignedOut() => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser == null;

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
    /// Retry the error related to Firebase dependencies.
    /// </summary>
    void RetryErrorFirebaseDependencies()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Close the error related to Firebase dependencies.
    /// </summary>
    void CloseErrorFirebaseDependencies()
    {
        mainMenuController.ShowUIGameReadyToPlay();
    }

    /// <summary>
    /// Retry the error related to Firebase authentication with Google.
    /// </summary>
    private void RetryErrorFirebaseAuthGoogle()
    {
        GameManager.Instance.HideDisplayError();
        loginController.SignOutGoogle();
        StartCoroutine(RetryErrorFirebaseAuthGoogleRutiner());
    }

    /// <summary>
    /// Coroutine for retrying the error related to Firebase authentication with Google.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator RetryErrorFirebaseAuthGoogleRutiner()
    {
        yield return new WaitForSeconds(.1f);
        loginController.LoginGoogle();
    }

    /// <summary>
    /// Close the error related to Firebase authentication with Google.
    /// </summary>
    private void CloseErrorFirebaseAuthGoogle()
    {
        loginController.SignOutGoogle();
    }

    /// <summary>
    /// Retry the error related to user photo.
    /// </summary>
    private void RetryErrorUserPhoto()
    {
        GameManager.Instance.HideDisplayError();
        StartCoroutine(LoadAvatarImage(user.PhotoUrl.ToString()));
    }

    /// <summary>
    /// Close the error related to user photo.
    /// </summary>
    private void CloseErrorUserPhoto()
    {
        mainMenuController.ShowUIGameReadyToPlay();
        loginController.UserPhoto = defaultUserPhoto;
        GameManager.Instance.UserPhoto = defaultUserPhoto;
    }
}