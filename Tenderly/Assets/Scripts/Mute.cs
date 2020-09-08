using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour
{
    public Button button;
    public Sprite unmuted;
    public Sprite muted;
    private bool toggle = false;
    public void MuteUpdate(AudioSource song)
    {
        song.mute = !song.mute;
        toggle = !toggle;
        if(toggle)
        {
            button.image.sprite = muted;
        }
        else if(!toggle)
        {
            button.image.sprite = unmuted;
        }
        
    }
    
}
