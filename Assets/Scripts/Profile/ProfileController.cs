using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    private void OnEnable()
    {
        Inventory.Instance.UpdateInventoryUI();
    }
}
