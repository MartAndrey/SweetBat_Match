using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.SocialPlatforms;

public class GoogleAuth : MonoBehaviour
{
    [SerializeField] TMP_Text message;
    [SerializeField] TMP_Text id;
    [SerializeField] TMP_Text namePlayer;
    [SerializeField] TMP_Text tokenId;
    [SerializeField] TMP_Text accessToken;
    [SerializeField] TMP_Text nameAccount;
    [SerializeField] FirebaseApp firebase;

    private void Start()
    {
        InitializePlayGamesLogin();
    }

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

    public void LoginGoogle()
    {
        Social.localUser.Authenticate(OnGoogleLogin);
    }

    void OnGoogleLogin(bool success)
    {
        if (success)
        {
            message.text = "Success";
            // Call Unity Authentication SDK to sign in or link with Google.
            Debug.Log("Login with Google done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
            id.text = "Id " + PlayGamesPlatform.Instance.localUser.id;
            namePlayer.text = "Name " + PlayGamesPlatform.Instance.localUser.userName;

            tokenId.text = "ID" + PlayGamesPlatform.Instance.GetIdToken();

            accessToken.text = "Token" + PlayGamesPlatform.Instance.GetServerAuthCode();
            firebase.LoginWithGoogle(PlayGamesPlatform.Instance.GetIdToken(), PlayGamesPlatform.Instance.GetServerAuthCode());
            nameAccount.text = "Name " + PlayGamesPlatform.Instance.GetUserDisplayName();
        }
        else
        {
            message.text = "Error";
            Debug.Log("Unsuccessful login");
        }
    }
}