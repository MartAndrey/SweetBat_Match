using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a set of game modes for different game objectives
public enum GameMode { FeedingObjective, ScoringObjective, TimeObjective, CollectionObjective }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    public int Level { get { return level; } }  // Public getter for the current level
    public GameMode TypeOfGame { get { return typeOfGame; } set { typeOfGame = value; } }

    // Serialized game mode field
    [SerializeField] GameMode typeOfGame;

    [SerializeField] int level = 0; // Private field for the current level

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Increases the level count and starts the next level routine.
    /// </summary>
    public void NextLevel()
    {
        // Increases the level count
        level++;
        // Starts the next level routine
        StartCoroutine(NextLevelRutiner());
    }

    /// <summary>
    /// Waits for 2 seconds and finds the LevelManager object.
    /// </summary>
    IEnumerator NextLevelRutiner()
    {
        yield return new WaitForSeconds(2);
        // Finds the LevelManager object
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.NextLevel();
    }
}