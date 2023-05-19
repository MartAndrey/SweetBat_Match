using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProbabilityFruit
{
    public static int[] SetFruitProbability(int probabilities)
    {
        int totalProbability = 0;

        int[] fruitsProbabilities = new int[probabilities];

        for (int i = 0; i < probabilities; i++)
        {
            int rn = Mathf.RoundToInt(Random.Range(1, 101));

            fruitsProbabilities[i] = rn;
            totalProbability += rn;
        }

        float fixProbability = 100f / totalProbability;

        for (int i = 0; i < probabilities; i++)
        {
            fruitsProbabilities[i] = Mathf.RoundToInt(fruitsProbabilities[i] * fixProbability);
        }

        return fruitsProbabilities;
    }

    public static int GetFruitProbability(int[] fruitsProbabilities)
    {
        int accumulatedProbability = 0;
        int randomNumber = UnityEngine.Random.Range(0, 101);
        int prefab = 0;

        foreach (int i in fruitsProbabilities)
        {
            if (randomNumber < accumulatedProbability + i)
                return prefab;

            accumulatedProbability += i;
            prefab++;
        }
        Debug.LogWarning("Is not Found Fruit Probability");
        return 1;
    }
}