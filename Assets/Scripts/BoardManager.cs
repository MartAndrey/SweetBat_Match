using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // minimum number of fruits to combine including the current one
    public const int MinFruitsToMatch = 2;

    // Check if a fruit is changing
    public bool isShifting { get; set; }
    public int XSize { get { return xSize; } }
    public int YSize { get { return ySize; } }

    public GameObject[,] Fruits { get { return fruits; } set { fruits = value; } }

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

    Fruit selectedFruit;

    // Variable that gives the distance of each fruit on the board
    float offset = 1;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.2f;

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
        {
            IsFruitTouchingTheBoard(fruits);
            boardCollider.enabled = false;
        }
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

    // Search in each row and column what space there is, that is, what fruit is deactivated
    public IEnumerator FindDisableFruits()
    {
        yield return new WaitForSeconds(timeChangePositionFruits); // Time that waits for it to finish changing the position of the fruits

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (fruits[x, y] != null && fruits[x, y].GetComponentInChildren<SpriteRenderer>().enabled == false)
                {
                    yield return StartCoroutine(MakeFruitsFall(x, y));
                    break;
                }
            }
        }
    }

    // Makes the fruits fall to occupy an empty position
    IEnumerator MakeFruitsFall(int x, int yStart, float shiftDelay = 0.1f)
    {
        isShifting = true;

        List<GameObject> boardFruits = new List<GameObject>();
        int disabledFruits = 0;

        for (int y = yStart; y < ySize; y++)
        {
            if (fruits[x, y] != null && fruits[x, y].activeSelf)
            {
                GameObject boardFruit = fruits[x, y];

                if (boardFruit.GetComponentInChildren<SpriteRenderer>().enabled == false)
                {
                    disabledFruits++;
                }

                boardFruits.Add(boardFruit);
            }
        }

        for (int i = 0; i < disabledFruits; i++)
        {
            int y = yStart; // Traverse the rows of the board

            yield return new WaitForSeconds(shiftDelay);

            for (int j = 0; j < boardFruits.Count - 1; j++)
            {
                //string nameFruit = fruits[x, y].name;
                boardFruits[j + 1].GetComponent<Fruit>().TargetPosition = boardFruits[j].transform.position; // Move the fruit down
                fruits[x, y] = fruits[x, y + 1]; // Change the previously moved fruit to the corresponding position in the array
                //fruits[x, y].name = nameFruit;
                if (j == boardFruits.Count - 2) 
                    fruits[x, y + 1] = null; // Generates a new fruit

                y++;
            }
        }

        DisableFruits(boardFruits);

        isShifting = false;
    }

    // Deactivate the fruits that were eliminated
    void DisableFruits(List<GameObject> fruits)
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruits[i].GetComponentInChildren<SpriteRenderer>().enabled == false)
            {
                fruits[i].SetActive(false);
            }
        }
    }

    // Generates a new fruit
    GameObject GetNewFruit(int x, int y)
    {
        List<GameObject> possibleFruits = new List<GameObject>();
        possibleFruits.AddRange(prefabs);

        if (x > 0)
            possibleFruits.Remove(fruits[x - 1, y]);

        if (x < xSize - 1)
            possibleFruits.Remove(fruits[x + 1, y]);

        if (y > 0)
            possibleFruits.Remove(fruits[x, y - 1]);

        return possibleFruits[Random.Range(0, possibleFruits.Count)];
    }
}