using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    // Unique fruit identifier
    public int Id { get; set; }

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
    float time = 5;

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
                SwapFruit(previousSelected.gameObject);
                previousSelected.DeselectFruit();
            }
        }
    }

    // Method in charge of changing the position of two fruits
    public void SwapFruit(GameObject newFruit)
    {
        if (spriteRenderer.sprite == newFruit.GetComponentInChildren<SpriteRenderer>().sprite)
            return;

        this.targetPosition = newFruit.transform.position;
        newFruit.GetComponent<Fruit>().targetPosition = this.transform.position;
    }
}