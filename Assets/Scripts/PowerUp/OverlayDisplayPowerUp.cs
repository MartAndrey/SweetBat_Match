using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayDisplayPowerUp : MonoBehaviour
{
    // Gets the current state of the overlay display.
    public bool OverlayEnable { get { return overlay.enabled; } }

    [SerializeField] GameObject containerInfo;
    [SerializeField] Image imageDisplay;

    Image overlay;

    void Awake()
    {
        overlay = GetComponent<Image>();
    }

    /// <summary>
    /// Toggles the overlay display and updates the power-up image.
    /// </summary>
    /// <param name="spritePowerUp">The sprite of the power-up to be displayed.</param>
    public void SwitchState(Sprite spritePowerUp)
    {
        overlay.enabled = !overlay.enabled;
        containerInfo.SetActive(!containerInfo.activeSelf);
        if (spritePowerUp != null)
            imageDisplay.sprite = spritePowerUp;
        GameManager.Instance.PowerUpActivate = !GameManager.Instance.PowerUpActivate;
        BoardManager.Instance.BoardCollider.enabled = !BoardManager.Instance.BoardCollider.enabled;
    }
}