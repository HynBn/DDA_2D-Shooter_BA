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
    public float roundBulletForce = 15f;
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
        DataEvaluator.OnSlightMoreStrafe += MakeSlightlyMoreStrafe;
        DataEvaluator.OnLessStrafe += MakeLessStrafe;
        DataEvaluator.OnSlightLessStrafe += MakeSlightlyLessStrafe;

        RoundEvaluator.OnRoundSignificantHarder += MakeRoundSignificantHarder;
        RoundEvaluator.OnRoundModerateHarder += MakeRoundModerateHarder;
        RoundEvaluator.OnRoundSignificantEasier += MakeRoundSignificantEasier;
        RoundEvaluator.OnRoundModerateEasier += MakeRoundModerateEasier;
        RoundEvaluator.OnRoundSlightlyEasier += MakeModerateEasier;
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
        DataEvaluator.OnSlightMoreStrafe += MakeSlightlyMoreStrafe;
        DataEvaluator.OnLessStrafe -= MakeLessStrafe;
        DataEvaluator.OnSlightLessStrafe -= MakeSlightlyLessStrafe;
 
        RoundEvaluator.OnRoundSignificantHarder -= MakeRoundSignificantHarder;
        RoundEvaluator.OnRoundModerateHarder -= MakeRoundModerateHarder;
        RoundEvaluator.OnRoundSignificantEasier -= MakeRoundSignificantEasier;
        RoundEvaluator.OnRoundModerateEasier -= MakeRoundModerateEasier;
        RoundEvaluator.OnRoundSlightlyEasier -= MakeModerateEasier;
        RoundEvaluator.OnRoundLessStrafe -= MakeLessStrafe;
        RoundEvaluator.OnRoundMoreAccurate -= MakeMoreAccurate;
    }

    public void ApplyDifficultyToEnemy(Enemy myEnemyScript)
    {
        if (myEnemyScript == null) return;

        // Fair Parameters
        myEnemyScript.awareRadius = roundAwareRadius;
        myEnemyScript.attackRange = roundAttackRange;
        myEnemyScript.retreatRange = roundRetreatRange;
        myEnemyScript.spread = roundSpread;
        myEnemyScript.dashSpeed = roundDashSpeed;
        myEnemyScript.retreatDashChance = roundRetreatDashChance;
        myEnemyScript.strafeSpeed = roundStrafeSpeed;
        myEnemyScript.strafeChangeInterval = roundStrafeChangeInterval;
        myEnemyScript.dodgeChance = roundDodgeChance;

        // Unfair Parameters
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
            roundAttackRange += 1f;
            roundRetreatRange += 1f;
            roundSpread -= 1.2f;           
            roundDodgeChance += 0.1f;
            roundDashSpeed += 1f;
            roundAwareRadius -= 0.5f;
            roundStrafeSpeed += 0.5f;
            roundStrafeChangeInterval += 0.1f;

            roundMoveSpeed += 0.6f;
            roundFireRate -= 0.3f;       
            roundDashCooldown -= 0.5f;   
            roundBulletForce += 1.5f;
        }
        else
        {
            roundAttackRange += 1f;
            roundRetreatRange += 1f;
            roundSpread -= 1.2f;           
            roundDodgeChance += 0.1f;
            roundDashSpeed += 1f;
            roundAwareRadius -= 0.5f;
            roundStrafeSpeed += 0.5f;
            roundStrafeChangeInterval += 0.1f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeModerateHarder()
    {
        if (IsNumericalMode())
        {
            roundAttackRange += 0.6f;
            roundRetreatRange += 0.6f;
            roundSpread -= 0.7f;
            roundDodgeChance += 0.05f;
            roundDashSpeed += 0.6f;
            roundAwareRadius -= 0.3f;
            roundStrafeSpeed += 0.3f;
            roundStrafeChangeInterval += 0.05f;

            roundMoveSpeed += 0.4f;
            roundFireRate -= 0.2f;
            roundDashCooldown -= 0.3f;
            roundBulletForce += 1f;
        }
        else
        {
            roundAttackRange += 0.6f;
            roundRetreatRange += 0.6f;
            roundSpread -= 0.7f;
            roundDodgeChance += 0.05f;
            roundDashSpeed += 0.6f;
            roundAwareRadius -= 0.3f;
            roundStrafeSpeed += 0.3f;
            roundStrafeChangeInterval += 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeSlightlyHarder()
    {
        if (IsNumericalMode())
        {
            roundAttackRange += 0.25f;
            roundRetreatRange += 0.25f;
            roundSpread -= 0.4f;
            roundDodgeChance += 0.02f;
            roundDashSpeed += 0.25f;
            roundAwareRadius -= 0.1f;
            roundStrafeSpeed += 0.1f;
            roundStrafeChangeInterval += 0.025f;

            roundMoveSpeed += 0.2f;
            roundFireRate -= 0.1f;
            roundDashCooldown -= 0.1f;
            roundBulletForce += 0.5f;
        }
        else
        {
            roundAttackRange += 0.25f;
            roundRetreatRange += 0.25f;
            roundSpread -= 0.4f;
            roundDodgeChance += 0.02f;
            roundDashSpeed += 0.25f;
            roundAwareRadius -= 0.1f;
            roundStrafeSpeed += 0.1f;
            roundStrafeChangeInterval += 0.025f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSignificantEasier()
    {
        if (IsNumericalMode())
        {
            roundAttackRange -= 1f;
            roundRetreatRange -= 1f;
            roundSpread += 1.2f;
            roundDodgeChance -= 0.1f;
            roundDashSpeed -= 1f;
            roundAwareRadius += 0.5f;
            roundStrafeSpeed -= 0.5f;
            roundStrafeChangeInterval -= 0.1f;

            roundMoveSpeed -= 0.6f;
            roundFireRate += 0.3f;       
            roundDashCooldown += 0.5f;
            roundBulletForce -= 1.5f;
        }
        else
        {
            roundAttackRange -= 1f;
            roundRetreatRange -= 1f;
            roundSpread += 1.2f;
            roundDodgeChance -= 0.1f;
            roundDashSpeed -= 1f;
            roundAwareRadius += 0.5f;
            roundStrafeSpeed -= 0.5f;
            roundStrafeChangeInterval -= 0.1f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeModerateEasier()
    {
        if (IsNumericalMode())
        {
            roundAttackRange -= 0.6f;
            roundRetreatRange -= 0.6f;
            roundSpread += 0.7f;
            roundDodgeChance -= 0.05f;
            roundDashSpeed -= 0.6f;
            roundAwareRadius += 0.3f;
            roundStrafeSpeed -= 0.3f;
            roundStrafeChangeInterval -= 0.05f;

            roundMoveSpeed -= 0.4f;
            roundFireRate += 0.3f;
            roundDashCooldown += 0.3f;
            roundBulletForce -= 1f;
        }
        else
        {
            roundAttackRange -= 0.6f;
            roundRetreatRange -= 0.6f;
            roundSpread += 0.7f;
            roundDodgeChance -= 0.05f;
            roundDashSpeed -= 0.6f;
            roundAwareRadius += 0.3f;
            roundStrafeSpeed -= 0.3f;
            roundStrafeChangeInterval -= 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSlightlyEasier()
    {
        if (IsNumericalMode())
        {
            roundAttackRange -= 0.25f;
            roundRetreatRange -= 0.25f;
            roundSpread += 0.4f;
            roundDodgeChance -= 0.02f;
            roundDashSpeed -= 0.25f;
            roundAwareRadius += 0.1f;
            roundStrafeSpeed -= 0.1f;
            roundStrafeChangeInterval -= 0.025f;

            roundMoveSpeed -= 0.2f;
            roundFireRate += 0.1f;
            roundDashCooldown += 0.1f;
            roundBulletForce -= 0.5f;
        }
        else
        {
            roundAttackRange -= 0.25f;
            roundRetreatRange -= 0.25f;
            roundSpread += 0.4f;
            roundDodgeChance -= 0.02f;
            roundDashSpeed -= 0.25f;
            roundAwareRadius += 0.1f;
            roundStrafeSpeed -= 0.1f;
            roundStrafeChangeInterval -= 0.025f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeMoreAccurate()
    {
        if (IsNumericalMode()) 
        {
            roundFireRate -= 0.05f;
            roundSpread -= 0.75f;
        }
        else 
        {
            roundSpread -= 0.75f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeSlightlyMoreAccurate()
    {
        if (IsNumericalMode())
        {
            roundFireRate -= 0.025f;
            roundSpread -= 0.25f;
        }
        else
        {
            roundSpread -= 0.25f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeLessAccurate()
    {
        if (IsNumericalMode())
        {
            roundFireRate += 0.05f; 
            roundSpread += 0.75f;
        }
        else
        {
            roundSpread += 0.75f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSlightlyLessAccurate()
    {
        if (IsNumericalMode())
        {
            roundFireRate += 0.025f;
            roundSpread += 0.25f;
        }
        else
        {
            roundSpread += 0.25f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeMoreStrafe()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed += 0.25f; 
            roundStrafeSpeed += 0.25f;
            roundStrafeChangeInterval += 0.05f;
        }
        else
        {
            roundStrafeSpeed += 0.25f;
            roundStrafeChangeInterval += 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeSlightlyMoreStrafe()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed += 0.15f; 
            roundStrafeSpeed += 0.15f;
            roundStrafeChangeInterval += 0.025f;
        }
        else
        {
            roundStrafeSpeed += 0.15f;
            roundStrafeChangeInterval += 0.025f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeLessStrafe()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed -= 0.25f;
            roundStrafeSpeed -= 0.25f;
            roundStrafeChangeInterval -= 0.05f;
        }
        else
        {
            roundStrafeSpeed -= 0.25f;
            roundStrafeChangeInterval -= 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeSlightlyLessStrafe()
    {
        if (IsNumericalMode())
        {
            roundMoveSpeed -= 0.15f;
            roundStrafeSpeed -= 0.15f;
            roundStrafeChangeInterval -= 0.025f;
        }
        else
        {
            roundStrafeSpeed -= 0.15f;
            roundStrafeChangeInterval -= 0.025f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeRoundSignificantHarder()
    {
        if (IsNumericalMode())
        {
            roundAttackRange += 1.5f;
            roundRetreatRange += 1.5f;
            roundSpread -= 1.7f;           
            roundDodgeChance += 0.15f;
            roundDashSpeed += 1.5f;
            roundAwareRadius -= 1f;
            roundStrafeSpeed += 1f;
            roundStrafeChangeInterval += 0.15f;

            roundMoveSpeed += 1f;
            roundFireRate -= 0.5f;       
            roundDashCooldown -= 0.6f;   
            roundBulletForce += 2f;
        }
        else
        {
            roundAttackRange += 1.5f;
            roundRetreatRange += 1.5f;
            roundSpread -= 1.7f;           
            roundDodgeChance += 0.15f;
            roundDashSpeed += 1.5f;
            roundAwareRadius -= 1f;
            roundStrafeSpeed += 1f;
            roundStrafeChangeInterval += 0.15f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeRoundModerateHarder()
    {
        if (IsNumericalMode())
        {
            roundAttackRange += 1.2f;
            roundRetreatRange += 1.2f;
            roundSpread -= 1.2f;
            roundDodgeChance += 0.12f;
            roundDashSpeed += 1.2f;
            roundAwareRadius -= 0.6f;
            roundStrafeSpeed += 0.6f;
            roundStrafeChangeInterval += 0.05f;

            roundMoveSpeed += 0.8f;
            roundFireRate -= 0.4f;
            roundDashCooldown -= 0.55f;
            roundBulletForce += 1.6f;
        }
        else
        {
            roundAttackRange += 1.2f;
            roundRetreatRange += 1.2f;
            roundSpread -= 1.2f;
            roundDodgeChance += 0.12f;
            roundDashSpeed += 1.2f;
            roundAwareRadius -= 0.6f;
            roundStrafeSpeed += 0.6f;
            roundStrafeChangeInterval += 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Increased", Color.red);
    }

    private void MakeRoundSignificantEasier()
    {
        if (IsNumericalMode())
        {
            roundAttackRange -= 1.5f;
            roundRetreatRange -= 1.5f;
            roundSpread += 1.7f;
            roundDodgeChance -= 0.15f;
            roundDashSpeed -= 1.5f;
            roundAwareRadius += 1f;
            roundStrafeSpeed -= 1f;
            roundStrafeChangeInterval -= 0.15f;

            roundMoveSpeed -= 0.6f;
            roundFireRate += 0.3f;       
            roundDashCooldown += 0.5f;
            roundBulletForce -= 1.5f;
        }
        else
        {
            roundAttackRange -= 1.5f;
            roundRetreatRange -= 1.5f;
            roundSpread += 1.7f;
            roundDodgeChance -= 0.15f;
            roundDashSpeed -= 1.5f;
            roundAwareRadius += 1f;
            roundStrafeSpeed -= 1f;
            roundStrafeChangeInterval -= 0.15f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI("Difficulty Decreased", Color.darkGreen);
    }

    private void MakeRoundModerateEasier()
    {
        if (IsNumericalMode())
        {
            roundAttackRange -= 1.2f;
            roundRetreatRange -= 1.2f;
            roundSpread += 1.2f;
            roundDodgeChance -= 0.12f;
            roundDashSpeed -= 1.2f;
            roundAwareRadius += 0.6f;
            roundStrafeSpeed -= 0.6f;
            roundStrafeChangeInterval -= 0.05f;

            roundMoveSpeed -= 0.8f;
            roundFireRate += 0.4f;       
            roundDashCooldown += 0.55f;
            roundBulletForce -= 1.6f;
        }
        else
        {
            roundAttackRange -= 1.2f;
            roundRetreatRange -= 1.2f;
            roundSpread += 1.2f;
            roundDodgeChance -= 0.12f;
            roundDashSpeed -= 1.2f;
            roundAwareRadius += 0.6f;
            roundStrafeSpeed -= 0.6f;
            roundStrafeChangeInterval -= 0.05f;
        }
        ApplyLimits();
        UpdateActiveEnemies();
    }

    void ApplyLimits()
    {
        //Fair Limits
        roundAttackRange = Mathf.Clamp(roundAttackRange, 3f, 12f);
        roundRetreatRange = Mathf.Clamp(roundRetreatRange, 1f, 10f);
        roundAwareRadius = Mathf.Clamp(roundAwareRadius, 1f, 8f);
        roundStrafeSpeed = Mathf.Clamp(roundStrafeSpeed, 0.1f, 6f);
        roundStrafeChangeInterval = Mathf.Clamp(roundStrafeChangeInterval, 0f, 2f);
        roundSpread = Mathf.Clamp(roundSpread, 0.1f, 30f);
        roundDodgeChance = Mathf.Clamp(roundDodgeChance, 0f, 0.9f);
        roundDashSpeed = Mathf.Clamp(roundDashSpeed, 5f, 20f);

        //Unfair Limits
        roundMoveSpeed = Mathf.Clamp(roundMoveSpeed, 1f, 6f);
        roundFireRate = Mathf.Clamp(roundFireRate, 0.2f, 4f);
        roundDashCooldown = Mathf.Clamp(roundDashCooldown, 1f, 4f);
        roundBulletForce = Mathf.Clamp(roundBulletForce, 5f, 20f);
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