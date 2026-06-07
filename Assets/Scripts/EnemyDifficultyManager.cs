using UnityEngine;

public class EnemyDifficultyManager : MonoBehaviour
{
    public static EnemyDifficultyManager Instance { get; private set; }

    [Header("Fair Round Parameters (Behavioral)")]
    public float roundAwareRadius = 8f;
    public float roundAttackRange = 7f;
    public float roundRetreatRange = 4f;
    public float roundSpread = 15f;
    public float roundDashSpeed = 10f;
    public float roundRetreatDashChance = 0.2f;
    public float roundStrafeSpeed = 3f;
    public float roundStrafeChangeInterval = 1.5f;
    public float roundDodgeChance = 0.8f;

    [Header("Unfair Round Parameters (Numerical)")]
    public float roundMoveSpeed = 3f;
    public float roundDashCooldown = 2f;
    public float roundBulletForce = 10f;
    public float roundFireRate = 2f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        DataEvaluator.OnSignificantHarder += MakeSignificantHarder;
        DataEvaluator.OnModerateHarder += MakeModerateHarder;
        DataEvaluator.OnSlightlyHarder += MakeSlightlyHarder;
        DataEvaluator.OnSignificantEasier += MakeSignificantEasier;
        DataEvaluator.OnModerateEasier += MakeModerateEasier;
        DataEvaluator.OnSlightlyEasier += MakeSlightlyEasier;
        DataEvaluator.OnMoreAccurate += MakeMoreAccurate;
        DataEvaluator.OnSlightMoreAccurate += MakeSlightlyMoreAccurate;
        DataEvaluator.OnLessAccurate += MakeLessAccurate;
        DataEvaluator.OnSlightLessAccurate += MakeSlightlyLessAccurate;
        DataEvaluator.OnMoreStrafe += MakeMoreStrafe;
        DataEvaluator.OnLessStrafe += MakeLessStrafe;

        RoundEvaluator.OnRoundSignificantHarder += MakeSignificantHarder;
        RoundEvaluator.OnRoundModerateHarder += MakeModerateHarder;
        RoundEvaluator.OnRoundSignificantEasier += MakeSignificantEasier;
        RoundEvaluator.OnRoundModerateEasier += MakeModerateEasier;
        RoundEvaluator.OnRoundSlightlyEasier += MakeSlightlyEasier;
        RoundEvaluator.OnRoundLessStrafe += MakeLessStrafe;
        RoundEvaluator.OnRoundMoreAccurate += MakeMoreAccurate;
    }

    void OnDisable()
    {
        DataEvaluator.OnSignificantHarder -= MakeSignificantHarder;
        DataEvaluator.OnModerateHarder -= MakeModerateHarder;
        DataEvaluator.OnSlightlyHarder -= MakeSlightlyHarder;
        DataEvaluator.OnSignificantEasier -= MakeSignificantEasier;
        DataEvaluator.OnModerateEasier -= MakeModerateEasier;
        DataEvaluator.OnSlightlyEasier -= MakeSlightlyEasier;
        DataEvaluator.OnMoreAccurate -= MakeMoreAccurate;
        DataEvaluator.OnSlightMoreAccurate -= MakeSlightlyMoreAccurate;
        DataEvaluator.OnLessAccurate -= MakeLessAccurate;
        DataEvaluator.OnSlightLessAccurate -= MakeSlightlyLessAccurate;
        DataEvaluator.OnMoreStrafe -= MakeMoreStrafe;
        DataEvaluator.OnLessStrafe -= MakeLessStrafe; 

        RoundEvaluator.OnRoundSignificantHarder -= MakeSignificantHarder;
        RoundEvaluator.OnRoundModerateHarder -= MakeModerateHarder;
        RoundEvaluator.OnRoundSignificantEasier -= MakeSignificantEasier;
        RoundEvaluator.OnRoundModerateEasier -= MakeModerateEasier;
        RoundEvaluator.OnRoundSlightlyEasier -= MakeSlightlyEasier;
        RoundEvaluator.OnRoundLessStrafe -= MakeLessStrafe;
        RoundEvaluator.OnRoundMoreAccurate -= MakeMoreAccurate;
    }

    public void ApplyDifficultyToEnemy(Enemy myEnemyScript)
    {
        if (myEnemyScript == null) return;

        // 1. Fair Parameters
        myEnemyScript.awareRadius = roundAwareRadius;
        myEnemyScript.attackRange = roundAttackRange;
        myEnemyScript.retreatRange = roundRetreatRange;
        myEnemyScript.spread = roundSpread;
        myEnemyScript.dashSpeed = roundDashSpeed;
        myEnemyScript.retreatDashChance = roundRetreatDashChance;
        myEnemyScript.strafeSpeed = roundStrafeSpeed;
        myEnemyScript.strafeChangeInterval = roundStrafeChangeInterval;
        myEnemyScript.dodgeChance = roundDodgeChance;

        // 2. Unfair Parameters
        myEnemyScript.moveSpeed = roundMoveSpeed;
        myEnemyScript.dashCooldown = roundDashCooldown;
        myEnemyScript.bulletForce = roundBulletForce;
        myEnemyScript.fireRate = roundFireRate;
    }


    private bool IsNumericalMode()
    {
        return GameSettings.Instance != null && GameSettings.Instance.currentDDAMode == GameSettings.DDAMode.Numerical;
    }

    private void MakeSignificantHarder()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed += 1f;
            roundFireRate -= 0.5f;       
            roundDashCooldown -= 0.5f;   
            roundBulletForce += 2f;
        }
        else
        {
            roundAttackRange += 1f;
            roundRetreatRange += 1f;
            roundSpread -= 2f;           
            roundDodgeChance += 0.1f;
            roundDashSpeed += 1f;
            roundAwareRadius -= 0.5f;
            roundStrafeSpeed += 0.5f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeModerateHarder()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed += 0.5f;
            roundFireRate -= 0.25f;
            roundDashCooldown -= 0.25f;
            roundBulletForce += 1f;
        }
        else
        {
            roundAttackRange += 0.5f;
            roundRetreatRange += 0.5f;
            roundSpread -= 1f;
            roundDodgeChance += 0.05f;
            roundDashSpeed += 0.5f;
            roundAwareRadius -= 0.25f;
            roundStrafeSpeed += 0.25f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeSlightlyHarder()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed += 0.25f;
            roundFireRate -= 0.1f;
            roundDashCooldown -= 0.1f;
            roundBulletForce += 0.5f;
        }
        else
        {
            roundAttackRange += 0.25f;
            roundRetreatRange += 0.25f;
            roundSpread -= 0.5f;
            roundDodgeChance += 0.025f;
            roundDashSpeed += 0.25f;
            roundAwareRadius -= 0.1f;
            roundStrafeSpeed += 0.1f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSignificantEasier()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed -= 1f;
            roundFireRate += 0.5f;       
            roundDashCooldown += 0.5f;
            roundBulletForce -= 2f;
        }
        else
        {
            roundAttackRange -= 1f;
            roundRetreatRange -= 1f;
            roundSpread += 2f;
            roundDodgeChance -= 0.1f;
            roundDashSpeed -= 1f;
            roundAwareRadius += 0.5f;
            roundStrafeSpeed -= 0.5f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeModerateEasier()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed -= 0.5f;
            roundFireRate += 0.25f;
            roundDashCooldown += 0.25f;
            roundBulletForce -= 1f;
        }
        else
        {
            roundAttackRange -= 0.5f;
            roundRetreatRange -= 0.5f;
            roundSpread += 1f;
            roundDodgeChance -= 0.05f;
            roundDashSpeed -= 0.5f;
            roundAwareRadius += 0.25f;
            roundStrafeSpeed -= 0.25f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSlightlyEasier()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed -= 0.25f;
            roundFireRate += 0.1f;
            roundDashCooldown += 0.1f;
            roundBulletForce -= 0.5f;
        }
        else
        {
            roundAttackRange -= 0.25f;
            roundRetreatRange -= 0.25f;
            roundSpread += 0.5f;
            roundDodgeChance -= 0.025f;
            roundDashSpeed -= 0.25f;
            roundAwareRadius += 0.1f;
            roundStrafeSpeed -= 0.1f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeMoreAccurate()
    {
        if (IsNumericalMode()) roundFireRate -= 0.2f; 
        else roundSpread -= 0.75f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeSlightlyMoreAccurate()
    {
        if (IsNumericalMode()) roundFireRate -= 0.1f;
        else roundSpread -= 0.25f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeLessAccurate()
    {
        if (IsNumericalMode()) roundFireRate += 0.2f; 
        else roundSpread += 0.75f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSlightlyLessAccurate()
    {
        if (IsNumericalMode()) roundFireRate += 0.1f;
        else roundSpread += 0.25f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeMoreStrafe()
    {
        if (IsNumericalMode()) roundMoveSpeed += 0.5f; 
        else roundStrafeSpeed += 0.5f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeLessStrafe()
    {
        if (IsNumericalMode()) roundMoveSpeed -= 0.5f;
        else roundStrafeSpeed -= 0.5f;
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    void ApplyLimits()
    {
        // Behavioral Limits
        roundAttackRange = Mathf.Clamp(roundAttackRange, 3f, 15f);
        roundRetreatRange = Mathf.Clamp(roundRetreatRange, 1f, 10f);
        roundAwareRadius = Mathf.Clamp(roundAwareRadius, 1f, 8f);
        roundStrafeSpeed = Mathf.Clamp(roundStrafeSpeed, 0.5f, 5f);
        roundSpread = Mathf.Clamp(roundSpread, 0f, 30f);
        roundDodgeChance = Mathf.Clamp(roundDodgeChance, 0f, 0.9f);
        roundDashSpeed = Mathf.Clamp(roundDashSpeed, 5f, 20f);

        // Numerical Limits
        roundMoveSpeed = Mathf.Clamp(roundMoveSpeed, 1f, 10f);
        roundFireRate = Mathf.Clamp(roundFireRate, 0.2f, 5f);
        roundDashCooldown = Mathf.Clamp(roundDashCooldown, 0.5f, 5f);
        roundBulletForce = Mathf.Clamp(roundBulletForce, 5f, 25f);
    }

    private void UpdateActiveEnemies()
    {
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        foreach (Enemy enemy in activeEnemies)
        {
            ApplyDifficultyToEnemy(enemy);
        }
    }

    private void NotifyUI(string message, Color alertColor)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDDAAlert(message, alertColor);
        }
    }
}