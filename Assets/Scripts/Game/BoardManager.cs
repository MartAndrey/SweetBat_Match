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
    [Header("Audio")]
    [SerializeField] AudioClip swapFruitAudio;
    [SerializeField] AudioClip fruitCrackAudio;
    [SerializeField] AudioClip missMove;

    // All the fruits on the board
    GameObject[,] fruits;
    // List<GameObject> check = new List<GameObject>();
    Collider2D boardCollider;

    Fruit selectedFruit;

    AudioSource audioSource;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.6f;

    // Time it takes for fruits to deactivate
    float timeToDisableFruit = 0.32f;

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
        IsShifting = true;

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
                    } while (NeighborsSameFruit(x, y, idx));

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
                    newFruit.transform.localPosition = new Vector2(x, y + 1);
                    newFruit.GetComponent<Fruit>().MoveFruit(new Vector2(x, y), false);
                }
                else newFruit.SetActive(false);
                fruits[x, y] = newFruit; // Add fruit to the board
            }
        }

        boardCollider.enabled = false;
        if (GUIManager.Instance.GamePlayMode == GamePlayMode.TimedMatch)
            StartCoroutine(GUIManager.Instance.TimeToMatchCoroutine());

        IsShifting = false;

        yield return null;
    }

    // Check if the fruit is on the table, if not, destroy it.
    bool IsFruitTouchingTheBoard(GameObject fruits) => boardCollider.IsTouching(fruits.GetComponent<Collider2D>());

    // Method in charge of verifying if the fruit is repeated in said column and row
    bool NeighborsSameFruit(int x, int y, int idx) => (x > 1 && fruits[x - 2, y] != null && idx == fruits[x - 2, y].GetComponent<Fruit>().Id) ||
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
            audioSource.PlayOneShot(missMove);
            BoardManager.Instance.IsShifting = false;
            yield break;
        }
        // Get the next fruit game object
        GameObject nextFruit = hit.collider.gameObject;

        // If the two fruits have the same ID, set IsShifting to false and return
        if (fruit.GetComponent<Fruit>().Id == nextFruit.GetComponent<Fruit>().Id)
        {
            audioSource.PlayOneShot(missMove);
            BoardManager.Instance.IsShifting = false;
            yield break;
        }

        audioSource.PlayOneShot(swapFruitAudio, 0.7f);

        // Move the two fruits to each other's positions
        fruit.GetComponent<Fruit>().MoveFruit(nextFruit.transform.localPosition);
        nextFruit.GetComponent<Fruit>().MoveFruit(fruit.transform.localPosition);

        // Decrement the MoveCounter variable in the Manager instance
        if (GUIManager.Instance.GamePlayMode == GamePlayMode.MovesLimited) GUIManager.Instance.MoveCounter--;

        yield return new WaitForSeconds(timeChangePositionFruits);
        // Check if any matches are found
        bool HasFoundMatches = FoundMatches(fruit, nextFruit);

        // MultiplicationFactor.Instance.SetMultiplicationFactor(); TODO:

        if (!HasFoundMatches)
        {
            if (GUIManager.Instance.GamePlayMode == GamePlayMode.TimedMatch)
            {
                audioSource.PlayOneShot(missMove);
                // If there are no matches found, return the fruits to their old position.
                fruit.GetComponent<Fruit>().MoveFruit(nextFruit.transform.localPosition);
                nextFruit.GetComponent<Fruit>().MoveFruit(fruit.transform.localPosition);

                yield return new WaitForSeconds(timeChangePositionFruits);
            }
            // Set IsShifting to false to indicate that the swap is complete
            IsShifting = false;
        }

        yield return null;
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
    /// <returns>A list of GameObjects representing the matching fruits found.</returns>
    List<GameObject> ThereAreFoundMatches(GameObject fruit)
    {
        List<GameObject> hMatches = new List<GameObject>();
        List<GameObject> vMatches = new List<GameObject>();

        // Search for horizontal matches
        hMatches = GetMatchesByDirections(fruit, new Vector2[2] { Vector2.left, Vector2.right });
        // Search for vertical matches
        vMatches = GetMatchesByDirections(fruit, new Vector2[2] { Vector2.up, Vector2.down });

        List<GameObject> combinedMatches = new List<GameObject>();
        // Combine the matches found in both directions
        if (hMatches.Count >= MinFruitsToMatch) combinedMatches = combinedMatches.Union(hMatches).ToList();

        if (vMatches.Count >= MinFruitsToMatch) combinedMatches = combinedMatches.Union(vMatches).ToList();

        return combinedMatches;
    }

    /// <summary>
    /// Clears any matches found between two given fruits by disabling their game objects.
    /// </summary>
    /// <param name="firstFruit">The first fruit to check for matches.</param>
    /// <param name="secondFruit">The second fruit to check for matches.</param>
    /// <returns>A list of GameObjects representing the fruits that were cleared.</returns>
    List<GameObject> ClearAllFruitMatches(GameObject firstFruit, GameObject secondFruit)
    {
        List<GameObject> firstMatches = ThereAreFoundMatches(firstFruit);
        List<GameObject> secondMatches = ThereAreFoundMatches(secondFruit);

        List<GameObject> allMatches = firstMatches.Union(secondMatches).ToList();

        if (allMatches.Count >= MinFruitsToMatch)
        {
            ClearSingleFruitMatch(allMatches);
        }

        return allMatches;
    }

    /// <summary>
    /// Clear all matched fruits from the grid and disable them.
    /// </summary>
    /// <param name="fruitToClear">List of matched fruits to be cleared.</param>
    void ClearSingleFruitMatch(List<GameObject> fruitToClear)
    {
        audioSource.PlayOneShot(fruitCrackAudio);
        fruitToClear.ForEach(matchedFruit =>
        {
            // Set the corresponding index in the array to null
            fruits[(int)matchedFruit.transform.localPosition.x, (int)matchedFruit.transform.localPosition.y] = null;
            // Disable the game object
            matchedFruit.GetComponent<Fruit>().DisableFruit();
        });
    }

    /// <summary>
    /// Coroutine that handles the animation of clearing matches and collapsing fruits.
    /// </summary>
    /// <param name="clearMatches">A list of GameObjects representing the fruits to be cleared.</param>
    IEnumerator FoundMatchesRutiner(List<GameObject> clearMatches)
    {
        yield return new WaitForSeconds(timeToDisableFruit);

        List<GameObject> collapsedFruits = CollapseFruits(GetColumns(clearMatches));

        GUIManager.Instance.Score += (clearMatches.Count * score);
        AddFruitsToPool(clearMatches);

        FindMatchesRecursively(collapsedFruits);
    }

    /// <summary>
    /// Determines if there are any matches between two given fruits, clears the matches, and triggers the animation coroutine if necessary.
    /// </summary>
    /// <param name="firstFruit">The first fruit to check for matches.</param>
    /// <param name="secondFruit">The second fruit to check for matches.</param>
    /// <returns>A boolean indicating whether any matches were found and cleared.</returns>
    bool FoundMatches(GameObject firstFruit, GameObject secondFruit)
    {
        bool foundMatches = false;

        List<GameObject> clearMatches = ClearAllFruitMatches(firstFruit, secondFruit);

        if (clearMatches.Count >= 3)
        {
            StartCoroutine(FoundMatchesRutiner(clearMatches));
            foundMatches = true;
        }

        return foundMatches;
    }

    /// <summary>
    /// Given a list of cleared matches, this method returns a list of their corresponding columns.
    /// </summary>
    /// <param name="clearMatches">The list of cleared matches.</param>
    /// <returns>The list of columns.</returns>
    List<int> GetColumns(List<GameObject> clearMatches)
    {
        List<int> columnMatches = new List<int>();

        // Loop through each cleared match and get the column it belongs to.
        clearMatches.ForEach(fruit =>
        {
            int x = (int)fruit.transform.localPosition.x;
            if (!columnMatches.Contains(x))
                columnMatches.Add(x);
        });

        return columnMatches;
    }

    /// <summary>
    /// This method collapses the fruits in the specified columns and returns a list of the fruits that have been moved.
    /// </summary>
    /// <param name="columns">The columns to collapse.</param>
    /// <param name="timeToCollapse">The time it takes for the fruits to collapse.</param>
    /// <returns>The list of fruits that have been moved.</returns>
    List<GameObject> CollapseFruits(List<int> columns)
    {
        List<GameObject> movingFruits = new List<GameObject>();
        // Loop through each column.
        for (int i = 0; i < columns.Count; i++)
        {
            int column = columns[i];
            // Loop through each row in the column.
            for (int y = 0; y < ySize; y++)
            {
                // If there is a null fruit, move the next non-null fruit down to fill the gap.
                if (fruits[column, y] == null)
                {
                    // Iterates through each row above the current position
                    for (int yPlus = y + 1; yPlus < ySize; yPlus++)
                    {
                        // If a fruit is found above the current position, moves it down to the current position
                        if (fruits[column, yPlus] != null && fruits[column, yPlus].activeSelf)
                        {
                            // Moves the fruit to the current position
                            fruits[column, yPlus].GetComponent<Fruit>().MoveFruit(new Vector2(column, y));
                            fruits[column, y] = fruits[column, yPlus];

                            // Adds the fruit to the movingFruits list if it is not already there
                            if (!movingFruits.Contains(fruits[column, y])) movingFruits.Add(fruits[column, y]);

                            // Empties the previous position of the fruit
                            fruits[column, yPlus] = null;
                            break;
                        }
                    }
                }
            }
        }


        FillBoard();

        return movingFruits;
    }

    /// <summary>
    /// Fills the game board with new fruits if any slot is empty.
    /// </summary>
    void FillBoard()
    {
        // List to store the newly generated fruits
        List<GameObject> newFruits = new List<GameObject>();

        // List to store the newly generated fruits
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                // If the slot is empty, generate a new fruit and add it to the list
                if (fruits[x, y] == null)
                {
                    fruits[x, y] = GetNewFruit(x, y);
                    newFruits.Add(fruits[x, y]);
                }
            }
        }

        // Check for any matches with the newly generated fruits
        FindMatchesRecursively(newFruits, true);
    }

    /// <summary>
    /// Recursively find and clear all matched fruits in the grid.
    /// </summary>
    /// <param name="collapsedFruits">List of fruits to check for matches.</param>
    void FindMatchesRecursively(List<GameObject> collapsedFruits, bool isCreatedNewFruits = false)
    {
        StartCoroutine(FindMatchesRecursivelyCoroutine(collapsedFruits, isCreatedNewFruits));
    }

    /// <summary>
    /// Coroutine that recursively finds and clears all matched fruits in the grid.
    /// </summary>
    /// <param name="collapsedFruits">List of fruits to check for matches.</param>
    IEnumerator FindMatchesRecursivelyCoroutine(List<GameObject> collapsedFruits, bool isCreatedNewFruits)
    {
        yield return new WaitForSeconds(.8f);

        List<GameObject> newMatches = new List<GameObject>();

        collapsedFruits.ForEach(fruit =>
        {
            // Check for matches with the current fruit
            List<GameObject> matches = ThereAreFoundMatches(fruit);
            if (matches.Count >= MinFruitsToMatch)
            {
                // Add new matches to the list of matches
                newMatches = newMatches.Union(matches).ToList();
                // Clear the matched fruits from the grid
                ClearSingleFruitMatch(matches);
            }
        });

        if (!(newMatches.Count >= MinFruitsToMatch))
        {
            if (!isCreatedNewFruits) IsShifting = false;
            yield break;
        }

        // Collapse the columns where new matches were found
        yield return new WaitForSeconds(timeToDisableFruit);
        List<GameObject> newCollapsedFruits = CollapseFruits(GetColumns(newMatches));
        GUIManager.Instance.Score += (newMatches.Count * score);
        AddFruitsToPool(newMatches);
        FindMatchesRecursively(newCollapsedFruits);

        yield return null;
    }

    /// <summary>
    /// Deactivates a single fruit and adds it to the object pooler.
    /// </summary>
    /// <param name="fruit">The fruit to add to the pool.</param>
    void AddFruitToPool(GameObject fruit)
    {
        ObjectPooler.Instance.FruitList.Add(fruit); // Add the fruit to the object pooler list
        fruit.transform.SetParent(ObjectPooler.Instance.gameObject.transform); // Set the parent object of the fruit
    }

    // <summary>
    /// Deactivates a list of fruits and adds them to the object pooler.
    /// </summary>
    /// <param name="fruits">The list of fruits to add to the pool.</param>
    void AddFruitsToPool(List<GameObject> fruits)
    {
        fruits.ForEach(fruit =>
        {
            // If the fruit is not active, add it to the pool
            if (!fruit.activeSelf) AddFruitToPool(fruit);
        });
    }

    /// <summary>
    /// Generates a new fruit at the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate of the new fruit.</param>
    /// <param name="y">The y coordinate of the new fruit.</param>
    /// <returns>The newly generated fruit.</returns>
    GameObject GetNewFruit(int x, int y)
    {
        // Randomly select a fruit prefab from the list
        int indexFruit = UnityEngine.Random.Range(0, prefabs.Count);

        GameObject newFruit = ObjectPooler.Instance.GetFruitToPool(indexFruit, spawnFruit.transform);
        newFruit.transform.localPosition = new Vector2(x, y);
        newFruit.name = string.Format("Fruit[{0}] [{1}]", x, y);
        newFruit.SetActive(true);

        return newFruit;
    }
}