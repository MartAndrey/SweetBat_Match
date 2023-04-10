using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnLevel : MonoBehaviour
{
    // These public GameObject variables, scrollRect, profileMarker, focusLevel, and profile, can be accessed in the Inspector.
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject profileMarker;
    [SerializeField] GameObject focusLevel;
    [SerializeField] GameObject profile;

    // This integer variable defines the amount of rotation when the player returns to the returnLevel.
    [SerializeField] int rotation = 90;

    // This public function returns the player to the previous level.
    public void ReturnBack()
    {
         // Calculate the distance between the focusLevel and profileMarker and set the scrollRect's anchoredPosition accordingly.
        float distance = focusLevel.transform.localPosition.x - profileMarker.transform.localPosition.x;
        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x + distance, 0);
    }

    // This public function rotates the returnLevel and profile based on the given direction.
    public void Rotation(Vector2 direction)
    {
        this.gameObject.transform.rotation = direction == Vector2.right ? Quaternion.Euler(0, 0, -rotation) : Quaternion.Euler(0, 0, rotation);
        profile.transform.localRotation = direction == Vector2.right ? Quaternion.Euler(0, 0, rotation) : Quaternion.Euler(0, 0, -rotation);
    }
}