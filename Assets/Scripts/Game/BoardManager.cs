using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton
    public static BoardManager Instance;

    // Event that notifies us when there are changes in the board
    public static Action OnBoardChanges;

    // minimum number of fruits to combine including the current one
    public const int MinFruitsToMatch = 3;

    // Check if a fruit is changing
    public bool IsShifting { get; set; }
    public int XSize { get { return xSize; } }
    public int YSize { get { return ySize; } }
    public GameObject[,] Fruits { get { return fruits; } set { fruits = value; } }
    // Variable to which the score is added and then multiplied by the multiplication factor
    public int SumScore { get; set; }

    [Tooltip("All prefabs fruits")]
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();
    [Tooltip("Reference fruit")]
    [SerializeField] GameObject currentFruit;
    [Tooltip("Board size in columns(y) and Rows(x)")]
    [SerializeField] int xSize, ySize; // Board size
    [Tooltip("Where the first fruit appears")]
    [SerializeField] Transform spawnFruit;
    [SerializeField] float delayToCreateFruit;
    [SerializeField] int score;
    [Tooltip("Probability of each fruit to appear")]
    [SerializeField] int[] fruitsProbabilities;
    [SerializeField] AudioClip endSwapFruitAudio;

    // All the fruits on the board
    GameObject[,] fruits;
    // List<GameObject> check = new List<GameObject>();
    Collider2D boardCollider;

    Fruit selectedFruit;

    AudioSource audioSource;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.6f;

    int totalProbabilities;

    // List of fruits that were changed position when there was a match
    List<GameObject> fruitsWereMoved;

    void OnEnable()
    {
        // OnBoardChanges += StartCoroutineFindDisable;
    }

    void OnDisable()
    {
        // OnBoardChanges -= StartCoroutineFindDisable;
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

        foreach (int i in fruitsProbabilities)
        {
            totalProbabilities += i;
        }

        boardCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    void Start()
    {
        StartCoroutine(CreateInitialBoard());
    }

    int SetFruitProbability()
    {
        int accumulatedProbability = 0;
        int randomNumber = UnityEngine.Random.Range(0, totalProbabilities + 1);
        int prefab = 0;

        foreach (int i in fruitsProbabilities)
        {
            if (randomNumber < accumulatedProbability + i)
            {
                return prefab;
            }

            accumulatedProbability += i;
            prefab++;
        }

        return 1;
    }

    // Create the initial elements or fruits of the board
    IEnumerator CreateInitialBoard(bool targetLevel = false)
    {
        fruits = new GameObject[xSize, ySize]; // Columns and rows of the board

        float startX = spawnFruit.position.x;
        float startY = spawnFruit.position.y;

        int idx = -1; // The initial value is temporary

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                yield return new WaitForSeconds(delayToCreateFruit);

                if (!targetLevel)
                {
                    do
                    {
                        // Change the "currentFruit" to a prefab made fruit randomly
                        idx = UnityEngine.Random.Range(0, prefabs.Count);
                    } while (NeighborsSameCandy(x, y, idx));

                }
                else
                {
                    idx = SetFruitProbability();
                }

                currentFruit = prefabs[idx];

                GameObject newFruit = Instantiate(currentFruit, spawnFruit);
                newFruit.transform.localPosition = new Vector2(x, y);

                // Add name to each fruit where we indicate in which column and row it is located
                newFruit.name = string.Format("Fruit[{0}] [{1}]", x, y);
                newFruit.GetComponent<Fruit>().Id = idx;

                Physics2D.Simulate(1);

                if (IsFruitTouchingTheBoard(newFruit))
                {
                    fruits[x, y] = newFruit; // Add fruit to the board
                    newFruit.transform.localPosition = new Vector2(x, y + 1);
                    newFruit.GetComponent<Fruit>().MoveFruit(new Vector2(x, y), false);
                }
                else AddFruitToPool(newFruit);
            }
        }

        boardCollider.enabled = false;
    }

    // Check if the fruit is on the table, if not, destroy it.
    bool IsFruitTouchingTheBoard(GameObject fruits) => boardCollider.IsTouching(fruits.GetComponent<Collider2D>());

    // Method in charge of verifying if the fruit is repeated in said column and row
    bool NeighborsSameCandy(int x, int y, int idx) => (x > 1 && fruits[x - 2, y] != null && idx == fruits[x - 2, y].GetComponent<Fruit>().Id) ||
                                                        (y > 1 && fruits[x, y - 2] != null && idx == fruits[x, y - 2].GetComponent<Fruit>().Id);

    /// <summary>
    /// Swaps the position of two fruit game objects in a given direction.
    /// </summary>
    /// <param name="fruit">The fruit game object to be swapped.</param>
    /// <param name="direction">The direction in which the swap is to be made.</param>
    public IEnumerator SwapFruit(GameObject fruit, Vector2 direction)
    {
        // Set IsShifting to true to indicate that a swap is in progress
        IsShifting = true;
        // Cast a ray in the given direction to find the next fruit to be swapped
        RaycastHit2D hit = Physics2D.Raycast(fruit.transform.position, direction);

        // If no fruit is found in the given direction, set IsShifting to false and return
        if (hit.collider == null)
        {
            BoardManager.Instance.IsShifting = false;
            yield break;
        }
        // Get the next fruit game object
        GameObject nextFruit = hit.collider.gameObject;

        // If the two fruits have the same ID, set IsShifting to false and return
        if (fruit.GetComponent<Fruit>().Id == nextFruit.GetComponent<Fruit>().Id)
        {
            BoardManager.Instance.IsShifting = false;
            yield break;
        }

        // Move the two fruits to each other's positions
        fruit.GetComponent<Fruit>().MoveFruit(nextFruit.transform.localPosition);
        nextFruit.GetComponent<Fruit>().MoveFruit(fruit.transform.localPosition);
        // Decrement the MoveCounter variable in the GUIManager instance
        GUIManager.Instance.MoveCounter--;

        yield return new WaitForSeconds(timeChangePositionFruits);
        // Boolean that tells us if there was a match or not
        bool matchesFound = ClearMatches(fruit, nextFruit);

        // MultiplicationFactor.Instance.SetMultiplicationFactor(); TODO:

        // TODO: If there are no matches found, return the fruits to their old position.

        // Set IsShifting to false to indicate that the swap is complete
        IsShifting = false;
    }

    /// <summary>
    /// Returns a list of matching fruits in the specified direction from the given fruit.
    /// </summary>
    /// <param name="fruit">The fruit to start the search from.</param>
    /// <param name="direction">The direction in which to search for matching fruits.</param>
    /// <returns>A list of matching fruits found in the specified direction.</returns>
    List<GameObject> GetMatchByDirection(GameObject fruit, Vector2 direction)
    {
        List<GameObject> fruitMatches = new List<GameObject>();
        fruitMatches.Add(fruit);

        Fruit currentFruit = fruit.GetComponent<Fruit>();
        RaycastHit2D hit = Physics2D.Raycast(fruit.transform.position, direction);

        while (hit.collider != null && hit.collider.gameObject.GetComponent<Fruit>().Id == currentFruit.Id)
        {
            fruitMatches.Add(hit.collider.gameObject);

            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }

        return fruitMatches;
    }

    /// <summary>
    /// Returns a list of matching fruits in all specified directions from the given fruit.
    /// </summary>
    /// <param name="fruit">The fruit to start the search from.</param>
    /// <param name="directions">An array of directions in which to search for matching fruits.</param>
    /// <returns>A list of all matching fruits found in the specified directions.</returns>
    List<GameObject> GetMatchesByDirections(GameObject fruit, Vector2[] directions)
    {
        List<GameObject> fruitList = new List<GameObject>();

        foreach (Vector2 direction in directions)
        {
            fruitList = (fruitList.Union(GetMatchByDirection(fruit, direction)).ToList());
        }

        return fruitList;
    }

    /// <summary>
    /// Determines if there are any matching fruits adjacent to the given fruit in both horizontal and vertical directions.
    /// </summary>
    /// <param name="fruit">The fruit to search for matches around.</param>
    /// <returns>A tuple containing a list of all matching fruits found adjacent to the given fruit and a boolean indicating whether matches were found.</returns>
    (List<GameObject> foundMatches, bool isMatchFound) ThereAreFoundMatches(GameObject fruit)
    {
        bool isMatchFound = false;
        List<GameObject> hMatches = new List<GameObject>();
        List<GameObject> vMatches = new List<GameObject>();

        // Search for horizontal matches
        hMatches = GetMatchesByDirections(fruit, new Vector2[2] { Vector2.left, Vector2.right });
        // Search for vertical matches
        vMatches = GetMatchesByDirections(fruit, new Vector2[2] { Vector2.up, Vector2.down });

        List<GameObject> combinedMatches = new List<GameObject>();
        // Combine the matches found in both directions
        if (hMatches.Count >= MinFruitsToMatch)
        {
            combinedMatches = combinedMatches.Union(hMatches).ToList();
            isMatchFound = true;
        }

        if (vMatches.Count >= MinFruitsToMatch)
        {
            combinedMatches = combinedMatches.Union(vMatches).ToList();
            isMatchFound = true;
        }

        return (combinedMatches, isMatchFound);
    }

    /// <summary>
    /// Clears any matches found between two given fruits by disabling their game objects.
    /// </summary>
    /// <param name="firstFruit">The first fruit to check for matches.</param>
    /// <param name="secondFruit">The second fruit to check for matches.</param>
    /// <returns>A boolean indicating whether any matches were found and cleared.</returns>
    bool ClearMatches(GameObject firstFruit, GameObject secondFruit)
    {
        (List<GameObject> firstMatches, bool isFirstMatchFound) = ThereAreFoundMatches(firstFruit);
        (List<GameObject> secondMatches, bool isSecondMatchFound) = ThereAreFoundMatches(secondFruit);

        if (isFirstMatchFound)
        {
            firstMatches.ForEach(matchedFruit =>
            {
                matchedFruit.GetComponent<Fruit>().DisableFruit();
            });
        }

        if (isSecondMatchFound)
        {
            secondMatches.ForEach(matchedFruit =>
            {
                matchedFruit.GetComponent<Fruit>().DisableFruit();
            });
        }

        return isFirstMatchFound || isSecondMatchFound;
    }

    // // Start the routine to find that the fruits are deactivated on the board
    // void StartCoroutineFindDisable()
    // {
    //     StopAllCoroutines();
    //     StartCoroutine(FindDisableFruits());
    // }

    // // Search in each row and column what space there is, that is, what fruit is deactivated
    // IEnumerator FindDisableFruits()
    // {
    //     // fruitsWereMoved = new List<GameObject>();

    //     yield return new WaitForEndOfFrame();

    //     for (int x = 0; x < xSize; x++)
    //     {
    //         for (int y = 0; y < ySize; y++)
    //         {
    //             if (fruits[x, y] != null && !fruits[x, y].gameObject.activeSelf)
    //             {
    //                 yield return StartCoroutine(CollapseFruits(x, y));
    //                 break;
    //             }
    //         }
    //     }

    //     audioSource.PlayOneShot(endSwapFruitAudio, 1);

    //     // We go through all the fruits to see if there is a match
    //     for (int i = 0; i < fruitsWereMoved.Count; i++)
    //     {
    //         if (fruitsWereMoved[i].activeSelf)
    //         {
    //             fruitsWereMoved[i].GetComponent<Fruit>().FindAllMatches();
    //         }
    //     }
    // }

    // // Makes the fruits fall to occupy an empty position
    // IEnumerator MakeFruitsFall(int x, int yStart, float shiftDelay = 0.1f)
    // {
    //     List<GameObject> boardFruits;
    //     int disabledFruits;

    //     CountDisableFruits(x, yStart, out boardFruits, out disabledFruits);

    //     bool secondTime = false; // Indicates if it is the second fruit that appears
    //     List<GameObject> listNewFruits = new List<GameObject>(); // Save the new fruits that appear

    //     for (int i = 0; i < disabledFruits; i++)
    //     {
    //         GUIManager.Instance.Score += score;
    //         SumScore += score;

    //         int y = yStart; // Traverse the rows of the board

    //         if (IsLastRow(disabledFruits, x, y))
    //         {
    //             LastRowOfFruits(x, y);
    //         }
    //         else // This is in case there are more fruits deactivated and they are in some row other than 7
    //         {
    //             for (int j = 0; j < boardFruits.Count - 1; j++)
    //             {
    //                 boardFruits[j + 1].GetComponent<Fruit>().MoveFruit(new Vector2(boardFruits[j + 1].transform.localPosition.x, boardFruits[j + 1].transform.localPosition.y - offset)); ;
    //                 fruits[x, y] = fruits[x, y + 1]; // Change the previously moved fruit to the corresponding position in the array

    //                 if (j == boardFruits.Count - 2)
    //                 {
    //                     fruits[x, y + 1] = GetNewFruit();
    //                     fruits[x, y + 1].transform.localPosition = new Vector2(x, y + 1);
    //                     fruits[x, y + 1].SetActive(true);
    //                     listNewFruits.Add(fruits[x, y + 1]);

    //                     if (secondTime)
    //                     {
    //                         for (int k = 0; k < listNewFruits.Count - 1; k++)
    //                         {
    //                             listNewFruits[k].GetComponent<Fruit>().MoveFruit(new Vector2(listNewFruits[k].transform.localPosition.x, listNewFruits[k].transform.localPosition.y - offset));
    //                         }
    //                     }
    //                     secondTime = true;
    //                 }
    //                 y++;
    //                 yield return new WaitForSeconds(shiftDelay);
    //             }
    //         }
    //     }

    //     fruitsWereMoved.AddRange(boardFruits);
    //     fruitsWereMoved.AddRange(listNewFruits);

    //     AddFruitsToPool(boardFruits);

    //     IsShifting = false;
    // }

    // void CountDisableFruits(int x, int yStart, out List<GameObject> boardFruits, out int disabledFruits)
    // {
    //     IsShifting = true;

    //     boardFruits = new List<GameObject>();
    //     disabledFruits = 0;

    //     for (int y = yStart; y < ySize; y++)
    //     {
    //         if (fruits[x, y] != null)
    //         {
    //             GameObject boardFruit = fruits[x, y];

    //             if (boardFruit.GetComponentInChildren<SpriteRenderer>().enabled == false)
    //             {
    //                 disabledFruits++;
    //             }

    //             boardFruits.Add(boardFruit);
    //         }
    //     }
    // }

    // // The method returns true if it is in the last row of each column
    // bool IsLastRow(int disabledFruits, int x, int y) => (disabledFruits <= 1 && y == YSize - 1) || (x == 0 && y == 6) || (x == 7 && y == 6);

    // void LastRowOfFruits(int x, int y)
    // {
    //     Vector3 fruitPosition = fruits[x, y].transform.position;
    //     fruits[x, y] = GetNewFruit();
    //     fruits[x, y].transform.position = fruitPosition;
    //     fruits[x, y].SetActive(true);
    // }

    // Deactivate the fruit that were eliminated and add to object pooler

    void AddFruitToPool(GameObject fruit)
    {
        fruit.SetActive(false);
        ObjectPooler.Instance.FruitList.Add(fruit);
        fruit.transform.SetParent(ObjectPooler.Instance.gameObject.transform);
    }

    // Deactivate the fruits that were eliminated and add to object pooler
    void AddFruitsToPool(List<GameObject> fruits)
    {
        fruits.ForEach(i =>
        {
            if (i.GetComponentInChildren<SpriteRenderer>().enabled == false)
            {
                i.SetActive(false);
                i.GetComponentInChildren<SpriteRenderer>().enabled = true;
                ObjectPooler.Instance.FruitList.Add(i);
                i.transform.parent = ObjectPooler.Instance.gameObject.transform;
            }
        });
    }

    // Generates a new fruit
    GameObject GetNewFruit()
    {
        int newFruit = UnityEngine.Random.Range(0, prefabs.Count);

        return ObjectPooler.Instance.GetFruitToPool(newFruit, spawnFruit.transform);
    }
}