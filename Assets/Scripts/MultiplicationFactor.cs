using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicationFactor : MonoBehaviour
{
    [SerializeField] GameObject multiplicationFactorGameObject;
    [SerializeField] Animation[] animations;

    int multiplicationFactor;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // IEnumerator SetMultiplicationFactor()
    // {
    //     multiplicationFactorGameObject.SetActive(true);
    //     multiplicationFactor++;
    // }
}
