using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    public int Level { get { return level; } }  // Public getter for the current level

    int level = 0; // Private field for the current level

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}