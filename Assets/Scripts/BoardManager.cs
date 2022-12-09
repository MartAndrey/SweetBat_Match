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
        fruits = new GameObject[xSize, ySize]; // Columns and rows of the board

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newFruit = Instantiate(currentFruit, new Vector3(
                                                                startX + (offset.x * x),
                                                                startY + (offset.y * y),
                                                                0),
                                                                currentFruit.transform.rotation);

                // Add name to each fruit where we indicate in which column and row it is located
                newFruit.name = string.Format("Fruit[{0}] [{1}]", x, y);

                fruits[x, y] = newFruit; // Add fruit to the board
            }
        }
    }
}