using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LevelUI : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] GameObject overlay;

    [SerializeField] TMP_Text nameLevelText;
    [SerializeField] Image[] stars;
    [SerializeField] TMP_Text goal;
    [SerializeField] GameObject powerUpPanel;

    [SerializeField] GameObject boxSelectPower;

    GameMode gameMode;

    // Method to activate the level UI by setting the 'container' and 'overlay' game objects to active
    public void ActiveLevelUI()
    {
        container.SetActive(true);
        overlay.SetActive(true);
    }

    /// <summary>
    /// Sets the name, number of stars, and goal of the level in the UI.
    /// </summary>
    /// <param name="nameText">The level name.</param>
    /// <param name="amountStars">The number of stars collected.</param>
    /// <param name="goalText">The goal text.</param>
    /// <param name="gameMode">The game mode.</param>
    public void SetValueLevel(string nameText, int amountStars, string goalText, GameMode gameMode)
    {
        nameLevelText.text = nameText; // Sets the level name

        // Enables stars in the UI based on the number of stars collected
        for (int i = 0; i < amountStars; i++)
        {
            stars[i].enabled = true;
        }

        goal.text = goalText;  // Sets the goal text

        this.gameMode = gameMode;

        List<GameObject> powerUps = Inventory.Instance.GetAvailablePowerUps();

        // Updates the power-up panel in the UI with the player's inventory
        if (powerUps.Count > 0)
        {
            Inventory.Instance.SetAvailablePowerUps(powerUps, boxSelectPower.transform, StatePowerUp.LevelUI);
            Inventory.Instance.UpdateInventoryUI(powerUpPanel);
        }
    }

    /// <summary>
    /// Starts playing the specified level.
    /// </summary>
    /// <param name="nameScene">The name of the scene to load.</param>
    public void PlayLevel(string nameScene)
    {
        if (LifeController.Instance.HasLives || LifeController.Instance.IsInfinite)
        {
            GameManager.Instance.GameMode = gameMode;
            GameManager.Instance.ObjectiveComplete = false;
            GameManager.Instance.CurrentLevel = Convert.ToInt32(nameLevelText.text.Split(' ')[1]) - 1;
            StartCoroutine(ScreenChangeTransition.Instance.FadeOut(nameScene));
            StartCoroutine(ResetParentPowerUps());
        }
        else
        {
            LevelMenuController.Instance.OffScreen(LevelMenuController.Instance.BoxLevelUI, () =>
            {
                LevelMenuController.Instance.OnScreen(LevelMenuController.Instance.LifeShop);
            });
        }
    }

    /// <summary>
    /// Coroutine to delay resetting the parent of power-ups.
    /// </summary>
    /// <returns>An IEnumerator for use in StartCoroutine.</returns>
    IEnumerator ResetParentPowerUps()
    {
        yield return new WaitForSeconds(.8f);
        Inventory.Instance.ResetParentPowerUps(false);
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