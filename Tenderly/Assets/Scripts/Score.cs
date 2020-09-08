using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;
    public static int scoreCount = 0;
    private double timeCount = 0.0;
    private double pointThreshold = 0.1;
    

    // Update is called once per frame
    void Update()
    {
        score.text = "Score: " + scoreCount.ToString();
    }
}
