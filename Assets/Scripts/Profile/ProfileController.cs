using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    // Declaration of the 'inventoryPanel' variable of the 'GameObject' data type
    [SerializeField] GameObject inventoryPanel;

    // Vector3 variable to store the scale of power-ups
    Vector3 scalePowerUps = new Vector3(1, 1, 1);

    private void OnEnable()
    {
        // If power-ups have been moved in the inventory, update the inventory UI with the updated power-ups
        if (Inventory.Instance.PowerUpsWereMoved)
        {
            Inventory.Instance.SetAvailablePowerUps(Inventory.Instance.GetAvailablePowerUps(), inventoryPanel.transform, scalePowerUps);
            Inventory.Instance.OrderPowerUps(inventoryPanel.transform);
            Inventory.Instance.PowerUpsWereMoved = false;
        }

        Inventory.Instance.UpdateInventoryUI(inventoryPanel);
    }
}