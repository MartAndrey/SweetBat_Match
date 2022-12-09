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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}