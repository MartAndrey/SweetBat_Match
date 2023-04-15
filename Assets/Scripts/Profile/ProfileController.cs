using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    // Declaration of the 'inventoryPanel' variable of the 'GameObject' data type
    [SerializeField] GameObject inventoryPanel;

    private void OnEnable()
    {
        // If power-ups have been moved in the inventory, update the inventory UI with the updated power-ups
        if (Inventory.Instance.PowerUpsWereMoved)
        {
            Inventory.Instance.SetAvailablePowerUps(Inventory.Instance.GetAvailablePowerUps(), inventoryPanel.transform, StatePowerUp.Profile);
            Inventory.Instance.OrderPowerUps(inventoryPanel.transform);
            Inventory.Instance.PowerUpsWereMoved = false;
        }

        Inventory.Instance.UpdateInventoryUI(inventoryPanel);
    }
}