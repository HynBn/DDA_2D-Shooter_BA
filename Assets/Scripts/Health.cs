using System;
using UnityEngine;

public class Health : MonoBehaviour
{

    public float maxHealth = 100f;
    private float currentHealth;

    public event Action<float, float> OnHealthChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
