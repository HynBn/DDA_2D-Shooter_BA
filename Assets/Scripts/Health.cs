using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum EntityType {Player, Enemy};
    public EntityType entityType;

    public float maxHealth = 150f;
    private float currentHealth;

    public event Action<float, float> OnHealthChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (entityType == EntityType.Player)
        {
            if (DataTracker.Instance != null)
            {
                DataTracker.Instance.RegisterPlayerDamageTaken(damageAmount);
            }
        } else if (entityType == EntityType.Enemy)
        {
            if (DataTracker.Instance != null)
            {
                DataTracker.Instance.RegisterPlayerHit();
            }

            LocalEnemyTracker myLocalTracker = GetComponent<LocalEnemyTracker>();
            if (myLocalTracker != null)
            {
                myLocalTracker.RegisterEnemyDamageTaken(damageAmount);
            }
        }

        if (currentHealth <= 0)
        {
            bool isPlayer = gameObject.CompareTag("Player");
            gameObject.SetActive(false);

            if(GameManager.Instance != null)
            {
                GameManager.Instance.EntityDeath(isPlayer);
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        gameObject.SetActive(true);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
