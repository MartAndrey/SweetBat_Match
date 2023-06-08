using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseApp : MonoBehaviour
{
    [SerializeField] LoginController loginController;

    Firebase.FirebaseApp app;

    void Awake()
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
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser user = auth.CurrentUser;

            loginController.LoginSuccess(user.DisplayName);
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
            loginController.UrlPhoto = photoUser;
        }
        else
        {
            Debug.LogError("Error : " + webRequest.error);
        }
    }

    /// <summary>
    /// Signs out the user from Firebase.
    /// </summary>
    public void SignOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
    }
}