using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{

    public Health playerHealth;
    public Health enemyHealth;

    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;

    public TextMeshProUGUI timeText;
    public int secondCounter;

    public TextMeshProUGUI roundText;

    public TextMeshProUGUI scoreText;

    void Update()
    {
        if(playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<Health>();
                playerHealth.OnHealthChanged += UpdatePlayerHealthUI;
                UpdatePlayerHealthUI(playerHealth.maxHealth, playerHealth.maxHealth);
            }
        }

        if(enemyHealth == null)
        {
            Enemy enemyObj = FindAnyObjectByType<Enemy>();
            if (enemyObj != null)
            {
                enemyHealth = enemyObj.GetComponent<Health>();
                enemyHealth.OnHealthChanged += UpdateEnemyHealthUI;
                UpdateEnemyHealthUI(enemyHealth.maxHealth, enemyHealth.maxHealth);
            }
        }

        if (GameManager.Instance != null)
        {
            roundText.text = "Round " + GameManager.Instance.currentRound;
        }

        secondCounter = Mathf.FloorToInt(GameManager.Instance.GetCurrentRoundTime());
        timeText.text = secondCounter.ToString();
    }

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
