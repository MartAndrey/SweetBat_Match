using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // Check if a fruit is changing
    public bool isShifting { get; set; }

    [Tooltip("All prefabs fruits")]
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();
    [Tooltip("Reference fruit")]
    [SerializeField] GameObject currentFruit;
    [Tooltip("Board size in columns(y) and Rows(x)")]
    [SerializeField] int xSize, ySize; // Board size
    [Tooltip("Where the first fruit appears")]
    [SerializeField] Transform spawnFruit;

    // All the fruits on the board
    GameObject[,] fruits;
    Collider2D boardCollider;

    // Variable that gives the distance of each fruit on the board
    float offset = 1;

    // Variable in charge of returning true when all the fruits on the board are reviewed 
    bool checkFruits = false;

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

        boardCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        CreateInitialBoard();
    }

    void Update()
    {
        if (!checkFruits)
            IsFruitTouchingTheBoard(fruits);
    }

    // Create the initial elements or fruits of the board
    void CreateInitialBoard()
    {
        fruits = new GameObject[xSize, ySize]; // Columns and rows of the board

        float startX = spawnFruit.position.x;
        float startY = spawnFruit.position.y;

        int idx = -1; // The initial value is temporary

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                do
                {
                    // Change the "currentFruit" to a prefab made fruit randomly
                    idx = Random.Range(0, prefabs.Count);
                } while (NeighborsSameCandy(x, y, idx));

                // int random = Random.Range(0, prefabs.Count);
                currentFruit = prefabs[idx];

                GameObject newFruit = Instantiate(currentFruit, new Vector3(
                                                                startX + (offset * x),
                                                                startY + (offset * y),
                                                                0),
                                                                currentFruit.transform.rotation, spawnFruit);

                // Add name to each fruit where we indicate in which column and row it is located
                newFruit.name = string.Format("Fruit[{0}] [{1}]", x, y);
                newFruit.GetComponent<Fruit>().Id = idx;

                fruits[x, y] = newFruit; // Add fruit to the board
            }
        }
    }

    // Check if the fruit is on the table, if not, destroy it.
    void IsFruitTouchingTheBoard(GameObject[,] fruits)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Collider2D currentFruit = fruits[x, y].GetComponent<Collider2D>();

                if (!boardCollider.IsTouching(currentFruit))
                {
                    Destroy(currentFruit.gameObject);
                }
            }
        }

        checkFruits = true;
    }

    // Method in charge of verifying if the fruit is repeated in said column and row
    bool NeighborsSameCandy(int x, int y, int idx) => (x > 1 && idx == fruits[x - 2, y].GetComponent<Fruit>().Id) ||
                                                        (y > 1 && idx == fruits[x, y - 2].GetComponent<Fruit>().Id);
}