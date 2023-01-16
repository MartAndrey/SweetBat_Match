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
    [SerializeField] int score;

    // All the fruits on the board
    GameObject[,] fruits;
    // List<GameObject> check = new List<GameObject>();
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
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (fruits[x, y] != null && !fruits[x, y].activeSelf)
                {
                    yield return StartCoroutine(MakeFruitsFall(x, y));
                    break;
                }
            }
        }

        // CheckMatch(check);
    }

    // Makes the fruits fall to occupy an empty position
    IEnumerator MakeFruitsFall(int x, int yStart)
    {
        isShifting = true;

        List<GameObject> boardFruits = new List<GameObject>();
        int disabledFruits = 0;

        for (int y = yStart; y < ySize; y++)
        {
            if (fruits[x, y] != null)
            {
                GameObject boardFruit = fruits[x, y];

                if (!boardFruit.activeSelf)
                {
                    disabledFruits++;
                    AddFruitsToPool(boardFruit);
                }

                boardFruits.Add(boardFruit);
            }
        }

        bool secondTime = false; // Indicates if it is the second fruit that appears
        List<GameObject> listNewFruit = new List<GameObject>(); // Save the new fruits that appear
        Vector2 positionNewFruit = new Vector2(); // Save the position of the new fruit

        for (int i = 0; i < disabledFruits; i++)
        {
            GUIManager.Instance.Score += score;

            int y = yStart; // Traverse the rows of the board

            for (int j = 0; j < boardFruits.Count - 1; j++)
            {
                //string nameFruit = fruits[x, y].name;
                boardFruits[j + 1].GetComponent<Fruit>().TargetPosition = new Vector2(boardFruits[j + 1].transform.position.x, boardFruits[j + 1].transform.position.y - offset);
                fruits[x, y] = fruits[x, y + 1]; // Change the previously moved fruit to the corresponding position in the array
                //fruits[x, y].name = nameFruit;
                if (j == boardFruits.Count - 2)
                {
                    fruits[x, y + 1] = GetNewFruit(x, ySize - 1); // Generates a new fruit
                    listNewFruit.Add(fruits[x, y + 1]); // We add the new fruit to the list

                    yield return new WaitForSeconds(timeChangePositionFruits);

                    // We save the position of the new fruit, where it always has to appear when instantiated
                    if (positionNewFruit == Vector2.zero)
                    {
                        positionNewFruit = new Vector2(fruits[x, y].transform.position.x, fruits[x, y].transform.position.y + offset);
                        fruits[x, y + 1].transform.position = positionNewFruit;
                    }
                    else
                    {
                        fruits[x, y + 1].transform.position = positionNewFruit;
                    }

                    fruits[x, y + 1].SetActive(true);

                    // If it is the second fruit that appears, we start by moving the new fruits one position down
                    // In case it is just a new fruit that appears, we simply leave it in the same position
                    if (secondTime)
                    {
                        for (int k = 0; k < listNewFruit.Count - 1; k++)
                        {
                            listNewFruit[k].transform.position = new Vector2(listNewFruit[k].transform.position.x, listNewFruit[k].transform.position.y - offset);
                        }
                    }
                    secondTime = true;
                }
                y++;
            }
        }

        // check.AddRange(boardFruits);
        // check.AddRange(listNewFruit);

        isShifting = false;
    }

    // void CheckMatch(List<GameObject> fruit)
    // {
    //     for (int i = 0; i < fruit.Count; i++)
    //     {
    //         if (fruit[i].activeSelf)
    //         {
    //             Debug.Log("Hi");
    //             fruit[i].GetComponent<Fruit>().FindAllMatches();
    //         }
    //     }

    //     check.Clear();
    // }

    // Deactivate the fruits that were eliminated and add to object pooler
    void AddFruitsToPool(GameObject fruits)
    {
        // Add to object pooler
        ObjectPooler.Instance.FruitList.Add(fruits);
        fruits.transform.parent = ObjectPooler.Instance.gameObject.transform;
    }

    // Generates a new fruit
    GameObject GetNewFruit(int x, int y)
    {
        int newFruit = Random.Range(0, prefabs.Count);

        return ObjectPooler.Instance.GetFruitToPool(newFruit);
    }
}