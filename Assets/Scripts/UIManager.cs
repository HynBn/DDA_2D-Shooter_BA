using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject selectPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI roundTimerText;
    public TextMeshProUGUI roundCounterText;
    public TextMeshProUGUI pScoreText;
    public TextMeshProUGUI eScoreText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemy1HealthText;
    public TextMeshProUGUI enemy2HealthText;
    public TextMeshProUGUI enemy3HealthText;

    [Header("Enemy Selection Visuals")]
    public Image[] enemyCountButtonImages;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.red;
    
    private bool isPaused = false;

    private TextMeshProUGUI[] enemyHealthTexts;
    private Health playerHealth;
    private Health[] enemyHealths = new Health[3];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        enemyHealthTexts = new TextMeshProUGUI[] { enemy1HealthText, enemy2HealthText, enemy3HealthText };
    }

    void OnEnable()
    {
        GameManager.OnRoundStart += ShowHUDForRound;
    }

    void OnDisable()
    {
        GameManager.OnRoundStart -= ShowHUDForRound;

        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdatePlayerHealthText;
        for (int i = 0; i < enemyHealths.Length; i++)
        {
            if (enemyHealths[i] != null) enemyHealths[i].OnHealthChanged -= UpdateEnemyHealthText(i, 0, 0);
        }

    }

    void Start()
    {
        ShowPanel(mainMenuPanel);
            }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            if (roundTimerText != null)
            {
                roundTimerText.text = GameManager.Instance.currentRoundTime.ToString("F0");
            }
        }
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            TogglePause();
        }
    }

    public void UpdateScoreText(int playerScore, int enemyScore)
    {
        if (eScoreText != null && pScoreText != null)
        {
            pScoreText.text = playerScore.ToString();
            eScoreText.text = enemyScore.ToString();
        }
    }

    public void BindPlayerHealth (Health health)
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdatePlayerHealthText;
        playerHealth = health;
        if (playerHealth != null) playerHealth.OnHealthChanged += UpdatePlayerHealthText;
    }

    public void BindEnemyHealth(Health health, int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= enemyHealths.Length) return;

        enemyHealths[enemyIndex] = health;

        if (health != null)
        {
            enemyHealthTexts[enemyIndex].gameObject.SetActive(true);
            health.OnHealthChanged += UpdateEnemyHealthText(enemyIndex, 0, 0);
        } else
        {
            enemyHealthTexts[enemyIndex].gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerHealthText(float current, float max)
    {
        if (playerHealthText != null) playerHealthText.text = "Player HP: " + current.ToString("F0") + " / " + max.ToString("F0");
    }

    private System.Action<float, float> UpdateEnemyHealthText(int index, float current, float max)
    {
        return (current, max) =>
        {
            if (enemyHealthTexts[index] != null) enemyHealthTexts[index].text = "Enemy " + (index + 1) + " HP: " + current.ToString("F0") + " / " + max.ToString("F0");
        };
    }

    public void StartGame()
    {
        ShowPanel(hudPanel);
        if(GameManager.Instance != null) GameManager.Instance.StartMatch();
    }

    public void OpenSelect()
    {
        ShowPanel(selectPanel);
    }

    public void OpenOptions()
    {
        ShowPanel(optionsPanel);
    }

    public void BackToMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            ShowPanel(pausePanel);
        }
        else
        {
            Time.timeScale = 1f;
            ShowPanel(hudPanel);
        }
    }

    private void ShowHUDForRound(int roundNumber)
    {
        if (roundCounterText != null) roundCounterText.text = "Round " + roundNumber;
        ShowPanel(hudPanel);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (selectPanel) selectPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (panelToShow) panelToShow.SetActive(true);
    }

    public void SetEnemyCount(int count)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.enemyCount = count;
            Debug.Log("Enemies: " + count);
        }

        if (enemyCountButtonImages != null)
        {
            for (int i = 0; i < enemyCountButtonImages.Length; i++)
            {
                if (enemyCountButtonImages[i] != null)
                {
                    if (i == (count - 1))
                    {
                        enemyCountButtonImages[i].color = selectedColor;
                    }
                    else
                    {
                        enemyCountButtonImages[i].color = normalColor;
                    }
                }
            }
        }
    }

    public void SetDDAModeFromSlider(float value)
    {
        if (GameSettings.Instance != null)
        {
            if (value == 0f) GameSettings.Instance.currentDDAMode = GameSettings.DDAMode.Behavioral; // Fair
            else if (value == 1f) GameSettings.Instance.currentDDAMode = GameSettings.DDAMode.Numerical; // Unfair
            
            Debug.Log("DDA Modus ist jetzt: " + GameSettings.Instance.currentDDAMode);
        }
    }

    
}