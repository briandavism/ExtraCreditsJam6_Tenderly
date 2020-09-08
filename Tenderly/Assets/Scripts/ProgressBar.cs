using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://fractalpixels.com/devblog/unity-2D-progress-bars

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Text displayText;

    public static int waterEarned = 0;
    private int waterXpReq;
    private int spentWaterXp;
    private int NewWaterXP;
    private int pointsRemaining;

    private int waterXpBase = 10;
    private float waterXpExponent = 1.33f;

    // Create a property to handle the slider's value
    private float currentValue = 0f;
    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            
            currentValue = value;
            slider.value = currentValue;
            displayText.text = (slider.value * 100).ToString("0.00") + "%";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentValue = 0f;
        waterXpReq = (int)Mathf.Round(Mathf.Pow(waterEarned, waterXpExponent) + waterXpBase);
    }

    void GrantWater()
    {
        spentWaterXp += waterXpReq;
        waterEarned++;
        waterXpReq = (int)Mathf.Round(Mathf.Pow(waterEarned, waterXpExponent) + waterXpBase);
    }

    // Update is called once per frame
    void Update()
    {
        NewWaterXP = Mathf.Max(Score.scoreCount - spentWaterXp,0);
        pointsRemaining = waterXpReq - NewWaterXP;
        displayText.text = "  Next water in " + pointsRemaining.ToString() + " pts";
        slider.value = (float)NewWaterXP / (float)waterXpReq;
        if(NewWaterXP >= waterXpReq)
        {
            GrantWater();
        }

        //CurrentValue += 0.0043f;
    }
}
