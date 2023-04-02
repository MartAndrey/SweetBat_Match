using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusLevel : MonoBehaviour
{
    // Define scaling factors for the level focus effect
    Vector3 scaleUp = new Vector3(1, 1, 1);
    Vector3 scaleDown = new Vector3(0.8f, .8f, .8f);

    // Reference to the levels scroll rect
    [SerializeField] ScrollRect scrollRect;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the levels scroll rect is not moving too fast horizontally
        if ((scrollRect.velocity.x < 500 && scrollRect.velocity.x >= 0) || scrollRect.velocity.x > -500 && scrollRect.velocity.x <= 0)
        {
            other.gameObject.transform.localScale = scaleUp;  // Scale up the level that the user is currently focusing on
            scrollRect.StopMovement(); // Stop the scroll rect movement to make the level more prominent
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Scale down the level that the user is no longer focusing on
        other.gameObject.transform.localScale = scaleDown;
    }
}