using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorDamageable : MonoBehaviour
{
    public ActorHealth actorHealth { get; private set; }

    void Awake()
    {
        actorHealth = GetComponent<ActorHealth>();
        // if not found, check deeper
        if (!actorHealth)  actorHealth = GetComponentInParent<ActorHealth>();
    }

    public void InflictDamage(float damage, GameObject damageOwner)
    {
        actorHealth.TakeDamage(damage, damageOwner);
    }
}
