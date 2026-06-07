using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

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
    public TextMeshProUGUI maxScoreText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemy1HealthText;
    public TextMeshProUGUI enemy2HealthText;
    public TextMeshProUGUI enemy3HealthText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI ddaAlertText;

    [Header("Enemy Selection Visuals")]
    public Image[] enemyCountButtonImages;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.red;

    [Header("Match Results")]
    public TextMeshProUGUI matchResultText;
    public TextMeshProUGUI finalScoreText;
    
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
        Time.timeScale = 1f;
        isPaused = false;
        if(GameManager.Instance != null) GameManager.Instance.AbortMatch();

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

    public void StartCountdown()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            StartCoroutine(CountdownRoutine());
        }
    }

    private IEnumerator CountdownRoutine() 
    {
        string[] steps = { "3", "2", "1", "FIGHT!" };

        foreach (string step in steps)
        {
            countdownText.text = step;
            countdownText.transform.localScale = Vector3.one * 1.5f;

            float timer = 0f;
            while (timer < 0.2f) 
            {
                timer += Time.deltaTime;
                countdownText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, timer / 0.2f);
                yield return null;
            }

            yield return new WaitForSeconds(0.8f);
        }

        countdownText.gameObject.SetActive(false);
    }

    public void UpdateScoreText(int playerScore, int enemyScore, int maxScore)
    {
        if (eScoreText != null && pScoreText != null)
        {
            pScoreText.text = playerScore.ToString();
            eScoreText.text = enemyScore.ToString();
            maxScoreText.text = "Best of " + maxScore.ToString();
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
        if (playerHealthText != null) playerHealthText.text = "Player HP " + current.ToString("F0") + " / " + max.ToString("F0");
    }

    private System.Action<float, float> UpdateEnemyHealthText(int index, float current, float max)
    {
        return (current, max) =>
        {
            if (enemyHealthTexts[index] != null) enemyHealthTexts[index].text = "Enemy " + (index + 1) + " HP " + current.ToString("F0") + " / " + max.ToString("F0");
        };
    }

    public void ShowDDAAlert(string message, Color alertColor)
    {
        if (ddaAlertText != null)
        {
            StopCoroutine("DDAAlertRoutine");
            StartCoroutine(DDAAlertRoutine(message, alertColor));
        }
    }

    private IEnumerator DDAAlertRoutine(string message, Color alertColor) 
    {
        ddaAlertText.text = message;
        ddaAlertText.color = alertColor;
        ddaAlertText.gameObject.SetActive(true);

        ddaAlertText.transform.localScale = Vector3.one * 1.5f;
        float timer = 0f;
        while (timer < 0.2f)        
        {
            timer += Time.deltaTime;
            ddaAlertText.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, timer / 0.2f);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        ddaAlertText.gameObject.SetActive(false);
    }

    public void SetDDAModeFromSlider(float value)
    {
        if (GameSettings.Instance != null)
        {
            if (value == 0f) GameSettings.Instance.currentDDAMode = GameSettings.DDAMode.Behavioral; // Fair
            else if (value == 1f) GameSettings.Instance.currentDDAMode = GameSettings.DDAMode.Numerical; // Unfair
            
            Debug.Log("DDA Mode is: " + GameSettings.Instance.currentDDAMode);
        }
    }
    
    public void ShowMatchResults(int pScore, int eScore)
    {
        ShowPanel(gameOverPanel);
        
        if(matchResultText != null)
        {
            if (pScore > eScore) 
            {
                matchResultText.text = "VICTORY!";
                matchResultText.color = Color.darkGreen;
            }
            else if (pScore < eScore) 
            {
                matchResultText.text = "DEFEAT!";
                matchResultText.color = Color.red;
            }
            else 
            {
                matchResultText.text = "DRAW!";
                matchResultText.color = Color.yellow;
            }
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + pScore + " - " + eScore;
        }
    }
}