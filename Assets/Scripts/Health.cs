using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum EntityType {Player, Enemy};
    public EntityType entityType;

    public float maxHealth = 150f;
    private float currentHealth;

    public event Action<float, float> OnHealthChanged;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (gameObject.activeInHierarchy && currentHealth > 0) StartCoroutine(FlashRoutine());
        if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSFX);

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

    private System.Collections.IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = originalColor;
        }
    }
}
