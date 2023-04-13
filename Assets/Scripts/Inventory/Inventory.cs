using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public Dictionary<TypePowerUp, int> InventoryItems { get { return inventoryItems; } set { inventoryItems = value; } }

    [SerializeField] List<GameObject> powerUpsObject;

    Dictionary<TypePowerUp, int> inventoryItems = new Dictionary<TypePowerUp, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (TypePowerUp item in Enum.GetValues(typeof(TypePowerUp)))
        {
            inventoryItems.Add(item, 0);
        }
    }

    // Method in charge of updating the UI of powers up (Profile)
    public void UpdateInventoryUI(GameObject inventoryPanel)
    {
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

    public void SetTransformPowerUps(Transform transformPosition, Vector3 scale)
    {
        powerUpsObject.ForEach(powerUp =>
        {
            powerUp.transform.SetParent(transformPosition, false);
            powerUp.transform.localScale = scale;
        }
    );
    }
}