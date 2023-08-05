// using System.Collections.Generic;
// using UnityEngine;
// using Facebook.Unity;

// public class FacebookAuth : MonoBehaviour
// {
//     string Token;
//     string Error;

//     // Awake function from Unity's MonoBehavior
//     void Awake()
//     {
//         if (!FB.IsInitialized)
//         {
//             // Initialize the Facebook SDK
//             FB.Init(InitCallback, OnHideUnity);
//         }
//         else
//         {
//             // Already initialized, signal an app activation App Event
//             FB.ActivateApp();
//         }
//     }

//     /// <summary>
//     /// Callback function when the Facebook SDK initialization is complete.
//     /// </summary>
//     void InitCallback()
//     {
//         if (FB.IsInitialized)
//         {
//             // Signal an app activation App Event
//             FB.ActivateApp();
//             // Continue with Facebook SDK
//         }
//         else
//         {
//             Debug.Log("Failed to Initialize the Facebook SDK");
//         }
//     }

//     /// <summary>
//     /// Callback function when the Facebook SDK initialization is complete.
//     /// </summary>
//     void OnHideUnity(bool isGameShown)
//     {
//         if (!isGameShown)
//         {
//             // Pause the game - we will need to hide
//             Time.timeScale = 0;
//         }
//         else
//         {
//             // Resume the game - we're getting focus again
//             Time.timeScale = 1;
//         }
//     }

//     /// <summary>
//     /// Initiates the Facebook login process.
//     /// </summary>
//     public void LoginFacebook()
//     {
//         // Define the permissions
//         var perms = new List<string>() { "public_profile", "email" };

//         FB.LogInWithReadPermissions(perms, result =>
//         {
//             if (FB.IsLoggedIn)
//             {
//                 Token = AccessToken.CurrentAccessToken.TokenString;
//                 Debug.Log($"Facebook Login token: {Token}");
//                 FirebaseApp.Instance.LoginWithFacebook(Token);
//             }
//             else
//             {
//                 Error = "User cancelled login";
//                 Debug.Log("[Facebook Login] User cancelled login");
//             }
//         });
//     }
// }