using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;
    public static int scoreCount;
    private double timeCount = 0.0;
    private double pointThreshold = 0.1;
    

    // Update is called once per frame
    void Update()
    {
        timeCount += Time.deltaTime;
        if(timeCount >= pointThreshold)
        {
            timeCount = 0.0;
            scoreCount++;
        }
        score.text = "Score: " + scoreCount.ToString();
    }
}
