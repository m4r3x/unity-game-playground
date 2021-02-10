using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Tooltip("Represents the team of actor")]
    public int affiliation;
    ActorsManager actorsManager;
    int score;

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
}
