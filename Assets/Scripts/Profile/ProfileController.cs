using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    // Declaration of the 'inventoryPanel' variable of the 'GameObject' data type
    [SerializeField] GameObject inventoryPanel;

    private void OnEnable()
    {
        // Updates the inventory UI using the 'UpdateInventoryUI' method from the 'Inventory' class
        Inventory.Instance.SetTransformPowerUps(inventoryPanel.transform, new Vector3(1, 1, 1));
        Inventory.Instance.UpdateInventoryUI(inventoryPanel);
    }
}