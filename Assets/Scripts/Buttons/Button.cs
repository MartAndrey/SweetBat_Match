using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
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

    public void OnButtonNormal()
    {
        animator.SetTrigger(("Normal"));
    }

    public void OnButtonPressed()
    {
        animator.SetTrigger(("Pressed"));
        audioSource.PlayOneShot(popIn);
    }

    public void OnButtonDisabled()
    {
        animator.SetTrigger(("Disabled"));
        audioSource.PlayOneShot(popOut);
    }
}
