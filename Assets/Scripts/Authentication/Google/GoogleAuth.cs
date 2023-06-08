using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleAuth : MonoBehaviour
{
    [SerializeField] FirebaseApp firebase;

    string token;

    void Awake()
    {
        InitializePlayGamesLogin();
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
            firebase.LoginWithGoogle(token, null);
        }
        else
        {
            Debug.Log("Unsuccessful login");
        }
    }
}