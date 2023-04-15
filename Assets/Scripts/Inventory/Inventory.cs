using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public Dictionary<TypePowerUp, int> InventoryItems { get { return inventoryItems; } set { inventoryItems = value; } }

    public bool PowerUpsWereMoved { get { return powerUpsWereMoved; } set { powerUpsWereMoved = value; } }

    // List of all available power-ups as game objects
    [SerializeField] List<GameObject> powerUpsObject;

    // Dictionary that stores the amount of each power-up the player has
    Dictionary<TypePowerUp, int> inventoryItems = new Dictionary<TypePowerUp, int>();

    // Flag that indicates if power-ups were moved in the UI
    bool powerUpsWereMoved;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Initialize the inventoryItems dictionary with each power-up type
        foreach (TypePowerUp item in Enum.GetValues(typeof(TypePowerUp)))
        {
            inventoryItems.Add(item, 0);
        }
    }

    /// <summary>
    /// Method in charge of updating the UI of powers up (Profile)
    /// </summary>
    /// <param name="inventoryPanel">The game object representing the inventory panel in the UI</param>
    public void UpdateInventoryUI(GameObject inventoryPanel)
    {
        // Update the text of each power-up in the UI based on the number of each power-up the player has
        foreach (Transform child in inventoryPanel.transform)
        {
            PowerUp powerUp = child.GetComponent<PowerUp>();

            if (inventoryItems.ContainsKey(powerUp.TypePowerUp))
            {
                powerUp.TextAmount.text = inventoryItems[powerUp.TypePowerUp].ToString();
            }
            else
            {
                powerUp.TextAmount.text = "0";
            }
        }
    }

    /// <summary>
    /// Gets a list of available power-ups the player has in their inventory
    /// </summary>
    /// <returns>A list of game objects representing the available power-ups in the inventory</returns>
    public List<GameObject> GetAvailablePowerUps()
    {
        List<GameObject> listPowerUps = new List<GameObject>();

        // Check each power-up type in the inventory and add it to the list if the player has at least one or if it is an infinite power-upx
        foreach (KeyValuePair<TypePowerUp, int> item in inventoryItems)
        {
            GameObject powerUp = powerUpsObject.Find(powerUp => powerUp.GetComponent<PowerUp>().TypePowerUp == item.Key);

            if (item.Value != 0 || powerUp.GetComponent<PowerUp>().IsInfinite)
            {
                listPowerUps.Add(powerUp);
            }
        }

        return listPowerUps;
    }

    /// <summary>
    /// Sets the parent and scale of each power-up game object in a list
    /// </summary>
    /// <param name="powerUps">The list of power-up game objects to set the parent and scale of</param>
    /// <param name="parent">The transform representing the parent object in the UI</param>
    /// <param name="statePowerUp">The state of the power up that is going to change</param>
    public void SetAvailablePowerUps(List<GameObject> powerUps, Transform parent, StatePowerUp statePowerUp)
    {
        // Set the parent and scale of each power-up game object in the list
        powerUps.ForEach(powerUp =>
        {
            powerUp.GetComponent<PowerUp>().StatePowerUp = statePowerUp;
            powerUp.transform.SetParent(parent.transform, false);
        });
    }

    /// <summary>
    /// Orders the power-up game objects under the given parent transform based on their TypePowerUp enum value.
    /// </summary>
    /// <param name="parent">The parent transform of the power-up game objects to be ordered.</param>
    public void OrderPowerUps(Transform parent)
    {
        List<PowerUp> powerUps = new List<PowerUp>();

        for (int i = 0; i < parent.childCount; i++)
        {
            powerUps.Add(parent.GetChild(i).GetComponent<PowerUp>());
        }

        powerUps.ForEach(powerUp =>
        {
            int indexPowerUp = Convert.ToInt32(powerUp.TypePowerUp);
            powerUp.transform.SetSiblingIndex(indexPowerUp);
        });
    }
}