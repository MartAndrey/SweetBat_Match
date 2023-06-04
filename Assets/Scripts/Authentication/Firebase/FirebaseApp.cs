using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FirebaseApp : MonoBehaviour
{
    Firebase.FirebaseApp app;

    [SerializeField] TMP_Text message;
    [SerializeField] TMP_Text id;
    [SerializeField] TMP_Text namePlayer;
    [SerializeField] TMP_Text email;
    [SerializeField] TMP_Text phone;
    [SerializeField] TMP_Text photo;
    [SerializeField] Image avatarImage;
    [SerializeField] TMP_Text avatarText;

    void Start()
    {
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
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void LoginWithGoogle(string googleIdToken, string googleAccessToken)
    {
        // string googleToken = playGamesAuth.Token;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                message.text = "Was Cancelled";
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                message.text = "Encountred an error V3";
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            message.text = "Susccess";
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;

            id.text = "ID" + user.UserId;
            namePlayer.text = "Name" + newUser.DisplayName;
            email.text = "Email " + user.Email;
            phone.text = "Phone " + user.PhoneNumber;

            if (user.DisplayName == null)
            {
                namePlayer.text = "Null";
            }

            // foreach (var info in user.ProviderData)
            // {
            //     if (info.ProviderId == "google.com")
            //     {
            //         namePlayer.text = "Name: " + info.DisplayName;
            //         break;
            //     }
            // }

            photo.text = "Photo " + user.PhotoUrl;

            StartCoroutine(LoadAvatarImage(photo.text));

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
    }

    private IEnumerator LoadAvatarImage(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {

                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);


                avatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), Vector2.zero);
                avatarText.text = "Success";
            }
            else
            {
                avatarText.text = "Error";
                Debug.LogError("Error : " + webRequest.error);
            }
        }
    }
}
