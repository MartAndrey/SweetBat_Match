using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fruit : MonoBehaviour, IDragHandler, IEndDragHandler
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

    // This variable is in charge of telling us if there is a touch or a click on the screen or more exactly on a fruit
    bool hasTouched = false;

    // When the fruit activates, you reset its target position because sometimes it changes its position
    void OnEnable()
    {
        targetPosition = Vector3.zero;
    }

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

    public void OnDrag(PointerEventData eventData)
    {
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
            SwapFruit(directionLabel);
            hasTouched = true;
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        hasTouched = false;
    }

    // Method in charge of detecting the mouse or the touch
    // void OnMouseDrag()
    // {
    //     if (spriteRenderer.sprite == null || BoardManager.Instance.isShifting)
    //         return;


    //     if (isSelected) // If the fruit is selected
    //     {
    //         DeselectFruit();
    //     }
    //     else
    //     {
    //         if (previousSelected == null) // If there is no fruit selected
    //         {
    //             SelectFruit();
    //         }
    //         else // If there is fruit selected
    //         {
    //             if (CanSwipe())
    //             {
    //                 StartCoroutine(CanBeSwapFruit());
    //             }
    //             else
    //             {
    //                 previousSelected.DeselectFruit();
    //                 SelectFruit();
    //             }
    //         }
    //     }
    // }

    // IEnumerator CanBeSwapFruit()
    // {
    //     SwapFruit(previousSelected);

    //     yield return new WaitForSeconds(timeChangePositionFruits);

    //     previousSelected.FindAllMatches();
    //     previousSelected.DeselectFruit();
    //     FindAllMatches();

    //     GUIManager.Instance.MoveCounter--;
    // }

    // Method in charge of changing the position of two fruits
    public void SwapFruit(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        // If the 'hit' is the same fruit, leave the method
        if (spriteRenderer.sprite == hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().sprite)
            return;

        Vector3 previewPosition = transform.position; // Save the position of the second selected fruit

        this.targetPosition = hit.collider.gameObject.transform.position;
        hit.collider.gameObject.GetComponent<Fruit>().TargetPosition = this.transform.position;

        // preview and Target refers to the second selected fruit and anotherFruit is the first selected fruit
        MoveFruitsPositionOfArray(previewPosition, targetPosition, hit.collider.gameObject);
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

            BoardManager.OnBoardChanges.Invoke();
        }
    }
}