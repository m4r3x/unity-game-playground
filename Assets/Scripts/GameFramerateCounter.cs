using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameFramerateCounter : MonoBehaviour
{
    [Tooltip("Delay between updates of the displayed framerate value")]
    public float pollingTime = 1f;
    [Tooltip("The text field displaying the framerate")]
    public TextMeshProUGUI uiText;
    float accDelta = 0f;
    int accFrames = 0;

    void Update()
    {
        accDelta += Time.deltaTime;
        accFrames++;

        if (accDelta >= pollingTime)
        {
            int framerate = Mathf.RoundToInt((float)accFrames / accDelta);
            uiText.text = "fps: " + framerate.ToString();

            accDelta = 0f;
            accFrames = 0;
        }
    }
}