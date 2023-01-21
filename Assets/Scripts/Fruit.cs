using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    // Unique fruit identifier
    public int Id;

    public Vector3 TargetPosition { get { return targetPosition; } set { targetPosition = value; } }

    // Variables that refer to the "fruits" when is selected or which is the last one that was selected
    static Color selectedColor = new Color(.5f, .5f, .5f, 1);
    static Fruit previousSelected = null;
    bool isSelected = false;

    // Directions to the sides of the "fruit"
    Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    SpriteRenderer spriteRenderer;

    // Position to move 
    Vector3 targetPosition;

    // Time it takes to move to the target
    float time = 30;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.4f;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        targetPosition = Vector3.zero;
    }

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, time * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                transform.position = targetPosition;
                targetPosition = Vector3.zero;
            }
        }
    }

    // Selected Fruit
    void SelectFruit()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Fruit>();
    }

    // Deselect fruit
    void DeselectFruit()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    // Method in charge of detecting the mouse or the touch
    void OnMouseDown()
    {
        if (spriteRenderer.sprite == null || BoardManager.Instance.isShifting)
            return;


        if (isSelected) // If the fruit is selected
        {
            DeselectFruit();
        }
        else
        {
            if (previousSelected == null) // If there is no fruit selected
            {
                SelectFruit();
            }
            else // If there is fruit selected
            {
                if (CanSwipe())
                {
                    StartCoroutine(CanBeSwapFruit());
                }
                else
                {
                    previousSelected.DeselectFruit();
                    SelectFruit();
                }
            }
        }
    }

    IEnumerator CanBeSwapFruit()
    {
        SwapFruit(previousSelected);

        yield return new WaitForSeconds(timeChangePositionFruits);

        previousSelected.FindAllMatches();
        previousSelected.DeselectFruit();
        FindAllMatches();

        GUIManager.Instance.MoveCounter--;

        StopCoroutine(BoardManager.Instance.FindDisableFruits());
        StartCoroutine(BoardManager.Instance.FindDisableFruits());
    }

    // Method in charge of changing the position of two fruits
    public void SwapFruit(Fruit newFruit)
    {
        if (spriteRenderer.sprite == newFruit.GetComponentInChildren<SpriteRenderer>().sprite)
            return;

        Vector3 previewPosition = transform.position; // Save the position of the second selected fruit

        this.targetPosition = newFruit.transform.position;
        newFruit.targetPosition = this.transform.position;

        // preview and Target refers to the second selected fruit and anotherFruit is the first selected fruit
        MoveFruitsPositionOfArray(previewPosition, targetPosition, newFruit.gameObject);
    }

    // Method that changes the position of the 2 fruits in the "fruit" array
    void MoveFruitsPositionOfArray(Vector3 previewPosition, Vector3 currentPosition, GameObject anotherFruit)
    {
        // We take advantage of changing the fruits in the array guiding us from the position since 
        // in these cases they are equivalent to the same

        // If the fruit is moved horizontally
        if (currentPosition.x != previewPosition.x)
        {
            if (currentPosition.x > previewPosition.x)
            {
                BoardManager.Instance.Fruits[(int)transform.localPosition.x + 1, (int)transform.localPosition.y] = this.gameObject;
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y] = anotherFruit;
                return;
            }
            else if (currentPosition.x < previewPosition.x)
            {
                BoardManager.Instance.Fruits[(int)transform.localPosition.x - 1, (int)transform.localPosition.y] = this.gameObject;
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y] = anotherFruit;
                return;
            }

        }

        // If the fruit is moved vertically
        if (currentPosition.y != previewPosition.y)
        {
            if (currentPosition.y > previewPosition.y)
            {
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y + 1] = this.gameObject;
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y] = anotherFruit;
                return;
            }
            else if (currentPosition.y < previewPosition.y)
            {
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y - 1] = this.gameObject;
                BoardManager.Instance.Fruits[(int)transform.localPosition.x, (int)transform.localPosition.y] = anotherFruit;
                return;
            }
        }
    }

    // Returns the neighboring fruit in that specified direction
    GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    // Returns neighboring fruits in all directions
    List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach (Vector2 direction in adjacentDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }

        return neighbors;
    }

    // Check if the fruit is a neighbor to be able to change the positions
    bool CanSwipe() => GetAllNeighbors().Contains(previousSelected.gameObject);

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
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}