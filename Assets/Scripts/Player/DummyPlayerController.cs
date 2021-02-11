using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour
{
    [Tooltip("Reference to the body used by this Dummy Player")]
    public GameObject body;
    ActorHealth actorHealth;

    void Start()
    {
        actorHealth = GetComponent<ActorHealth>();

        DebugUtility.HandleErrorIfNullGetComponent<ActorHealth, PlayerCharacterController>(actorHealth, this, gameObject);

        actorHealth.onDie += OnDie;
        actorHealth.onRevive += OnRevive;
    }

    void OnDie()
    {
		body.SetActive(false);
    }

    void OnRevive()
    {
        body.SetActive(true);
    }
}
