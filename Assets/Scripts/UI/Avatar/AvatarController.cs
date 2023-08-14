using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarController : MonoBehaviour
{
    [SerializeField] Image avatar;
    [Space()]
    [SerializeField] GameObject boxAvatarMan;
    [SerializeField] GameObject boxAvatarWomen;
    [SerializeField] GameObject boxAvatarUnknown;
    [Space()]
    [SerializeField] Image photoAvatarMan;
    [SerializeField] Image photoAvatarWomen;
    [SerializeField] Image photoAvatarUnknown;
    [Space()]
    [SerializeField] Sprite overlayMan;
    [SerializeField] Sprite overlayWomen;
    [SerializeField] Sprite overlayUnknown;
    [Space()]
    [SerializeField] Image imageFrameMan;
    [SerializeField] GameObject informationUserMan;
    [SerializeField] GameObject offsetMan;
    [SerializeField] TMP_Text textNameMan;
    [SerializeField] TMP_Text textLevelMan;
    [Space()]
    [SerializeField] Image imageFrameWomen;
    [SerializeField] GameObject informationUserWomen;
    [SerializeField] GameObject offsetWomen;
    [SerializeField] TMP_Text textNameWomen;
    [SerializeField] TMP_Text textLevelWomen;
    [Space()]
    [SerializeField] TMP_Text textNameUnknown;
    [SerializeField] TMP_Text textLevelUnknown;

    [SerializeField] bool showFrame = false;

    /// <summary>
    /// Sets the UI and displays the male avatar.
    /// </summary>
    /// <param name="photoAvatar">Sprite of the user's avatar.</param>
    public void UserMan(Sprite photoAvatar)
    {
        boxAvatarMan.SetActive(true);
        boxAvatarUnknown.SetActive(false);
        boxAvatarWomen.SetActive(false);

        avatar.sprite = overlayMan;
        photoAvatarMan.sprite = photoAvatar;

        if (showFrame)
        {
            imageFrameMan.enabled = true;
            informationUserMan.SetActive(true);
            offsetMan.SetActive(true);
            textNameMan.text = GameManager.Instance.UserData["name"].ToString().Split(' ')[0];
            textLevelMan.text = (GameManager.Instance.Level + 1).ToString();
        }
    }

    /// <summary>
    /// Sets the UI and displays the female avatar.
    /// </summary>
    /// <param name="photoAvatar">Sprite of the user's avatar.</param>
    public void UserWomen(Sprite photoAvatar)
    {
        boxAvatarWomen.SetActive(true);
        boxAvatarUnknown.SetActive(false);
        boxAvatarMan.SetActive(false);

        avatar.sprite = overlayWomen;
        photoAvatarWomen.sprite = photoAvatar;

        if (showFrame)
        {
            imageFrameWomen.enabled = true;
            informationUserWomen.SetActive(true);
            offsetWomen.SetActive(true);
            textNameWomen.text = GameManager.Instance.UserData["name"].ToString().Split(' ')[0];
            textLevelWomen.text = (GameManager.Instance.Level + 1).ToString();
        }
    }

    /// <summary>
    /// Sets the UI and displays the female avatar.
    /// </summary>
    /// <param name="photoAvatar">Sprite of the user's avatar.</param>
    public void UserUnknown(Sprite photoAvatar)
    {
        boxAvatarUnknown.SetActive(true);
        boxAvatarMan.SetActive(false);
        boxAvatarWomen.SetActive(false);

        avatar.sprite = overlayUnknown;
        photoAvatarUnknown.sprite = photoAvatar;

        if (showFrame)
        {
            textNameUnknown.text = GameManager.Instance.UserData["name"].ToString().Split(' ')[0];
            textLevelUnknown.text = (GameManager.Instance.Level + 1).ToString();
        }
    }

    /// <summary>
    /// Updates the avatar based on the gender and photo of the user.
    /// </summary>
    /// <param name="genderUser">The gender of the user.</param>
    /// <param name="photoUser">The user's photo.</param>
    public void UpdateAvatar(GenderUser genderUser, Sprite photoUser)
    {
        if (genderUser == GenderUser.Male) UserMan(photoUser);
        else if (genderUser == GenderUser.Female) UserWomen(photoUser);
        else if (genderUser == GenderUser.Unknown) UserUnknown(photoUser);
    }

    /// <summary>
    /// Updates the anonymous user's avatar information.
    /// </summary>
    public void UpdateAvatarAnonymous()
    {
        if (showFrame)
        {
            textNameUnknown.text = GameManager.Instance.UserData["name"].ToString().Split(' ')[0];
            textLevelUnknown.text = (GameManager.Instance.Level + 1).ToString();
        }
    }
}