using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GenderUser { Male, Female, Unknown }

public class LoginController : MonoBehaviour
{
    public Sprite UrlPhoto { get { return photoUser; } set { photoUser = value; } }

    [SerializeField] GameObject displaySingIn;
    [SerializeField] GameObject displaySingOut;
    [SerializeField] GameObject displayConfirmSingOut;

    //=================Gender==========================//
    [SerializeField] GameObject displayGetGender;

    //=================Sing In=========================//
    [SerializeField] TMP_Text welcomeText;
    [SerializeField] AvatarController avatar;
    [SerializeField] TMP_Text nameUserText;

    [SerializeField] GoogleAuth googleAuth;
    [SerializeField] FirebaseApp firebaseApp;

    GenderUser currentGenderUser;
    string nameUser;
    Sprite photoUser;

    /// <summary>
    /// Initiates the Google login process.
    /// </summary>
    public void LoginGoogle()
    {
        googleAuth.LoginGoogle();
    }

    /// <summary>
    /// Handles the successful login by displaying the appropriate UI.
    /// </summary>
    /// <param name="nameUser">Name of the logged-in user.</param>
    public void LoginSuccess(string nameUser)
    {
        StartCoroutine(LoginSuccessRutiner(nameUser));
    }

    /// <summary>
    /// Coroutine to handle the UI transition after a successful login.
    /// </summary>
    /// <param name="nameUser">Name of the logged-in user.</param>
    IEnumerator LoginSuccessRutiner(string nameUser)
    {
        yield return new WaitForEndOfFrame();
        displaySingIn.SetActive(false);
        displayGetGender.SetActive(true);
        this.nameUser = nameUser;
    }

    /// <summary>
    /// Toggles the selection of the male gender.
    /// </summary>
    /// <param name="toggle">Toggle component for male gender.</param>
    public void ToggleMale(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Male;
    }

    /// <summary>
    /// Toggles the selection of the female gender.
    /// </summary>
    /// <param name="toggle">Toggle component for female gender.</param>
    public void ToggleFemale(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Female;
    }

    /// <summary>
    /// Toggles the selection of the unknown gender.
    /// </summary>
    /// <param name="toggle">Toggle component for unknown gender.</param>
    public void ToggleUnknown(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Unknown;
    }

    /// <summary>
    /// Continues the process after selecting the gender and sets the user information.
    /// </summary>
    public void ContinueGenderUser()
    {
        SetInformationUser();
    }

    /// <summary>
    /// Sets the user information and updates the UI accordingly.
    /// </summary>
    public void SetInformationUser()
    {
        displayGetGender.SetActive(false);
        displaySingOut.SetActive(true);

        welcomeText.text = string.Format($"Hello, {nameUser.Split(' ')[0]}!");
        nameUserText.text = nameUser;

        if (currentGenderUser == GenderUser.Male) avatar.UserMan(photoUser);
        else if (currentGenderUser == GenderUser.Female) avatar.UserWomen(photoUser);
        else if (currentGenderUser == GenderUser.Unknown) avatar.UserUnknown(photoUser);
    }

    /// <summary>
    /// Initiates the sign out process.
    /// </summary>
    public void SignOut()
    {
        displayConfirmSingOut.SetActive(true);
    }
}