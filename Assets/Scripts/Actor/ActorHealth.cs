﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ActorHealth : MonoBehaviour
{
    [Tooltip("Maximum amount of health")]
    public float maxHealth = 50f;
    [Tooltip("Make character invincible")] 
    public bool invincible;
    [Tooltip("The text field displaying the score")]
    public GameObject GUI;
    public UnityAction onDie;
    public UnityAction onRevive;
    public float currentHealth { get; set; }
    // Number of seconds after which Actor is revived after death. 
    const float ReviveInterval = 5f;
    float reviverPolling = 0f;
    int score = 0;
    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update() 
	{
        reviverPolling += Time.deltaTime;
        // Only Revive when player is Dead, and once per ReviverTimer interval. 
        if (isDead && ReviveInterval < reviverPolling) HandleRevival();
 	}

    public void TakeDamage(float damage, GameObject damageSource)
    {
        if (invincible)
        {
            // don't kill, but incr score
            IncreaseScore(damageSource);
            return;
        }
        currentHealth -= damage;
        HandleDeath(damageSource);
    }
    
    void HandleDeath(GameObject damageSource)
    {
        if (isDead) return;

        if (currentHealth <= 0f)
        {
            // Set reviverPolling to zero, to ensure player will be revived in ReviverTimer interval via Update method.
            reviverPolling = 0f;
          	isDead = true;
            IncreaseScore(damageSource);
            if (onDie != null) onDie.Invoke();
        }
    }
    
    public void HandleRevival()
    {
        reviverPolling = 0f;
        isDead = false;
        currentHealth = maxHealth;
        if (onRevive != null) onRevive.Invoke();
    }

    void IncreaseScore(GameObject damageSource)
    {
        score++;
        // a shameful way of getting score Text GUI, to avoid rebuilding bunch of classes
        TextMeshProUGUI uiText = damageSource.GetComponentInChildren<TextMeshProUGUI>();
        if (uiText != null)
        {
            uiText.text = "kills: " + score;
        }
    }
}

