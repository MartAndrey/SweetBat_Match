using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor;

public class Fruit : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Unique fruit identifier
    public int Id;

    // Variables that refer to the next fruit to select
    static Color selectedColor = new Color(.5f, .5f, .5f, 1);
    static Fruit nextSelected = null;

    [SerializeField] AudioClip swapFruitAudio;
    [SerializeField] AudioClip fruitDestroyAudio;
    // Movement curve when moving the fruit
    [SerializeField] Ease ease;

    AudioSource audioSource;

    // Directions to the sides of the "fruit"
    Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    SpriteRenderer spriteRenderer;

    // Movement speed
    float speed = 20;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.25f;

    // This variable is in charge of telling us if there is a touch or a click on the screen or more exactly on a fruit
    bool hasTouched = false;

    // When the fruit activates, you reset its target position because sometimes it changes its position
    void OnEnable()
    {
        transform.localScale = Vector3.one * 0.3f;
        transform.DOScale(Vector3.one, 0.35f);
        transform.rotation = Quaternion.Euler(0, 0, 180);
        transform.DORotate(Vector3.zero, 0.2f);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Selected Fruit
    void SelectFruit()
    {
        spriteRenderer.color = selectedColor;
    }

    // Deselect fruit
    void DeselectFruit()
    {
        spriteRenderer.color = Color.white;
    }

    // Method in charge of detecting the mouse or the touch
    void OnMouseDown()
    {
        SelectFruit();
    }

    // Method in charge of detecting when you release the mouse or touch
    void OnMouseUp()
    {
        DeselectFruit();
    }

    // Method in charge of detecting when we are dragging the fruit
    public void OnDrag(PointerEventData eventData)
    {
        StartCoroutine(ChangeOnTheBoard());

        MultiplicationFactor.Instance.ResetMultiplicationFactor();

        if (BoardManager.Instance.IsShifting || GUIManager.Instance.MoveCounter <= 0)
            return;

        // Calculate the drag direction
        Vector2 direction = eventData.position - eventData.pressPosition;

        // Initialize the direction label with the vertical direction
        Vector2 directionLabel = direction.y > 0 ? Vector2.up : Vector2.down;

        // If the horizontal direction is greater than the vertical direction, change the direction label to the horizontal direction
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            directionLabel = direction.x > 0 ? Vector2.right : Vector2.left;
        }

        // Check if the method has not been called yet
        if (!hasTouched)
        {
            audioSource.PlayOneShot(swapFruitAudio, 0.7f);
            StartCoroutine(CanBeSwapFruit(directionLabel));
            hasTouched = true;
            return;
        }
    }

    /// <summary>
    /// Moves a fruit to the specified position and updates its name and position in the BoardManager.
    /// </summary>
    /// <param name="position">The position to move the fruit to.</param>
    public void MoveFruit(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        // Use the DOTween library to move the fruit to the new position with a specified ease and time.
        this.transform.DOLocalMove(position, timeChangePositionFruits).SetEase(ease).OnComplete(() =>
        {
            // Set the name of the fruit to include its new position.
            this.name = string.Format("Fruit[{0}] [{1}]", x, y);
            // Update the BoardManager with the new position of the fruit.
            BoardManager.Instance.Fruits[x, y] = this.gameObject;
        });
    }

    // If there are no more changes on the board, it exits the coroutine
    IEnumerator ChangeOnTheBoard()
    {
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
    }

    // // Method in charge of detecting when we drop the fruit and therefore it stops dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        hasTouched = false;
    }

    IEnumerator CanBeSwapFruit(Vector2 direction)
    {
        SwapFruit(direction);
        DeselectFruit();

        yield return new WaitForSeconds(timeChangePositionFruits);

        if (nextSelected == null) yield break;

        nextSelected.FindAllMatches();
        FindAllMatches();

        GUIManager.Instance.MoveCounter--;
    }

    // Method in charge of changing the position of two fruits
    public void SwapFruit(Vector2 direction)
    {
        BoardManager.Instance.IsShifting = true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (hit.collider == null)
        {
            BoardManager.Instance.IsShifting = false;
            return;
        }

        nextSelected = hit.collider.gameObject.GetComponent<Fruit>();

        // If the 'hit' is the same fruit, leave the method
        if (spriteRenderer.sprite == nextSelected.gameObject.GetComponentInChildren<SpriteRenderer>().sprite)
        {
            BoardManager.Instance.IsShifting = false;
            return;
        }

        this.MoveFruit(nextSelected.transform.localPosition);
        nextSelected.MoveFruit(this.transform.localPosition);
    }

    // Method returns the neighboring fruits that match
    List<GameObject> FindMatch(Vector2 direction)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

        // Query the neighbors in the direction of the parameter
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        while (hit.collider != null && hit.collider.GetComponent<Fruit>().Id == Id)
        {
            matchingFruits.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }

        return matchingFruits;
    }

    // Method in charge of cleaning the fruits and returns true if it is the case
    bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

        foreach (Vector2 direction in directions)
        {
            matchingFruits.AddRange(FindMatch(direction));
        }

        if (matchingFruits.Count >= BoardManager.MinFruitsToMatch)
        {
            foreach (GameObject fruit in matchingFruits)
            {
                fruit.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }

            return true;
        }

        return false;
    }

    // Method in charge of looking for the fruits horizontally and vertically 
    public void FindAllMatches()
    {
        bool hMatch = ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        bool vMatch = ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (hMatch || vMatch)
        {
            MultiplicationFactor.Instance.SetMultiplicationFactor();

            audioSource.PlayOneShot(fruitDestroyAudio, 1);
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

            BoardManager.OnBoardChanges.Invoke();
        }
        else
        {
            BoardManager.Instance.IsShifting = false;
        }
    }
}