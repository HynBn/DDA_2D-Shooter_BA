using UnityEngine;
using System;

public class RoundEvaluator : MonoBehaviour
{
    public static event Action OnRoundSignificantHarder;
    public static event Action OnRoundModerateHarder;

    public static event Action OnRoundSignificantEasier;
    public static event Action OnRoundModerateEasier;
    public static event Action OnRoundSlightlyEasier;

    public static event Action OnRoundLessStrafe;
    public static event Action OnRoundMoreAccurate;

    [Header("Time Thresholds")]
    public float baseFastTime = 100f;
    public float fastTimePerEnemy = 100f;
    public float baseSlowTime = 200f;
    public float slowTimePerEnemy = 200f;

    [Header("Damage Thresholds (Round based)")]
    public float highDamage = 70f; 
    public float lowDamage = 20f;

    [Header("Secondary Thresholds")]
    public float lowHitRateLimit = 0.2f;

    void OnEnable()
    {
        GameManager.OnRoundEnd += EvaluateRound;
    }

    void OnDisable()
    {
        GameManager.OnRoundEnd -= EvaluateRound;
    }

    private bool IsDDAEnabled()
    {
        return GameSettings.Instance != null && GameSettings.Instance.currentDifficultySystem == GameSettings.DifficultySystem.DDA;    
    }

    private void EvaluateRound(float roundTime, bool playerWon)
    {
        if (DataTracker.Instance == null) return;

        if (IsDDAEnabled()) EvaluateDDAAdjustment(roundTime, playerWon);

        if (DataExporter.Instance != null && UIManager.isTelemetryAllowed) DataExporter.Instance.ExportRoundData(roundTime, playerWon);

        DataTracker.Instance.ResetRoundData();
    }

    private void EvaluateDDAAdjustment(float roundTime, bool playerWon)
    {
        float damage = DataTracker.Instance.roundDamageTaken;
        float hitRate = DataTracker.Instance.GetRoundPlayerHitRate();
        float shotsFired = DataTracker.Instance.roundShotsFired;
        int totalDashes = DataTracker.Instance.roundTotalDashes;

        int enemyCount = GameSettings.Instance != null ? Mathf.Max(1, GameSettings.Instance.enemyCount) : 1;
        int additionalEnemies = Mathf.Max(0, enemyCount - 1);

        float currentFastLimit = baseFastTime + fastTimePerEnemy * additionalEnemies;
        float currentSlowLimit = baseSlowTime + slowTimePerEnemy * additionalEnemies;

        //AFK
        if (!playerWon && roundTime > currentSlowLimit && shotsFired == 0 && totalDashes == 0)
        {
            DataTracker.Instance.ResetRoundData();
            Debug.Log("ROUND END - AFK: No changes.");
            return;
        }

        //lost
        if (!playerWon)
        {
            if (roundTime < currentFastLimit)
            {
                OnRoundSignificantEasier?.Invoke();
                Debug.Log("ROUND END - FAST LOST: Sig Easier");
            }
            else
            {
                OnRoundModerateEasier?.Invoke();
                Debug.Log("ROUND END - AVG LOST: Mod Easier");
            }
        }

        //Won
        else
        {
            // Fast round
            if (roundTime < currentFastLimit)
            {
                if (damage >= highDamage)
                {
                    Debug.Log("ROUND END - FAST WON - High Dmg: FLOW");
                }
                else
                {
                    OnRoundSignificantHarder?.Invoke();
                    Debug.Log("ROUND END - FAST WON - AVG Dmg: Sig harder");
                }
            }

            //AVG round
            else if (roundTime >= currentFastLimit && roundTime <= currentSlowLimit)
            {
                if (damage >= highDamage)
                {
                    OnRoundSlightlyEasier?.Invoke();
                    Debug.Log("ROUND END - MID WON - High Dmg: Sli Easier");
                }
                else if (damage <= lowDamage)
                {
                    OnRoundModerateHarder?.Invoke();
                    Debug.Log("ROUND END - MID WON - Low Dmg: Mod Harder");
                }
                else
                {
                    Debug.Log("ROUND END - MID WIN - AVG Dmg: FLOW");
                }
            }

            //Long Round
            else
            {
                if (damage >= highDamage)
                {
                    OnRoundModerateEasier?.Invoke();
                    Debug.Log("ROUND END - LONG WIN - High Dmg: Mod Easier");
                }
                else if (damage <= lowDamage)
                {
                    OnRoundLessStrafe?.Invoke();
                    OnRoundMoreAccurate?.Invoke();
                    Debug.Log("ROUND END - LONG WIN - Low Dmg: Less Strafe + More Accuracy");
                }
                else
                {
                    if (hitRate < lowHitRateLimit) 
                    {
                        OnRoundLessStrafe?.Invoke();
                        Debug.Log("ROUND END - LONG WIN - Low Hit Rate: Less Strafe");
                    }
                    else
                    {
                        Debug.Log("ROUND END - LONG WIN: FLOW");
                    }
                }
            }
        }

        if (DataExporter.Instance != null && UIManager.isTelemetryAllowed)
        {
            DataExporter.Instance.ExportRoundData(roundTime, playerWon);
        }
    }
}