using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoToggle : MonoBehaviour
{
    private bool toggle = false;

    public void ToggleUpdate(GameObject image)
    {
        toggle = !toggle;
        if (toggle)
        {
            image.SetActive(true);
        }
        else if (!toggle)
        {
            image.SetActive(false);
        }

    }
}
