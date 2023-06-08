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
        avatar.sprite = overlayUnknown;
        photoAvatarUnknown.sprite = photoAvatar;
    }
}