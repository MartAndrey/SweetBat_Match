using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarController : MonoBehaviour
{
    [SerializeField] GameObject boxAvatarMan;
    [SerializeField] GameObject boxAvatarWomen;
    [SerializeField] Image photoAvatarMan;
    [SerializeField] Image photoAvatarWomen;

    public void UserMan(Sprite photoAvatar)
    {
        boxAvatarMan.SetActive(true);
        boxAvatarWomen.SetActive(false);
        photoAvatarMan.sprite = photoAvatar;
    }

     public void UserWomen(Sprite photoAvatar)
    {
        boxAvatarWomen.SetActive(true);
        boxAvatarMan.SetActive(false);
        photoAvatarWomen.sprite = photoAvatar;
        
    }
}
