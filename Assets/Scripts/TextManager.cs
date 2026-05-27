using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{

    public Health playerHealth;
    public Health enemyHealth;

    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;

    void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged += UpdatePlayerHealthUI;

        if (enemyHealth != null) enemyHealth.OnHealthChanged += UpdateEnemyHealthUI;
    }

    void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdatePlayerHealthUI;

        if (enemyHealth != null) enemyHealth.OnHealthChanged -= UpdateEnemyHealthUI;
    }

    private void UpdatePlayerHealthUI(float current, float max)
    {
        playerHealthText.text = "Player HP: " + current;
    }

    private void UpdateEnemyHealthUI(float current, float max)
    {
        enemyHealthText.text = "Enemy HP: " + current;
    }
}
