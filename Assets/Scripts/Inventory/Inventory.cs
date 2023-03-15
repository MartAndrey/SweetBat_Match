using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    Dictionary<TypePowerUp, int> inventory = new Dictionary<TypePowerUp, int>();

    [SerializeField] GameObject inventoryPanel;

    void Start()
    {
        foreach (TypePowerUp item in Enum.GetValues(typeof(TypePowerUp)))
        {
            inventory.Add(item, 0);
        }

        UpdateInventoryUI();
    }
    
    void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            PowerUp powerUp = child.GetComponent<PowerUp>();

            if (inventory.ContainsKey(powerUp.TypePowerUp))
            {
                powerUp.TextAmount.text = inventory[powerUp.TypePowerUp].ToString();
            }
            else
            {
                powerUp.TextAmount.text = "0";
            }
        }
    }

    void AddPowerUp(TypePowerUp powerUp)
    {

    }

    void RemovePowerUp(TypePowerUp powerUp)
    {

    }
}