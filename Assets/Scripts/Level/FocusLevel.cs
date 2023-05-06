using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusLevel : MonoBehaviour
{
    static GameObject focusLevel;  // Declare a static GameObject variable called focusLevel

    // Reference to the levels scroll rect
    [SerializeField] ScrollRect scrollRect;

    // Define scaling factors for the level focus effect
    Vector3 scaleUp = new Vector3(1, 1, 1);
    Vector3 scaleDown = new Vector3(0.8f, .8f, .8f);

    int offset = 5; // A variable for defining the distance to the focused level in the scrollable view

    int limitAnchorScroll = 120;   // A variable for defining the limit of scrolling distance for anchoring the view to a level

    int speedToStop = 500; // A variable for defining the maximum speed for scrolling

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((Mathf.Abs(scrollRect.content.offsetMin.x) <= limitAnchorScroll))
        {
            SetLevelFocus(other);
        }
        else if ((Mathf.Abs(scrollRect.content.offsetMax.x) <= limitAnchorScroll))
        {
            SetLevelFocus(other);
        }

        // Check if the levels scroll rect is not moving too fast horizontally
        if (SlowSpeed())
        {
            SetLevelFocus(other);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (focusLevel != other.gameObject) return; // Check if the current focused level is the same as the entered level

        // Get the position of the focused level and the entered level and check if they are within the distance offset
        int focusLevelPosition = Mathf.CeilToInt(transform.position.x);
        int otherPosition = Mathf.CeilToInt(other.transform.position.x);

        if (otherPosition >= (focusLevelPosition - offset) && otherPosition <= (focusLevelPosition + offset))
        {
            scrollRect.StopMovement();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Scale down the level that the user is no longer focusing on
        other.gameObject.transform.localScale = scaleDown;
    }

    public void SetLevelFocus(Collider2D other)
    {
        focusLevel = other.gameObject;
        other.gameObject.transform.localScale = scaleUp;  // Scale up the level that the user is currently focusing on
    }

    // A function to check if the scrolling speed is slow enough to stop
    bool SlowSpeed() => (scrollRect.velocity.x < speedToStop && scrollRect.velocity.x >= 0) || (scrollRect.velocity.x > -speedToStop && scrollRect.velocity.x <= 0);
}