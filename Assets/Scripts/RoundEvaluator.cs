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
    public float baseFastTime = 60f;
    public float fastTimePerEnemy = 60f;
    public float baseSlowTime = 110f;
    public float slowTimePerEnemy = 110f;

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

    private void EvaluateRound(float roundTime, bool playerWon)
    {
        if (DataTracker.Instance == null) return;

        float damage = DataTracker.Instance.roundDamageTaken;
        float hitRate = DataTracker.Instance.GetRoundPlayerHitRate();
        float shotsFired = DataTracker.Instance.roundShotsFired;
        int totalDashes = DataTracker.Instance.roundTotalDashes;

        int enemyCount = GameSettings.Instance != null ? GameSettings.Instance.enemyCount : 1;

        float currentFastLimit = baseFastTime + (fastTimePerEnemy * enemyCount);
        float currentSlowLimit = baseSlowTime + (slowTimePerEnemy * enemyCount);

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

        DataTracker.Instance.ResetRoundData();
    }
}