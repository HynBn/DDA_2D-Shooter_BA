using System.Net;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyDifficultyManager : MonoBehaviour
{
    //Modes werden auch erst später eingefügt
    // public enum DDAMode {FairStats, UnfairStats};
    // public DDAMode currentMode;

    // [Header("Fair Parameters")]
    public float dodgeChance = 0.1f;
    public float attackCooldown = 0.1f;

    private Health myHealth;
    private LocalEnemyTracker myLocalTracker;
    private Enemy myEnemyScript;

    void Awake()
    {
        myHealth = GetComponent<Health>();
        myLocalTracker = GetComponent<LocalEnemyTracker>();
        myEnemyScript = GetComponent<Enemy>();
    }

    // void OnEnable()
    // {
    //     DataEvaluator.OnIncreaseDifficulty += MakeHarder;
    //     DataEvaluator.OnDecreaseDifficulty += MakeEasier;
    // }

    // void OnDisable()
    // {
    //     DataEvaluator.OnIncreaseDifficulty -= MakeHarder;
    //     DataEvaluator.OnDecreaseDifficulty -= MakeEasier;        
    // }

    private void MakeHarder()
    {
        if (myEnemyScript == null) return;

        myEnemyScript.awareRadius -= 0.5f;
        myEnemyScript.strafeSpeed += 0.5f;
        myEnemyScript.spread -= 0.5f;

        //Grenzen werden später eingefügt
        // myEnemyScript.awareRadius = Mathf.Max(1.5f, myEnemyScript.awareRadius);

        Debug.Log($"SCHWERER: AwareRadius={myEnemyScript.awareRadius:F1}, Strafe={myEnemyScript.strafeSpeed:F1}");
    }

    private void MakeEasier()
    {
        if (myEnemyScript == null) return;

        myEnemyScript.awareRadius += 0.5f;
        myEnemyScript.strafeSpeed -= 0.5f;
        myEnemyScript.spread += 0.5f;

        //Grenzen werden später eingefügt
        // myEnemyScript.awareRadius = Mathf.Min(8f, myEnemyScript.awareRadius);

        Debug.Log($"LEICHTER: AwareRadius={myEnemyScript.awareRadius:F1}, Strafe={myEnemyScript.strafeSpeed:F1}");
    }
}
