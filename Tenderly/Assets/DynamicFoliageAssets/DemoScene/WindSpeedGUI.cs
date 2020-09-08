using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpeedGUI : MonoBehaviour {

    private FoliageInteraction fi;

    void Awake(){
        fi = GameObject.FindObjectOfType<FoliageInteraction>();
    }

    void OnGUI(){

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;

        GUI.Label(new Rect(25, 20, 100, 20), "Wind Speed", style);
        fi.windSpeed = GUI.HorizontalSlider(new Rect(150, 25, 150, 150), fi.windSpeed, 0, 40.0f);
    }
}
