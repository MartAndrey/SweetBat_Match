
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UpdateScoreUI : MonoBehaviour
{
    // Audio source used to play the score increasing sound effect
    [SerializeField] AudioSource audioSourceScore;
    // Text object displaying the current level in the UI
    [SerializeField] TMP_Text scoreText;
    [SerializeField] AnimationCurve easingCurve;

    int displayScore = 0;
    float lerpDurationScore = 3;

    /// <summary>
    /// Coroutine that updates the displayed score in the UI over time
    /// </summary>
    public IEnumerator UpdateScoreRutiner()
    {
        // Wait for 1.1 seconds before starting the routine
        yield return new WaitForSeconds(1.1f);

        audioSourceScore.Play(); // Play the score increasing sound effect
        float timeElapsed = 0;

        // Continuously update the displayed score in the UI until it matches the current score
        while (timeElapsed < lerpDurationScore)
        {
            float t = easingCurve.Evaluate(timeElapsed / lerpDurationScore);

            // Increment the displayed score and update the score text in the UI
            displayScore = (int)Mathf.Lerp(displayScore, GUIManager.Instance.Score, t);

            scoreText.text = displayScore.ToString();
            // Wait for the next frame
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        // Update the score text in the UI one last time to match the final score
        scoreText.text = GUIManager.Instance.Score.ToString();
        // Stop playing the score increasing sound effect
        audioSourceScore.Stop();

        yield return null;
    }
}