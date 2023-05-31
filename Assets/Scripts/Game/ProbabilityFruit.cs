using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ProbabilityFruit
{
    /// <summary>
    /// Generates randomized probabilities for different fruits.
    /// </summary>
    /// <param name="probabilities">The number of probabilities to generate.</param>
    /// <returns>A dictionary with fruit IDs as keys and their corresponding probabilities as values.</returns>
    public static Dictionary<int, int> GenerateFruitProbabilities(int probabilities)
    {
        int totalProbability = 0;

        // Create a dictionary to store fruit probabilities
        Dictionary<int, int> fruitsProbabilities = new Dictionary<int, int>();

        // Generate probabilities for each fruit
        for (int i = 0; i < probabilities; i++)
        {
            int rn = Mathf.RoundToInt(Random.Range(1, 101));

            // Add the generated probability to the dictionary
            fruitsProbabilities.Add(i, rn);
            // Update the total probability
            totalProbability += rn;
        }

        // Calculate a fix factor for normalizing probabilities to a total of 100
        float fixProbability = 100f / totalProbability;

        // Adjust the probabilities based on the fix factor
        for (int i = 0; i < probabilities; i++)
            fruitsProbabilities[i] = Mathf.RoundToInt(fruitsProbabilities[i] * fixProbability);

        // Return the generated fruit probabilities
        return fruitsProbabilities;
    }

    /// <summary>
    /// Retrieves the ID of a fruit based on its probability.
    /// </summary>
    /// <param name="fruitsProbabilities">A dictionary containing fruit IDs and their corresponding probabilities.</param>
    /// <returns>The ID of the selected fruit based on probability.</returns>
    public static int GetFruitProbability(Dictionary<int, int> fruitsProbabilities)
    {
        int accumulatedProbability = 0;
        int randomNumber = UnityEngine.Random.Range(1, 101);

        // Find the fruit based on the generated random number and accumulated probabilities
        foreach (KeyValuePair<int, int> i in fruitsProbabilities)
        {
            if (randomNumber < accumulatedProbability + i.Value)
                return i.Key;

            accumulatedProbability += i.Value;
        }

        return GetFruitProbabilityWithException(fruitsProbabilities.Count, fruitsProbabilities.Last().Key);
    }

    /// <summary>
    /// Calculates the probability of getting a fruit, excluding a specific fruit.
    /// </summary>
    /// <param name="amountFruits">The total number of fruits.</param>
    /// <param name="exception">The fruit to be excluded from the calculation.</param>
    /// <returns>The index of the randomly selected fruit.</returns>
    static int GetFruitProbabilityWithException(int amountFruits, int exception)
    {
        int rn;

        do
        {
            rn = Random.Range(0, amountFruits);
        } while (rn == exception);

        return rn;
    }
}