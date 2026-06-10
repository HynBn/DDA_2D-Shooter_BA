using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { PreRound, Playing, PostRound, Paused, GameOver }
    public GameState currentState;

    public int maxRounds = 3;
    public int currentRound = 1;
    public float currentRoundTime;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerSpawnPoint;
    public Transform[] enemySpawnPoints;

    public static event Action<int> OnRoundStart;
    public static event Action<float, bool> OnRoundEnd;

    public int playerScore;
    public int enemyScore;

    private GameObject currentPlayerInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = GameState.PreRound;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.Playing)
        {
            currentRoundTime += Time.deltaTime;
        }
    }

    public void StartMatch()
    {
        playerScore = 0;
        enemyScore = 0;
        currentRound = 1;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(playerScore, enemyScore, maxRounds);
        }

        StartCoroutine(PreRoundSetup());
    }

    public void EntityDeath(bool isPlayer)
    {
        if (currentState != GameState.Playing) return;

        if (isPlayer)
        {
            StartCoroutine(PostRoundSummary(false));
        }
        else
        {
            Enemy[] enemies = FindObjectsByType<Enemy>();
            if (enemies.Length == 0)
            {
                StartCoroutine(PostRoundSummary(true));
            }
        }
    }

    private IEnumerator PreRoundSetup()
    {
        currentState = GameState.PreRound;
        currentRoundTime = 0f;

        Debug.Log($"--- ROUND {currentRound} SETUP");

        if(AudioManager.Instance != null) AudioManager.Instance.PlayGameMusic();

        if(UIManager.Instance != null)
        {
            UIManager.Instance.StartCountdown();
        }

        if (currentPlayerInstance == null && playerPrefab != null && playerSpawnPoint != null)
        {
            currentPlayerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
        
        if (currentPlayerInstance != null)
        {
            currentPlayerInstance.SetActive(true);
            currentPlayerInstance.transform.position = playerSpawnPoint.position;
            currentPlayerInstance.GetComponent<Health>()?.ResetHealth();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.BindPlayerHealth(currentPlayerInstance.GetComponent<Health>());
            }

            Rigidbody2D pRb = currentPlayerInstance.GetComponent<Rigidbody2D>();
            if (pRb != null)
            {
                pRb.linearVelocity = Vector2.zero; 
                pRb.angularVelocity = 0f;
            }
        }

        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include);
        
        if (existingEnemies.Length == 0 && enemyPrefab != null && enemySpawnPoints != null)
        {
            int enemiesToSpawn = 1;
            if (GameSettings.Instance != null) enemiesToSpawn = GameSettings.Instance.enemyCount;

            int spawnLimit = Mathf.Min(enemiesToSpawn, enemySpawnPoints.Length);

            for (int i = 0; i < spawnLimit; i++)
            {
                Instantiate(enemyPrefab, enemySpawnPoints[i].position, Quaternion.identity);
            }
            existingEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include);
        }

        for (int i = 0; i < existingEnemies.Length; i++)
        {
            Enemy enemyScript = existingEnemies[i];

            if (enemySpawnPoints != null && i < enemySpawnPoints.Length)
            {
                enemyScript.transform.position = enemySpawnPoints[i].position;
            }

            enemyScript.gameObject.SetActive(true);
            enemyScript.GetComponent<Health>()?.ResetHealth();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.BindEnemyHealth(enemyScript.GetComponent<Health>(), i);
            }

            Rigidbody2D eRb = enemyScript.GetComponent<Rigidbody2D>();
            if (eRb != null)
            {
                eRb.linearVelocity = Vector2.zero;
                eRb.angularVelocity = 0f;
            }

            if (currentPlayerInstance != null)
            {
                enemyScript.player = currentPlayerInstance.transform;
            }

            if (EnemyDifficultyManager.Instance != null)
            {
                EnemyDifficultyManager.Instance.ApplyDifficultyToEnemy(enemyScript);
            }
        }

        for (int i = existingEnemies.Length; i < enemySpawnPoints.Length; i++)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.BindEnemyHealth(null, i);
            }
        }


        yield return new WaitForSeconds(3f);

        currentState = GameState.Playing;
        OnRoundStart?.Invoke(currentRound);
    }

    private IEnumerator PostRoundSummary(bool playerWon)
    {
        currentState = GameState.PostRound;
        
        if(AudioManager.Instance != null) AudioManager.Instance.PlayMenuMusic();

        Bullet[] lingeringBullets = FindObjectsByType<Bullet>(FindObjectsInactive.Include);
        foreach (Bullet b in lingeringBullets)
        {
            Destroy(b.gameObject);
        }

        float finalRoundTime = currentRoundTime;

        if (playerWon)
        {
            playerScore++;
            //Debug.Log("Player wins Round");
        }
        else
        {
            enemyScore++;
            //Debug.Log("Enemy wins Round");
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(playerScore, enemyScore, maxRounds);
        }

        OnRoundEnd?.Invoke(finalRoundTime, playerWon);

        yield return new WaitForSeconds(3f);

        int winsNeeded = (maxRounds / 2) + 1; 
        bool matchDecidedEarly = (playerScore >= winsNeeded) || (enemyScore >= winsNeeded);

        if (matchDecidedEarly || currentRound >= maxRounds)
        {
            currentState = GameState.GameOver;
            //Debug.Log($"--- MATCH OVER --- Player: {playerScore} | Enemy: {enemyScore}");
            
            // if (playerScore > enemyScore) 
            //     Debug.Log("PLAYER WINS THE MATCH");
            // else if (enemyScore > playerScore) 
            //     Debug.Log("ENEMY WINS THE MATCH");
            // else 
            //     Debug.Log("DRAW");

            if(UIManager.Instance != null) UIManager.Instance.ShowMatchResults(playerScore, enemyScore);
        }
        else
        {
            currentRound++;
            StartCoroutine(PreRoundSetup());
        }
    }

    public float GetCurrentRoundTime()
    {
        return currentRoundTime;
    }

    public void AbortMatch()
    {
        StopAllCoroutines();
        currentState = GameState.PreRound;

        if(AudioManager.Instance != null) AudioManager.Instance.PlayMenuMusic();

        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
            currentPlayerInstance = null;
        }

        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include);
        foreach (Enemy e in existingEnemies)
        {
            if (e != null)
            {
                Destroy(e.gameObject);
            }
        }
        playerScore = 0;
        enemyScore = 0;
        currentRound = 1;
        currentRoundTime = 0f;
    }
}
