using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorHealth : MonoBehaviour
{
    [Tooltip("Maximum amount of health")]
    public float maxHealth = 50f;
    public UnityAction onDie;
    public UnityAction onRevive;
    public float currentHealth { get; set; }
    public bool invincible { get; set; }
	// Number of seconds after which Actor is revived after death. 
    const float ReviveInterval = 5f;
    float reviverPolling = 0f;
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
        if (invincible) return;
        currentHealth -= damage;
        HandleDeath(damageSource);
    }
    
    void HandleDeath(GameObject damageSource)
    {
        if (isDead) return;

        if (currentHealth <= 0f)
        {
            // Set reviverPolling to zero, to ensure Actor will be revived in ReviverTimer interval via Update method.
            reviverPolling = 0f;
          	isDead = true;
            Actor actor = damageSource.GetComponent<Actor>();
            actor.IncreaseScore();
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
}

