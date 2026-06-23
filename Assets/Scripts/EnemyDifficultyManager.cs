using JetBrains.Annotations;
using Unity.VisualScripting;
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

    [Header("Modifiers")]
    [SerializeField] private float modAttackRange = 1.0f;
    [SerializeField] private float modRetreatRange = 1.0f;
    [SerializeField] private float modSpread = 1.2f;       
    [SerializeField] private float modDodgeChance = 0.1f;
    [SerializeField] private float modDashSpeed = 1.0f;
    [SerializeField] private float modAwareRadius = 0.5f;  
    [SerializeField] private float modStrafeSpeed = 0.5f;
    [SerializeField] private float modStrafeInterval = 0.1f;
    
    [Space(5)]
    [SerializeField] private float modMoveSpeed = 0.6f;
    [SerializeField] private float modFireRate = 0.3f;     
    [SerializeField] private float modDashCooldown = 0.5f; 
    [SerializeField] private float modBulletForce = 1.5f;

    [Header("Specific Modifiers (Accuracy & Strafe)")]
    [SerializeField] private float modAccSpread = 0.75f;
    [SerializeField] private float modAccFireRate = 0.05f;
    [SerializeField] private float modSpecStrafeSpeed = 0.25f;
    [SerializeField] private float modSpecStrafeInterval = 0.05f;
    [SerializeField] private float modSpecMoveSpeed = 0.25f;

    [Header("DDA Telemetry")]
    public int totalHarderInterventions = 0;
    public int totalEasierInterventions = 0;
    public int totalAccuracyInterventions = 0;
    public int totalStrafeInterventions = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private bool IsDDAEnabled()
    {
        return GameSettings.Instance != null && GameSettings.Instance.currentDifficultySystem == GameSettings.DifficultySystem.DDA;
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
        RoundEvaluator.OnRoundSlightlyEasier += MakeRoundSlightlyEasier;
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
        DataEvaluator.OnSlightMoreStrafe -= MakeSlightlyMoreStrafe;
        DataEvaluator.OnLessStrafe -= MakeLessStrafe;
        DataEvaluator.OnSlightLessStrafe -= MakeSlightlyLessStrafe;
 
        RoundEvaluator.OnRoundSignificantHarder -= MakeRoundSignificantHarder;
        RoundEvaluator.OnRoundModerateHarder -= MakeRoundModerateHarder;
        RoundEvaluator.OnRoundSignificantEasier -= MakeRoundSignificantEasier;
        RoundEvaluator.OnRoundModerateEasier -= MakeRoundModerateEasier;
        RoundEvaluator.OnRoundSlightlyEasier -= MakeRoundSlightlyEasier;
        RoundEvaluator.OnRoundLessStrafe -= MakeLessStrafe;
        RoundEvaluator.OnRoundMoreAccurate -= MakeMoreAccurate;
    }

    private bool IsNumericalMode()
    {
        return IsDDAEnabled() && GameSettings.Instance.currentDDAMode == GameSettings.DDAMode.Numerical;
    }

    private void AdjustGeneralDifficulty (float multiplier, string notifyMsg, Color color)
    {
        if (!IsDDAEnabled())
        {
            return;
        }

        if(multiplier > 0) totalHarderInterventions++;
        else totalEasierInterventions++;

        roundAwareRadius -= modAwareRadius * multiplier;
        roundAttackRange += modAttackRange * multiplier;
        roundRetreatRange += modRetreatRange * multiplier;
        roundSpread -= modSpread * multiplier;
        roundDashSpeed += modDashSpeed * multiplier;
        roundStrafeSpeed += modStrafeSpeed * multiplier;
        roundStrafeChangeInterval += modStrafeInterval * multiplier;
        roundDodgeChance += modDodgeChance * multiplier;

        if (IsNumericalMode())
        {
            roundMoveSpeed += modMoveSpeed * multiplier;
            roundDashCooldown -= modDashCooldown * multiplier;
            roundBulletForce += modBulletForce * multiplier;
            roundFireRate -= modFireRate * multiplier;
        }

        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI(notifyMsg, color);
    }

    private void AdjustAccuracy (float multiplier, string notifyMsg, Color color)
    {
        if (!IsDDAEnabled())
        {
            return;
        }

        totalAccuracyInterventions++;
        if (multiplier > 0) totalHarderInterventions++;
        else if (multiplier < 0) totalEasierInterventions++;

        roundSpread -= modAccSpread * multiplier;
        if(IsNumericalMode()) roundFireRate -= modAccFireRate * multiplier;

        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI(notifyMsg, color);
    }

    private void AdjustStrafe (float multiplier, string notifyMsg, Color color)
    {
        if (!IsDDAEnabled())
        {
            return;
        }

        totalStrafeInterventions++;
        if (multiplier > 0) totalHarderInterventions++;
        else if (multiplier < 0) totalEasierInterventions++;

        roundStrafeSpeed += modSpecStrafeSpeed * multiplier;
        roundStrafeChangeInterval += modSpecStrafeInterval * multiplier;
        if(IsNumericalMode()) roundMoveSpeed += modSpecMoveSpeed * multiplier;

        ApplyLimits();
        UpdateActiveEnemies();
        NotifyUI(notifyMsg, color);
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

    //DataEvaluator (Interval)
    private void MakeSignificantHarder() => AdjustGeneralDifficulty(1.0f, "Difficulty Increased", Color.red);
    private void MakeModerateHarder() => AdjustGeneralDifficulty(0.6f, "Difficulty Increased", Color.red);
    private void MakeSlightlyHarder() => AdjustGeneralDifficulty(0.25f, "Difficulty Increased", Color.red);

    private void MakeSignificantEasier() => AdjustGeneralDifficulty(-1.0f, "Difficulty Decreased", Color.darkGreen);
    private void MakeModerateEasier() => AdjustGeneralDifficulty(-0.6f, "Difficulty Decreased", Color.darkGreen);
    private void MakeSlightlyEasier() => AdjustGeneralDifficulty(-0.25f, "Difficulty Decreased", Color.darkGreen);

    //Accuracy
    private void MakeMoreAccurate() => AdjustAccuracy(1.0f, "Difficulty Increased", Color.red);
    private void MakeSlightlyMoreAccurate() => AdjustAccuracy(0.33f, "Difficulty Increased", Color.red);
    private void MakeLessAccurate() => AdjustAccuracy(-1.0f, "Difficulty Decreased", Color.darkGreen);
    private void MakeSlightlyLessAccurate() => AdjustAccuracy(-0.33f, "Difficulty Decreased", Color.darkGreen);

    //Strafe
    private void MakeMoreStrafe() => AdjustStrafe(1.0f, "Difficulty Increased", Color.red);
    private void MakeSlightlyMoreStrafe() => AdjustStrafe(0.6f, "Difficulty Increased", Color.red);
    private void MakeLessStrafe() => AdjustStrafe(-1.0f, "Difficulty Decreased", Color.darkGreen);
    private void MakeSlightlyLessStrafe() => AdjustStrafe(-0.6f, "Difficulty Decreased", Color.darkGreen);

    //RoundEvaluator
    private void MakeRoundSignificantHarder() => AdjustGeneralDifficulty(1.5f, "Difficulty Increased", Color.red);
    private void MakeRoundModerateHarder() => AdjustGeneralDifficulty(1.2f, "Difficulty Increased", Color.red);
    private void MakeRoundSignificantEasier() => AdjustGeneralDifficulty(-1.5f, "Difficulty Decreased", Color.darkGreen);
    private void MakeRoundModerateEasier() => AdjustGeneralDifficulty(-1.2f, "Difficulty Decreased", Color.darkGreen);
    private void MakeRoundSlightlyEasier() => AdjustGeneralDifficulty(-0.5f, "Difficulty Decreased", Color.darkGreen);

    // private void MakeSignificantHarder()
    // {
    //         roundAttackRange += 1f;
    //         roundRetreatRange += 1f;
    //         roundSpread -= 1.2f;           
    //         roundDodgeChance += 0.1f;
    //         roundDashSpeed += 1f;
    //         roundAwareRadius -= 0.5f;
    //         roundStrafeSpeed += 0.5f;
    //         roundStrafeChangeInterval += 0.1f;
            
    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.6f;
    //         roundFireRate -= 0.3f;       
    //         roundDashCooldown -= 0.5f;   
    //         roundBulletForce += 1.5f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeModerateHarder()
    // {
    //         roundAttackRange += 0.6f;
    //         roundRetreatRange += 0.6f;
    //         roundSpread -= 0.7f;
    //         roundDodgeChance += 0.05f;
    //         roundDashSpeed += 0.6f;
    //         roundAwareRadius -= 0.3f;
    //         roundStrafeSpeed += 0.3f;
    //         roundStrafeChangeInterval += 0.05f;
        
    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.4f;
    //         roundFireRate -= 0.2f;
    //         roundDashCooldown -= 0.3f;
    //         roundBulletForce += 1f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeSlightlyHarder()
    // {
    //         roundAttackRange += 0.25f;
    //         roundRetreatRange += 0.25f;
    //         roundSpread -= 0.4f;
    //         roundDodgeChance += 0.02f;
    //         roundDashSpeed += 0.25f;
    //         roundAwareRadius -= 0.1f;
    //         roundStrafeSpeed += 0.1f;
    //         roundStrafeChangeInterval += 0.025f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.2f;
    //         roundFireRate -= 0.1f;
    //         roundDashCooldown -= 0.1f;
    //         roundBulletForce += 0.5f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeSignificantEasier()
    // {
    //     if (IsNumericalMode())
    //     {
    //         roundAttackRange -= 1f;
    //         roundRetreatRange -= 1f;
    //         roundSpread += 1.2f;
    //         roundDodgeChance -= 0.1f;
    //         roundDashSpeed -= 1f;
    //         roundAwareRadius += 0.5f;
    //         roundStrafeSpeed -= 0.5f;
    //         roundStrafeChangeInterval -= 0.1f;

    //         roundMoveSpeed -= 0.6f;
    //         roundFireRate += 0.3f;       
    //         roundDashCooldown += 0.5f;
    //         roundBulletForce -= 1.5f;
    //     }
    //     else
    //     {
    //         roundAttackRange -= 1f;
    //         roundRetreatRange -= 1f;
    //         roundSpread += 1.2f;
    //         roundDodgeChance -= 0.1f;
    //         roundDashSpeed -= 1f;
    //         roundAwareRadius += 0.5f;
    //         roundStrafeSpeed -= 0.5f;
    //         roundStrafeChangeInterval -= 0.1f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeModerateEasier()
    // {
    //         roundAttackRange -= 0.6f;
    //         roundRetreatRange -= 0.6f;
    //         roundSpread += 0.7f;
    //         roundDodgeChance -= 0.05f;
    //         roundDashSpeed -= 0.6f;
    //         roundAwareRadius += 0.3f;
    //         roundStrafeSpeed -= 0.3f;
    //         roundStrafeChangeInterval -= 0.05f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.4f;
    //         roundFireRate += 0.3f;
    //         roundDashCooldown += 0.3f;
    //         roundBulletForce -= 1f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeSlightlyEasier()
    // {
    //         roundAttackRange -= 0.25f;
    //         roundRetreatRange -= 0.25f;
    //         roundSpread += 0.4f;
    //         roundDodgeChance -= 0.02f;
    //         roundDashSpeed -= 0.25f;
    //         roundAwareRadius += 0.1f;
    //         roundStrafeSpeed -= 0.1f;
    //         roundStrafeChangeInterval -= 0.025f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.2f;
    //         roundFireRate += 0.1f;
    //         roundDashCooldown += 0.1f;
    //         roundBulletForce -= 0.5f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeMoreAccurate()
    // {
    //         roundSpread -= 0.75f;

    //     if (IsNumericalMode()) 
    //     {
    //         roundFireRate -= 0.05f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeSlightlyMoreAccurate()
    // {
    //         roundSpread -= 0.25f;

    //     if (IsNumericalMode())
    //     {
    //         roundFireRate -= 0.025f;
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeLessAccurate()
    // {
    //         roundSpread += 0.75f;

    //     if (IsNumericalMode())
    //     {
    //         roundFireRate += 0.05f; 
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeSlightlyLessAccurate()
    // {
    //         roundSpread += 0.25f;

    //     if (IsNumericalMode())
    //     {
    //         roundFireRate += 0.025f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeMoreStrafe()
    // {
    //         roundStrafeSpeed += 0.25f;
    //         roundStrafeChangeInterval += 0.05f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.25f; 
    //     }
    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeSlightlyMoreStrafe()
    // {
    //         roundStrafeSpeed += 0.15f;
    //         roundStrafeChangeInterval += 0.025f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.15f; 
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeLessStrafe()
    // {
    //         roundStrafeSpeed -= 0.25f;
    //         roundStrafeChangeInterval -= 0.05f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.25f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeSlightlyLessStrafe()
    // {
    //         roundStrafeSpeed -= 0.15f;
    //         roundStrafeChangeInterval -= 0.025f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.15f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeRoundSignificantHarder()
    // {
    //         roundAttackRange += 1.5f;
    //         roundRetreatRange += 1.5f;
    //         roundSpread -= 1.7f;           
    //         roundDodgeChance += 0.15f;
    //         roundDashSpeed += 1.5f;
    //         roundAwareRadius -= 1f;
    //         roundStrafeSpeed += 1f;
    //         roundStrafeChangeInterval += 0.15f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 1f;
    //         roundFireRate -= 0.5f;       
    //         roundDashCooldown -= 0.6f;   
    //         roundBulletForce += 2f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeRoundModerateHarder()
    // {
    //         roundAttackRange += 1.2f;
    //         roundRetreatRange += 1.2f;
    //         roundSpread -= 1.2f;
    //         roundDodgeChance += 0.12f;
    //         roundDashSpeed += 1.2f;
    //         roundAwareRadius -= 0.6f;
    //         roundStrafeSpeed += 0.6f;
    //         roundStrafeChangeInterval += 0.05f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed += 0.8f;
    //         roundFireRate -= 0.4f;
    //         roundDashCooldown -= 0.55f;
    //         roundBulletForce += 1.6f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Increased", Color.red);
    // }

    // private void MakeRoundSignificantEasier()
    // {
    //         roundAttackRange -= 1.5f;
    //         roundRetreatRange -= 1.5f;
    //         roundSpread += 1.7f;
    //         roundDodgeChance -= 0.15f;
    //         roundDashSpeed -= 1.5f;
    //         roundAwareRadius += 1f;
    //         roundStrafeSpeed -= 1f;
    //         roundStrafeChangeInterval -= 0.15f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.6f;
    //         roundFireRate += 0.3f;       
    //         roundDashCooldown += 0.5f;
    //         roundBulletForce -= 1.5f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    //     NotifyUI("Difficulty Decreased", Color.darkGreen);
    // }

    // private void MakeRoundModerateEasier()
    // {
    //         roundAttackRange -= 1.2f;
    //         roundRetreatRange -= 1.2f;
    //         roundSpread += 1.2f;
    //         roundDodgeChance -= 0.12f;
    //         roundDashSpeed -= 1.2f;
    //         roundAwareRadius += 0.6f;
    //         roundStrafeSpeed -= 0.6f;
    //         roundStrafeChangeInterval -= 0.05f;

    //     if (IsNumericalMode())
    //     {
    //         roundMoveSpeed -= 0.8f;
    //         roundFireRate += 0.4f;       
    //         roundDashCooldown += 0.55f;
    //         roundBulletForce -= 1.6f;
    //     }

    //     ApplyLimits();
    //     UpdateActiveEnemies();
    // }

    void ApplyLimits()
    {
        //Fair/behavioral Limits
        roundAttackRange = Mathf.Clamp(roundAttackRange, 3f, 12f);
        roundRetreatRange = Mathf.Clamp(roundRetreatRange, 1f, 10f);
        roundAwareRadius = Mathf.Clamp(roundAwareRadius, 1f, 8f);
        roundStrafeSpeed = Mathf.Clamp(roundStrafeSpeed, 0.1f, 6f);
        roundStrafeChangeInterval = Mathf.Clamp(roundStrafeChangeInterval, 0f, 2f);
        roundSpread = Mathf.Clamp(roundSpread, 0.1f, 30f);
        roundDodgeChance = Mathf.Clamp(roundDodgeChance, 0f, 0.9f);
        roundDashSpeed = Mathf.Clamp(roundDashSpeed, 5f, 20f);

        //Unfair/numerical Limits
        roundMoveSpeed = Mathf.Clamp(roundMoveSpeed, 1f, 6f);
        roundFireRate = Mathf.Clamp(roundFireRate, 0.2f, 3.5f);
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

    public void ResetMatchDDACounters()
    {
        totalHarderInterventions = 0;
        totalEasierInterventions = 0;
        totalAccuracyInterventions = 0;
        totalStrafeInterventions = 0;
    }

    private void ResetDifficultyParameters()
    {
        // Behavioral parameters
        roundAwareRadius = 8f;
        roundAttackRange = 7f;
        roundRetreatRange = 4f;
        roundSpread = 15f;
        roundDashSpeed = 10f;
        roundRetreatDashChance = 0.2f;
        roundStrafeSpeed = 3f;
        roundStrafeChangeInterval = 1.5f;
        roundDodgeChance = 0.8f;

        // Numerical parameters
        roundMoveSpeed = 3f;
        roundDashCooldown = 2f;
        roundBulletForce = 15f;
        roundFireRate = 2f;
    }

    private void ApplyStaticDifficulty(GameSettings.StaticDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameSettings.StaticDifficulty.Easy:
                roundAwareRadius = 2.5f;
                roundAttackRange = 5f;
                roundRetreatRange = 3f;
                roundSpread = 28f;
                roundDashSpeed = 6f;
                roundRetreatDashChance = 0.03f;
                roundStrafeSpeed = 0.8f;
                roundStrafeChangeInterval = 2f;
                roundDodgeChance = 0.1f;
                break;

            case GameSettings.StaticDifficulty.Medium:
                roundAwareRadius = 6f;
                roundAttackRange = 7f;
                roundRetreatRange = 4f;
                roundSpread = 15f;
                roundDashSpeed = 10f;
                roundRetreatDashChance = 0.2f;
                roundStrafeSpeed = 3f;
                roundStrafeChangeInterval = 1.5f;
                roundDodgeChance = 0.6f;
                break;

            case GameSettings.StaticDifficulty.Hard:
                roundAwareRadius = 8f;
                roundAttackRange = 8.5f;
                roundRetreatRange = 5f;
                roundSpread = 7f;
                roundDashSpeed = 13f;
                roundRetreatDashChance = 0.4f;
                roundStrafeSpeed = 4.5f;
                roundStrafeChangeInterval = 0.8f;
                roundDodgeChance = 0.9f;
                break;

            default:
                Debug.LogWarning(
                    $"Unknown static difficulty: {difficulty}");
                break;
        }
    }

    public void ConfigureDifficultyForMatch()
{
    Debug.Log("[DIFF] ConfigureDifficultyForMatch started");

    ResetDifficultyParameters();
    Debug.Log("[DIFF] ResetDifficultyParameters completed");

    ResetMatchDDACounters();
    Debug.Log("[DIFF] ResetMatchDDACounters completed");

    if (GameSettings.Instance == null)
    {
        Debug.LogError("[DIFF] GameSettings.Instance is null");
        return;
    }

    Debug.Log(
        $"[DIFF] System: {GameSettings.Instance.currentDifficultySystem}");

    if (GameSettings.Instance.currentDifficultySystem ==
        GameSettings.DifficultySystem.Static)
    {
        Debug.Log(
            $"[DIFF] Applying static difficulty: " +
            $"{GameSettings.Instance.currentStaticDifficulty}");

        ApplyStaticDifficulty(
            GameSettings.Instance.currentStaticDifficulty);

        Debug.Log("[DIFF] ApplyStaticDifficulty completed");
    }

    ApplyLimits();
    Debug.Log("[DIFF] ApplyLimits completed");

    UpdateActiveEnemies();
    Debug.Log("[DIFF] UpdateActiveEnemies completed");
}
}