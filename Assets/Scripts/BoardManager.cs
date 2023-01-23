using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // Event that notifies us when there are changes in the board
    public static Action OnBoardChanges;

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

    void OnEnable()
    {
        OnBoardChanges += StartCoroutineFindDisable;
    }

    void OnDisable()
    {

        OnBoardChanges -= StartCoroutineFindDisable;
    }

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

        StartCoroutine(IsFruitTouchingTheBoard(fruits));

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
                    idx = UnityEngine.Random.Range(0, prefabs.Count);
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
    IEnumerator IsFruitTouchingTheBoard(GameObject[,] fruits)
    {
        List<GameObject> noTouchingFruits = new List<GameObject>();

        yield return new WaitForSeconds(0.00f);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Collider2D currentFruit = fruits[x, y].GetComponent<Collider2D>();

                if (!boardCollider.IsTouching(currentFruit))
                {
                    currentFruit.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                    noTouchingFruits.Add(currentFruit.gameObject);
                    fruits[x, y] = null;
                }
            }
        }

        AddFruitsToPool(noTouchingFruits);
        boardCollider.enabled = false;
    }

    // Method in charge of verifying if the fruit is repeated in said column and row
    bool NeighborsSameCandy(int x, int y, int idx) => (x > 1 && idx == fruits[x - 2, y].GetComponent<Fruit>().Id) ||
                                                        (y > 1 && idx == fruits[x, y - 2].GetComponent<Fruit>().Id);

    // Start the routine to find that the fruits are deactivated on the board
    void StartCoroutineFindDisable()
    {
        StopCoroutine(FindDisableFruits());
        StartCoroutine(FindDisableFruits());
    }

    // Search in each row and column what space there is, that is, what fruit is deactivated
    public IEnumerator FindDisableFruits()
    {
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
    IEnumerator MakeFruitsFall(int x, int yStart)
    {
        List<GameObject> boardFruits;
        int disabledFruits;

        CountDisableFruits(x, yStart, out boardFruits, out disabledFruits);

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

                y++;
                yield return new WaitForSeconds(timeChangePositionFruits);
            }
        }

        AddFruitsToPool(boardFruits);

        isShifting = false;
    }

    private void CountDisableFruits(int x, int yStart, out List<GameObject> boardFruits, out int disabledFruits)
    {
        isShifting = true;

        boardFruits = new List<GameObject>();
        disabledFruits = 0;

        for (int y = yStart; y < ySize; y++)
        {
            if (fruits[x, y] != null)
            {
                GameObject boardFruit = fruits[x, y];

                if (boardFruit.GetComponentInChildren<SpriteRenderer>().enabled == false)
                {
                    disabledFruits++;
                }

                boardFruits.Add(boardFruit);
            }
        }
    }

    // Deactivate the fruits that were eliminated and add to object pooler
    void AddFruitsToPool(List<GameObject> fruits)
    {
        fruits.ForEach(i =>
        {
            if (i.GetComponentInChildren<SpriteRenderer>().enabled == false)
            {
                i.SetActive(false);
                ObjectPooler.Instance.FruitList.Add(i);
                i.transform.parent = ObjectPooler.Instance.gameObject.transform;
            }
        });
    }

    // Generates a new fruit
    GameObject GetNewFruit(int x, int y)
    {
        int newFruit = UnityEngine.Random.Range(0, prefabs.Count);

        return ObjectPooler.Instance.GetFruitToPool(newFruit, spawnFruit.transform);
    }
}