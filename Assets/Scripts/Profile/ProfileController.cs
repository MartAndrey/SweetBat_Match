using System;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    // Declaration of the 'inventoryPanel' variable of the 'GameObject' data type
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] AvatarController avatar;

    void OnEnable()
    {
        UpdateProfileData();
    }

    void OnDisable()
    {
        Inventory.Instance.ResetParentPowerUps(false);
        // Iterate through power-ups to check if they are in transition and disable descriptions.
        Inventory.Instance.PowerUpsObject.ForEach(powerUp =>
        {
            PowerUp power = powerUp.GetComponent<PowerUp>();

            if (power.IsInTransitionDescription)
                power.DisableDescription();
        });
    }

    /// <summary>
    /// Updates the profile data, including power-up inventory and avatar.
    /// </summary>
    void UpdateProfileData()
    {
        // update the inventory UI with the updated power-ups
        Inventory.Instance.SetAvailablePowerUps(Inventory.Instance.PowerUpsObject, inventoryPanel.transform, StatePowerUp.Profile);
        Inventory.Instance.OrderPowerUps(inventoryPanel.transform);

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