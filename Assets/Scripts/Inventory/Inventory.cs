using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public Dictionary<TypePowerUp, int> InventoryItems { get { return inventoryItems; } set { inventoryItems = value; } }
    public List<GameObject> PowerUpsObject { get { return powerUpsObject; } set { powerUpsObject = value; } }

    // List of all available power-ups as game objects
    [SerializeField] List<GameObject> powerUpsObject;

    // Dictionary that stores the amount of each power-up the player has
    Dictionary<TypePowerUp, int> inventoryItems = new Dictionary<TypePowerUp, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GetUserPowerUps();
    }

    /// <summary>
    /// Retrieves user power-ups and initializes them if they exist; otherwise, creates initial power-ups.
    /// </summary>
    void GetUserPowerUps()
    {
        Dictionary<string, object> data = GameManager.Instance.CollectiblesData;

        // If power-ups exist, set them; otherwise, create initial power-ups
        if (data != null && data.Count > 0)
        {
            if (data.ContainsKey("power ups"))
            {
                Dictionary<string, object> powerUpsData = data["power ups"] as Dictionary<string, object>;

                SetCollectibles(false, powerUpsData);

                return;
            }
        }

        SetCollectibles(true);
    }

    /// <summary>
    /// Sets the user's collectibles based on the provided data or initializes them if setDataBase is true.
    /// </summary>
    /// <param name="setDataBase">Whether to initialize the collectibles in the database.</param>
    /// <param name="powerUpsData">The data of the user's collectibles.</param>
    void SetCollectibles(bool setDataBase, Dictionary<string, object> powerUpsData = null)
    {
        if (powerUpsData != null && !setDataBase)
        {
            // Set existing power-ups from the provided data
            foreach (KeyValuePair<string, object> item in powerUpsData)
            {
                Dictionary<string, object> powerUp = item.Value as Dictionary<string, object>;

                if (Enum.TryParse(item.Key, out TypePowerUp type))
                {
                    // Update the inventory with the amount from the data
                    inventoryItems.Add(type, Convert.ToInt32(powerUp["amount"]));

                    powerUpsObject.ForEach(powerUpObject =>
                    {
                        PowerUp power = powerUpObject.GetComponent<PowerUp>();

                        if (power.TypePowerUp == type && Convert.ToInt32(powerUp["time"]) != 0)
                        {
                            StartCoroutine(InternetTime.Instance.GetInternetTime(time =>
                            {
                                power.SetTimerInfinite(time, (Timestamp)powerUp["last date"], Convert.ToInt32(powerUp["time"]));
                            }));
                        }
                    });
                }
            }

            return;
        }

        if (setDataBase)
        {
            // Initialize the inventoryItems dictionary with each power-up type
            foreach (TypePowerUp item in Enum.GetValues(typeof(TypePowerUp)))
            {
                inventoryItems.Add(item, 0);
            }

            Dictionary<string, object> data = new Dictionary<string, object>();

            // Prepare data for storing in the database
            foreach (KeyValuePair<TypePowerUp, int> item in inventoryItems)
            {

                Dictionary<string, object> subData = new Dictionary<string, object> { { "amount", item.Value }, { "time", 0 }, { "last date", DateTime.Now } };
                data.Add(item.Key.ToString(), subData);
            }

            SaveDataBase(data);
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
    /// Resets the parent of power-ups and optionally changes their checked status.
    /// </summary>
    /// <param name="changeCheck">Flag to change the check status of power-ups.</param>
    public void ResetParentPowerUps(bool changeCheck)
    {
        foreach (GameObject powerUp in powerUpsObject)
        {
            // Reset the parent of each power-up to this game object.
            powerUp.transform.SetParent(gameObject.transform, false);

            PowerUp power = powerUp.GetComponent<PowerUp>();

            if (changeCheck)
            {
                // Set the checked status of power-ups based on the 'changeCheck' flag.
                power.IsChecked = false;
            }

            if (power.IsCooldown)
                power.ResetCoolDownUI();
        }
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

    /// <summary>
    /// Saves the provided data to the database.
    /// </summary>
    /// <param name="data">The data to be saved.</param>
    public void SaveDataBase(Dictionary<string, object> data)
    {
        CloudFirestore.Instance.SetCollectible(new Dictionary<string, object> { { "power ups", data } });
    }

    /// <summary>
    /// Sets the power-up game state and updates the UI.
    /// </summary>
    public void SetPowerUpGame()
    {
        // Create a list to store selected power-ups.
        List<GameObject> listPowerUps = new();
        GameObject containerGame = GameObject.FindGameObjectWithTag("Container Power Game");

        if (containerGame != null)
        {
            // Iterate through power-ups to find and process checked ones.
            powerUpsObject.ForEach(powerUp =>
            {
                PowerUp power = powerUp.GetComponent<PowerUp>();

                if (power.IsChecked)

                {
                    listPowerUps.Add(power.gameObject);
                    power.ChangeCheckPowerUpUI(false);
                }
            });

            // If there are selected power-ups, set them as available in the game container.
            if (listPowerUps.Count >= 1)
                SetAvailablePowerUps(listPowerUps, containerGame.transform, StatePowerUp.Game);
        }
    }
}