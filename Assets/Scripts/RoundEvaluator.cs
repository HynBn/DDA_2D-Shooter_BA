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
    public float fastRoundLimit = 60f;
    public float slowRoundLimit = 100f;

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

        Debug.Log($"--- ROUND SCORE --- TIME: {roundTime:F1}s | DMG: {damage} | WIN: {playerWon}");

        //AFK
        if (!playerWon && roundTime > slowRoundLimit && shotsFired == 0 && totalDashes == 0)
        {
            Debug.Log("ROUND END: AFK detected. No changes.");
            DataTracker.Instance.ResetRoundData();
            return;
        }

        //lost
        if (!playerWon)
        {
            if (roundTime < fastRoundLimit)
            {
                Debug.Log("ROUND END: Fast death. Too difficult!");
                OnRoundSignificantEasier?.Invoke();
            }
            else
            {
                Debug.Log("ROUND END: Lost normally/slowly. Make it slightly easier.");
                OnRoundModerateEasier?.Invoke();
            }
        }

        //Won
        else
        {
            // Fast round
            if (roundTime < fastRoundLimit)
            {
                if (damage >= highDamage)
                {
                    Debug.Log("ROUND END: Fast, but took a lot of damage (Glass Cannon). Flow / slightly harder.");
                    OnRoundSlightlyEasier?.Invoke(); 
                }
                else
                {
                    Debug.Log("ROUND END: Fast and little damage taken. Too easy!");
                    OnRoundSignificantHarder?.Invoke();
                }
            }

            //AVG round
            else if (roundTime >= fastRoundLimit && roundTime <= slowRoundLimit)
            {
                if (damage >= highDamage)
                {
                    Debug.Log("ROUND END: Average time, high damage. Slightly too hard.");
                    OnRoundSlightlyEasier?.Invoke();
                }
                else if (damage <= lowDamage)
                {
                    Debug.Log("ROUND END: Average time, low damage. Slightly too easy.");
                    OnRoundModerateHarder?.Invoke();
                }
                else
                {
                    Debug.Log("ROUND END: Perfekter FLOW STATE. Keine Änderung.");
                }
            }

            //Long Round
            else
            {
                if (damage >= highDamage)
                {
                    Debug.Log("ROUND END: Langsam und viel Schaden. Zu schwer.");
                    OnRoundModerateEasier?.Invoke();
                }
                else if (damage <= lowDamage)
                {
                    Debug.Log("ROUND END: Langsam, kaum Schaden. Beide treffen schlecht. Gegner wird ruhiger/präziser.");
                    OnRoundLessStrafe?.Invoke();
                    OnRoundMoreAccurate?.Invoke();
                }
                else
                {
                    if (hitRate < lowHitRateLimit) 
                    {
                        Debug.Log("ROUND END: Miese HitRate macht Runde langsam. Strafe verringern.");
                        OnRoundLessStrafe?.Invoke();
                    }
                    else
                    {
                        Debug.Log("ROUND END: Langer, ausgeglichener Ausdauer-Kampf (Flow). Keine Änderung.");
                    }
                }
            }
        }

        DataTracker.Instance.ResetRoundData();
    }
}