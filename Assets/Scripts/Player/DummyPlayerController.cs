using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour
{
    Actor actor;
    ActorHealth actorHealth;

    void Start()
    {
        actor = GetComponent<Actor>();
        actorHealth = GetComponent<ActorHealth>();

        DebugUtility.HandleErrorIfNullGetComponent<ActorHealth, PlayerCharacterController>(actorHealth, this, gameObject);
        DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerCharacterController>(actor, this, gameObject);

        actorHealth.onDie += OnDie;
    }

    void OnDie()
    {
        Destroy(this.gameObject);
    }
}
