using UnityEngine;
using System;

public class DataEvaluator : MonoBehaviour
{
    //General Difficulty Adjustment
    public static event Action OnSignificantHarder;
    public static event Action OnModerateHarder;
    public static event Action OnSlightlyHarder;
    public static event Action OnSignificantEasier;
    public static event Action OnModerateEasier;
    public static event Action OnSlightlyEasier;

    //Aim Accuracy adjustment
    public static event Action OnMoreAccurate;
    public static event Action OnSlightMoreAccurate;
    public static event Action OnLessAccurate;
    public static event Action OnSlightLessAccurate;

    //Movement Adjustment
    public static event Action OnMoreStrafe;
    public static event Action OnLessStrafe;

    public float evaluationInterval = 5f; //in sec
    private float timer = 0f;

    public float highHitRate = 0.5f;
    public float midHitRate = 0.25f;
    public float lowHitRate = 0.1f;

    public float highDamage = 50f;
    public float midDamage = 30f;
    public float lowDamage = 10f;

    public int nearHitsLimit = 2;
    public float dashEfficiencyLimit = 0.5f;

    [Range(0f, 1f)]
    public float targetPlayerHitRate = 0.2f;

    public float tolerance = 0.05f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= evaluationInterval)
        {
            EvaluatePlayerPerformance();
            timer = 0f;
        }
    }

    private void EvaluatePlayerPerformance()
    {
        if (DataTracker.Instance == null) return;

        float hitRate = DataTracker.Instance.GetPlayerHitRate();
        float damage = DataTracker.Instance.playerDamageTaken;
        int totalDashes = DataTracker.Instance.totalDashes;
        int perfectDodges = DataTracker.Instance.perfectDodges;
        int nearHits = DataTracker.Instance.nearHits;
        float shotsFired = DataTracker.Instance.playerShotsFired;

        float dashEfficiency = totalDashes > 0 ? (float)perfectDodges / totalDashes : 0f;

        Debug.Log($"Hit Rate: {hitRate * 100}%; Dash Efficiency: {dashEfficiency *100}%");

        //AFK
        if (shotsFired == 0 && damage == 0 && totalDashes == 0)
        {
            Debug.Log("no Action detected, no changes");
        }

        //Harder 
        else if (hitRate >= highHitRate && damage <= lowDamage)
        {
            OnSignificantHarder?.Invoke();
            Debug.Log("TOO EASY: SIG INCREASE Difficulty");
        }
        else if (hitRate >= midHitRate && damage <= lowDamage)
        {
            OnModerateHarder?.Invoke();
            Debug.Log("EASY: MOD INCREASE Difficulty");
        }
        else if (hitRate >= lowHitRate && damage <= lowDamage)
        {
            OnSlightlyHarder?.Invoke();
            Debug.Log("GOOD: SLI INCREASE Difficulty");
        }
        else if (hitRate >= midHitRate && damage <= lowDamage && dashEfficiency >= dashEfficiencyLimit)
        {
            OnMoreAccurate?.Invoke();
            Debug.Log("EASY (Agile): More accurate aim");
        }
        else if (hitRate >= midHitRate && damage <= midDamage && nearHits <= nearHitsLimit)
        {
            OnSlightMoreAccurate?.Invoke();
            Debug.Log("GOOD (Agile): SLI More accurate aim");
        }

        //Easier
        else if (hitRate == 0 && shotsFired > 0 && damage >= highDamage)
        {
            OnSignificantEasier?.Invoke();
            Debug.Log("TOO HARD: SIG DECREASE Difficulty");
        }
        else if (hitRate <= lowHitRate && damage >= highDamage)
        {
            OnModerateEasier?.Invoke();
            Debug.Log("HARD: MOD DECREASE Difficulty");
        }
        else if (hitRate >= lowHitRate && damage >= midDamage)
        {
            OnSlightlyEasier?.Invoke();
            Debug.Log("DECENT: SLI DECREASE Difficulty");
        }
        else if (hitRate >= lowHitRate && damage <= midDamage && dashEfficiency < dashEfficiencyLimit)
        {
            OnLessAccurate?.Invoke();
            Debug.Log("HARD (Agile): Less Accurate Aim");
        }
        else if (hitRate >= midHitRate && damage <= lowDamage && nearHits > nearHitsLimit)
        {
            OnSlightLessAccurate?.Invoke();
            Debug.Log("DECENT (Agile): SLI Less Accurate Aim");
        }

        //Flow-State
        else
        {
            Debug.Log("FLOW STATE: NO Changes");
        }
        DataTracker.Instance.ResetInterval();
    }
}
