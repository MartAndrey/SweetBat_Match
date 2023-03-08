using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Tooltip("Sound when touching and leaving")]
    [SerializeField] AudioClip popIn, popOut;

    Animator animator;
    AudioSource audioSource;
    Button button;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
    }

    // Button normal state
    public void OnButtonNormal()
    {
        animator.SetTrigger(("Normal"));
    }

    // When the button is pressed
    public void OnButtonPressed()
    {
        animator.SetTrigger(("Pressed"));
        audioSource.PlayOneShot(popIn);
    }

    // When they stop touching the button
    public void OnButtonDisabled()
    {
        animator.SetTrigger(("Disabled"));
        audioSource.PlayOneShot(popOut);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(popIn);
    }

    public void PlayReleaseSound()
    {
        audioSource.PlayOneShot(popOut);
    }
}