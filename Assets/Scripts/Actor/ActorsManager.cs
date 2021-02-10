using System.Collections.Generic;
using UnityEngine;

public class ActorsManager : MonoBehaviour
{
    public List<Actor> actors { get; private set; }

    void Awake()
    {
        actors = new List<Actor>();
    }
}