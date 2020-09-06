using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberFunctions : MonoBehaviour
{
    // For finding a normal distribution value.
    // Code borrowed from: https://answers.unity.com/questions/421968/normal-distribution-random.html
    public static float RandomGaussian(float minGaussian = 1f, float maxGaussian = 100f)
    {
        float minValue = minGaussian;
        float maxValue = maxGaussian;

        float u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 5.0f;

        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

}
