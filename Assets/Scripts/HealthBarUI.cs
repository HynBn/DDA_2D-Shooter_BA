using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;

    [Header("Settings")]
    public bool isEnemyFloatingBar = false; // Haken setzen für Gegner-Leisten!

    private Health targetHealth;

    void Start()
    {
        // Wenn das die schwebende Leiste auf dem Gegner-Prefab ist, 
        // sucht sie sich ihr Health-Skript einfach selbst!
        if (isEnemyFloatingBar)
        {
            Health myHealth = GetComponentInParent<Health>();
            if (myHealth != null)
            {
                Setup(myHealth);
            }
        }
    }

    // Diese Methode wird vom GameManager/UIManager gerufen, um den Spieler zu verknüpfen
    public void Setup(Health healthComponent)
    {
        // Altes Event abmelden (wichtig beim Runden-Recycling!)
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
        }

        targetHealth = healthComponent;

        // Neues Event anmelden
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthBar;
        }
    }

    // Wird automatisch ausgelöst, wenn das Health-Skript OnHealthChanged?.Invoke ruft
    private void UpdateHealthBar(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }

    void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}