using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] GameObject overlay;

    [SerializeField] TMP_Text nameLevelText;
    [SerializeField] Image[] stars;
    [SerializeField] TMP_Text goal;
    [SerializeField] GameObject powerUpPanel;

    [SerializeField] GameObject boxSelectPower;

    // Method to activate the level UI by setting the 'container' and 'overlay' game objects to active
    public void ActiveLevelUI()
    {
        container.SetActive(true);
        overlay.SetActive(true);
    }

    // Method to set the name, number of stars, and goal of the level in the UI
    public void SetValueLevel(string nameText, int amountStars, string goalText)
    {
        nameLevelText.text = nameText; // Sets the level name

        // Enables stars in the UI based on the number of stars collected
        for (int i = 0; i < amountStars; i++)
        {
            stars[i].enabled = true;
        }

        goal.text = goalText;  // Sets the goal text

        List<GameObject> powerUps = Inventory.Instance.GetAvailablePowerUps();

        // Updates the power-up panel in the UI with the player's inventory
        if (powerUps.Count > 0)
        {
            Inventory.Instance.SetAvailablePowerUps(powerUps, boxSelectPower.transform, StatePowerUp.LevelUI);
            Inventory.Instance.UpdateInventoryUI(powerUpPanel);
            Inventory.Instance.PowerUpsWereMoved = true;
        }
    }

    public void PlayLevel(string nameScene)
    {
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(nameScene));
    }

    /// <summary>
    /// Resets the information of a level by disabling stars.
    /// </summary>
    public void ResetInformationLevel()
    {
        for (int i = 0; i < 3; i++)
        {
            stars[i].enabled = false;
        }
    }
}