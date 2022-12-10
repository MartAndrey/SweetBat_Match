using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // Check if a fruit is changing
    public bool isShifting { get; set; }

    [SerializeField] List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] GameObject currentFruit;
    [SerializeField] int xSize, ySize; // Board size

    GameObject[,] fruits;
    
    // Variable that gives the distance of each fruit on the board
    float offset = 1;

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
        CreateInitialBoard();
    }

    // Create the initial elements or fruits of the board
    void CreateInitialBoard()
    {
        fruits = new GameObject[xSize, ySize]; // Columns and rows of the board

        float startX = transform.position.x;
        float startY = transform.position.y;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newFruit = Instantiate(currentFruit, new Vector3(
                                                                startX + (offset * x),
                                                                startY + (offset * y),
                                                                0),
                                                                currentFruit.transform.rotation);

                // Add name to each fruit where we indicate in which column and row it is located
                newFruit.name = string.Format("Fruit[{0}] [{1}]", x, y);
                newFruit.transform.parent = transform;

                fruits[x, y] = newFruit; // Add fruit to the board
            }
        }
    }
}