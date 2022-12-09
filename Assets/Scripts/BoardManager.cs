using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // Check if a fruit is changing
    public bool isShifting { get; set; }

    [SerializeField] List<Sprite> prefabs = new List<Sprite>();
    [SerializeField] GameObject currentFruit;
    [SerializeField] int xSize, ySize; // Board size

    GameObject[,] fruits;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Vector2 offset = currentFruit.GetComponent<BoxCollider2D>().size;

        CreateInitialBoard(offset);
    } 

    // Create the initial elements or fruits of the board
    void CreateInitialBoard(Vector2 offset)
    {

    }
}