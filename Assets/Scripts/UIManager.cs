using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
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
    
    // (Später fügen wir hier noch die Image-Lebensleisten hinzu)

    private bool isPaused = false;

    void OnEnable()
    {
        GameManager.OnRoundStart += ShowHUDForRound;
        GameManager.OnRoundEnd += HideHUDForSummary;
    }

    void OnDisable()
    {
        GameManager.OnRoundStart -= ShowHUDForRound;
        GameManager.OnRoundEnd -= HideHUDForSummary;
    }

    void Start()
    {
        // Beim Spielstart zeigen wir NUR das Hauptmenü
        ShowPanel(mainMenuPanel);
            }

    void Update()
    {
        // HUD aktualisieren, wenn das Spiel läuft
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            if (roundTimerText != null)
            {
                roundTimerText.text = GameManager.Instance.currentRoundTime.ToString("F0");
            }
        }

        // NEU: Pause-Logik über das neue Input System (ESC-Taste)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            TogglePause();
        }
    }

    // --- BUTTON METHODEN FÜR DAS MENÜ ---

    public void StartGame()
    {
        // GameManager muss eine Methode haben, um das Spiel (Runde 1) wirklich zu starten.
        // Aktuell startet er ja direkt in Start(). Das ändern wir gleich!
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
        Debug.Log("Spiel wird beendet...");
        Application.Quit();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // Spielzeit einfrieren
            ShowPanel(pausePanel);
        }
        else
        {
            Time.timeScale = 1f; // Spielzeit weiterlaufen lassen
            ShowPanel(hudPanel);
        }
    }

    // --- INTERNE HILFSMETHODEN ---

    private void ShowHUDForRound(int roundNumber)
    {
        if (roundCounterText != null) roundCounterText.text = "Round " + roundNumber;
        ShowPanel(hudPanel);
    }

    private void HideHUDForSummary(float time, bool playerWon)
    {
        // Optional: Hier könntest du ein kleines "Round Over" Zwischen-Panel einblenden
    }

    private void ShowPanel(GameObject panelToShow)
    {
        // Alle Panels ausschalten
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (selectPanel) selectPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Nur das gewünschte Panel einschalten
        if (panelToShow) panelToShow.SetActive(true);
    }

    // --- METHODEN FÜR DAS SELECT PANEL ---

    // Diese Methode rufst du über Buttons auf (z.B. ein Button für "1", einer für "2", etc.)
    public void SetEnemyCount(int count)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.enemyCount = count;
            Debug.Log("Gegner-Anzahl gesetzt auf: " + count);
        }
    }

    // Diese Methode verknüpfst du mit dem "OnValueChanged" Event eines Sliders
    // WICHTIG: Der Slider muss in Unity auf "Whole Numbers" (Ganze Zahlen) gestellt sein, Min = 0, Max = 1
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