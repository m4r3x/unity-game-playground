using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Actor : MonoBehaviour
{
    [Tooltip("Represents the team of actor")]
    public int affiliation;
    [Tooltip("The text field displaying the score")]
    public TextMeshProUGUI scoreUIText;
    public int score { set; get; }
    ActorsManager actorsManager;
    
    private void Start()
    {
        actorsManager = GameObject.FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(actorsManager, this);

        if (!actorsManager.actors.Contains(this))  actorsManager.actors.Add(this); 
    }

    private void OnDestroy()
    {
        if (actorsManager)  actorsManager.actors.Remove(this);
    }

    public void IncreaseScore()
    { 
        score++;
        scoreUIText.text = "kills: " + score;
    }
}
