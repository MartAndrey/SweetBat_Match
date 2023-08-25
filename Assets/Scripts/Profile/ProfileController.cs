using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    // Declaration of the 'inventoryPanel' variable of the 'GameObject' data type
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] AvatarController avatar;

    private void OnEnable()
    {
        UpdateProfileData();
    }

    /// <summary>
    /// Updates the profile data, including power-up inventory and avatar.
    /// </summary>
    void UpdateProfileData()
    {
        // If power-ups have been moved in the inventory, update the inventory UI with the updated power-ups
        if (Inventory.Instance.PowerUpsWereMoved)
        {
            Inventory.Instance.SetAvailablePowerUps(Inventory.Instance.GetAvailablePowerUps(), inventoryPanel.transform, StatePowerUp.Profile);
            Inventory.Instance.OrderPowerUps(inventoryPanel.transform);
            Inventory.Instance.PowerUpsWereMoved = false;
        }

        // Update inventory UI
        Inventory.Instance.UpdateInventoryUI(inventoryPanel);

        // Update avatar information based on user type
        if (!FirebaseApp.Instance.User.IsAnonymous)
        {
            GenderUser genderUser = (GenderUser)Enum.Parse(typeof(GenderUser), GameManager.Instance.UserData["gender"].ToString());
            Sprite userPhoto = GameManager.Instance.UserPhoto;
            avatar.UpdateAvatar(genderUser, userPhoto);
        }
        else avatar.UpdateAvatarAnonymous();
    }
}