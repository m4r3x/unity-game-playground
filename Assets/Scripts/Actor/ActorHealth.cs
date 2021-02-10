using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorHealth : MonoBehaviour
{
    [Tooltip("Maximum amount of health")]
    public float maxHealth = 50f;
    public UnityAction onDie;
    public float currentHealth { get; set; }
    public bool invincible { get; set; }
    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        if (invincible) return;
        currentHealth -= damage;
        HandleDeath();
    }

    void HandleDeath()
    {
        if (isDead)
        {
            return;
        }

        if (currentHealth <= 0f)
        {
            isDead = true;
            if (onDie != null)
            {
                onDie.Invoke();
            }
        }
    }
}

