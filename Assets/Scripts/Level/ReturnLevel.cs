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
    [SerializeField] GameObject pinIcon;
    // Defines the total duration of the move in seconds.
    [SerializeField] float moveDuration = 0.5f;

    // This integer variable defines the amount of rotation when the player returns to the returnLevel.
    [SerializeField] int rotation = 90;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // This public function returns the player to the previous level.
    public void ReturnBack()
    {
        audioSource.Play();

        // Calculate the distance between the focusLevel and profileMarker and set the scrollRect's anchoredPosition accordingly.
        float distance = focusLevel.transform.localPosition.x - profileMarker.transform.localPosition.x;
        // scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x + distance, 0);

        // Calculate the end position of the scrollRect content based on the distance.
        Vector2 endScrollRect = new Vector2(scrollRect.content.anchoredPosition.x + distance, 0);

        // Start a coroutine to smoothly move the scrollRect content from its current position to the target position.
        StartCoroutine(SmoothReturnBack(scrollRect.content.anchoredPosition, endScrollRect, moveDuration));
    }

    // This public function rotates the returnLevel and profile based on the given direction.
    public void Rotation(Vector2 direction)
    {
        this.gameObject.transform.rotation = direction == Vector2.right ? Quaternion.Euler(0, 0, -rotation) : Quaternion.Euler(0, 0, rotation);
        profile.transform.localRotation = direction == Vector2.right ? Quaternion.Euler(0, 0, rotation) : Quaternion.Euler(0, 0, -rotation);
    }

    // Set the active or disable state of the profile and pinIcon game objects.
    public void StatusChild(bool state)
    {
        profile.SetActive(state);
        pinIcon.SetActive(state);
    }

    // Coroutine to smoothly move the scrollRect content from the start position to the target position over a specified duration.
    IEnumerator SmoothReturnBack(Vector2 startPosition, Vector2 target, float lerpDuration)
    {
        float timeElapsed = 0f;

        // Loop until the lerpDuration has elapsed.
        while (timeElapsed < lerpDuration)
        {
            // Interpolate the position of the scrollRect content using a lerp function.
            scrollRect.content.anchoredPosition = Vector2.Lerp(startPosition, target, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;  // Increment the time elapsed.
            yield return null; // Wait for the next frame.
        }

        // Set the position of the scrollRect content to the target position to ensure accuracy.
        scrollRect.content.anchoredPosition = target;
    }
}