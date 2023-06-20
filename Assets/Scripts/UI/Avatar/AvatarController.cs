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
}