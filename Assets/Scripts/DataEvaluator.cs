using UnityEngine;
using System;
using UnityEngine.InputSystem.Composites;

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
    public static event Action OnSlightMoreStrafe;
    public static event Action OnLessStrafe;
    public static event Action OnSlightLessStrafe;

    [Header("Scan Interval")]
    public float evaluationInterval = 5f; //in sec
    private float timer = 0f;

    [Header("Hit Rate Limits")]
    public float highHitRate = 0.4f;
    public float midHitRate = 0.25f;
    public float lowHitRate = 0.15f;

    [Header("Damage Taken Limits")]
    public float highDamage = 40f;
    public float midDamage = 30f;
    public float lowDamage = 10f;

    [Header("Movement Limits")]
    public int nearHitsLimit = 3;
    public float dashEfficiencyLimit = 0.5f;

    private Transform playerTransform;

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= evaluationInterval)
        {
            EvaluatePlayerPerformance();
            timer = 0f;
        }
    }

    private float GetHealthDiscrepancy()
    {
        if(playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null) playerTransform = playerObj.transform;
            else return 0f;
        }

        Health pHealth = playerTransform.GetComponent<Health>();
        if(pHealth == null || pHealth.maxHealth <= 0f) return 0f;
        float playerHPPercent = pHealth.currentHealth / pHealth.maxHealth;

        float highestEnemyHPPercent = 0f;
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        foreach(Enemy e in activeEnemies)
        {
            if(e != null)
            {
                Health eHealth = e.GetComponent<Health>();
                if(eHealth != null && eHealth.maxHealth > 0)
                {
                    float hpPercent = eHealth.currentHealth / eHealth.maxHealth;
                    if(hpPercent > highestEnemyHPPercent) highestEnemyHPPercent = hpPercent;
                }
            }
        }
        return playerHPPercent - highestEnemyHPPercent;
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

        float discrepancy = GetHealthDiscrepancy();

        //BIG Health Discrepancy, ignore the rest of the adaptation
        if(discrepancy >= 0.3f)
        {
            OnSignificantHarder?.Invoke();
            return;
        }

        if(discrepancy <= -0.3f)
        {
            OnSignificantEasier?.Invoke();
            return;
        }

        //AFK
        if (shotsFired == 0 && damage == 0 && totalDashes == 0)
        {
            Debug.Log("AFK: No changes");
        }

        //+++ INCREASE
        else if (hitRate >= highHitRate && damage <= lowDamage)
        {
            if (nearHits <= nearHitsLimit)
            {
                OnMoreAccurate?.Invoke();
                OnMoreStrafe?.Invoke();
                OnSignificantHarder?.Invoke();
                Debug.Log("INCREASE 1.1: Sig + Accuracy + Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnSlightMoreAccurate?.Invoke();
                OnMoreStrafe?.Invoke();
                OnSignificantHarder?.Invoke();
                Debug.Log("INCREASE 1.2: Sig + Sli-Accuracy + Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency >= dashEfficiencyLimit)
            {
                OnSlightMoreAccurate?.Invoke();
                OnMoreStrafe?.Invoke();
                OnModerateHarder?.Invoke();
                Debug.Log("INCREASE 1.3: Mod + Sli-Accuracy + Strafe");
            }
            else
            {
                OnSignificantHarder?.Invoke();
                Debug.Log("INCREASE 1: Sig");
            }
        }
        else if (hitRate >= midHitRate && damage <= lowDamage)
        {
            if (nearHits <= nearHitsLimit)
            {
                OnMoreAccurate?.Invoke();
                OnMoreStrafe?.Invoke();
                OnModerateHarder?.Invoke();
                Debug.Log("INCREASE 2.1: Mod + Accuracy + Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnMoreStrafe?.Invoke();
                OnModerateHarder?.Invoke();
                Debug.Log("INCREASE 2.2: Mod + Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency >= dashEfficiencyLimit)
            {
                OnMoreStrafe?.Invoke();
                OnSlightlyHarder?.Invoke();
                Debug.Log("INCREASE 2.3: Sli + Strafe");
            }
            else
            {
                OnModerateHarder?.Invoke();
                Debug.Log("INCREASE 2: Sig");
            }
        }
        else if (hitRate >= lowHitRate && damage <= lowDamage)
        {
            if (nearHits <= nearHitsLimit)
            {
                OnSlightMoreAccurate?.Invoke();
                OnSlightlyHarder?.Invoke();
                Debug.Log("INCREASE 3.1: Sli + Sli-Accuracy");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnSlightMoreStrafe?.Invoke();
                OnSlightlyHarder?.Invoke();
                Debug.Log("INCREASE 3.2: Sli + Sli-Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency >= dashEfficiencyLimit)
            {
                OnSlightMoreStrafe?.Invoke();
                Debug.Log("INCREASE 3.3: Sli-Strafe");
            }
            else
            {
                OnSlightlyHarder?.Invoke();
                Debug.Log("INCREASE 3: Sli");
            }
        }
        else if (hitRate >= highHitRate && damage > lowDamage && damage <= midDamage)
        {
            OnMoreStrafe?.Invoke();
            OnModerateHarder?.Invoke();
            Debug.Log("INCREASE 4: Mod + Strafe");
        }
        else if (hitRate >= highHitRate && damage > midDamage && damage < highDamage)
        {
            OnLessAccurate?.Invoke();
            OnSlightlyHarder?.Invoke();
            Debug.Log("INCREASE 5: Sli - Accuracy");
        }
        else if (hitRate >= midHitRate && damage > lowDamage && damage <= midDamage)
        {
            if(nearHits <= nearHitsLimit)
            {
                OnSlightMoreAccurate?.Invoke();
                Debug.Log("INCREASE 6: Sli-Accuracy");
            }
            else
            {
                Debug.Log("FLOW: No changes");
            }
        }

        //--- DECREASE
        else if (hitRate <= lowHitRate && damage >= highDamage)
        {
            if (nearHits >= nearHitsLimit)
            {
                OnLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnSignificantEasier?.Invoke();
                Debug.Log("DECREASE 1.1: Sig - Accuracy - Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy <  -0.2 && discrepancy > -0.3)
            {
                OnLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnSignificantEasier?.Invoke();
                Debug.Log("DECREASE 1.2: Sig - Accuracy - Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnSlightMoreAccurate?.Invoke();
                OnSlightLessStrafe?.Invoke();
                Debug.Log("DECREASE 1.3: Sli-Accuracy - Sli-Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency <= dashEfficiencyLimit)
            {
                OnSlightLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnSignificantEasier?.Invoke();
                Debug.Log("DECREASE 1.4: Sig - Sli-Accuracy - Strafe");
            }
            else
            {
                OnSignificantEasier?.Invoke();
                Debug.Log("DECREASE 1: Sig");
            }
        }
        else if (hitRate > lowHitRate && hitRate < highHitRate && damage >= highDamage)
        {
            if (nearHits >= nearHitsLimit)
            {
                OnLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnModerateEasier?.Invoke();
                Debug.Log("DECREASE 2.1: Mod - Accuracy - Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy <  -0.2 && discrepancy > -0.3)
            {
                OnLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnModerateEasier?.Invoke();
                Debug.Log("DECREASE 2.2: Mod - Accuracy - Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnMoreAccurate?.Invoke();
                OnSlightLessStrafe?.Invoke();
                Debug.Log("DECREASE 2.3: Accuracy - Sli-Strafe");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency <= dashEfficiencyLimit)
            {
                OnSlightLessAccurate?.Invoke();
                OnSlightLessStrafe?.Invoke();
                OnModerateEasier?.Invoke();
                Debug.Log("DECREASE 2.4: Mod - Sli-Accuracy - Sli-Strafe");
            }
            else
            {
                OnModerateEasier?.Invoke();
                Debug.Log("DECREASE 2: Mod");
            }
        }
        else if (hitRate >= highHitRate && damage >= highDamage)
        {
            if (nearHits >= nearHitsLimit)
            {
                OnSlightLessAccurate?.Invoke();
                OnSlightLessStrafe?.Invoke();
                OnSlightlyEasier?.Invoke();
                Debug.Log("DECREASE 3.1: Sli - Sli-Accuracy - Sli-Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy <  -0.2 && discrepancy > -0.3)
            {
                OnLessAccurate?.Invoke();
                OnLessStrafe?.Invoke();
                OnSlightlyEasier?.Invoke();
                Debug.Log("DECREASE 3.2: Sli - Accuracy - Strafe");
            }
            else if (dashEfficiency <= dashEfficiencyLimit && discrepancy > 0.2 && discrepancy < 0.3)
            {
                OnSlightMoreAccurate?.Invoke();
                Debug.Log("DECREASE 3.3: Sli-Accuracy");
            }
            else if (dashEfficiency >= dashEfficiencyLimit && discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            }
            else if (dashEfficiency <= dashEfficiencyLimit)
            {
                OnSlightLessAccurate?.Invoke();
                OnMoreStrafe?.Invoke();
                OnSlightlyEasier?.Invoke();
                Debug.Log("DECREASE 3.4: Sli - Sli-Accuracy + Strafe");
            }
            else
            {
                OnSlightlyEasier?.Invoke();
                Debug.Log("DECREASE 3: Sli");
            }
        }

        //BALANCE
        else if (hitRate >= midHitRate && damage > midDamage)
        {
            OnSlightLessAccurate?.Invoke();
            OnMoreStrafe?.Invoke();
            Debug.Log("BALANCE 1: - Sli Accuracy + Strafe");
        }
        else if (hitRate < midHitRate && damage < midDamage)
        {
            if(discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            } else
            {
                OnSlightMoreAccurate?.Invoke();
                OnSlightLessStrafe?.Invoke();
                Debug.Log("BALANCE 2: Sli Accuracy - Sli-Strafe ");
            }
        }
        else if (hitRate <= lowHitRate && damage <= lowDamage)
        {
            OnMoreAccurate?.Invoke();
            OnLessStrafe?.Invoke();
            Debug.Log("BALANCE 3: Accuarcy - Strafe");
        }
        else if (hitRate >= midHitRate && damage <= midDamage && nearHits >= nearHitsLimit)
        {
            if(discrepancy <= 0.1 && discrepancy >= -0.1)
            {
                Debug.Log("FLOW: No changes");
            } else
            {
                OnSlightMoreStrafe?.Invoke();
                OnSlightMoreAccurate?.Invoke();
                Debug.Log("BALANCE 4: Sli Strafe + Sli Accuracy");
            }
        }

        //Flow-State
        else
        {
            Debug.Log("FLOW STATE: NO Changes");
        }
        DataTracker.Instance.ResetInterval();
    }
}
