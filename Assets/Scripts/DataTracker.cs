using UnityEngine;

public class DataTracker : MonoBehaviour
{
    public static DataTracker Instance { get; private set; }

    public float playerShotsFired;
    public float playerShotsHit;
    public float playerDamageTaken;
    public int nearHits;
    public int perfectDodges;
    public int totalDashes;

    public float roundShotsFired;
    public float roundShotsHit;
    public float roundDamageTaken;
    public int roundNearHits;
    public int roundPerfectDodges;
    public int roundTotalDashes;

    public float startTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startTime = Time.time;
    } 

    public void ResetInterval()
    {
        playerShotsFired = 0;
        playerShotsHit = 0;
        playerDamageTaken = 0;
        nearHits = 0;
        perfectDodges = 0;
        totalDashes = 0;
    }

    public void ResetRoundData()
    {
        roundShotsFired = 0;
        roundShotsHit = 0;
        roundDamageTaken = 0;
        roundNearHits = 0;
        roundPerfectDodges = 0;
        roundTotalDashes = 0;
    }

    public void RegisterPlayerShot()
    {
        playerShotsFired++;
        roundShotsFired++;
    }

    public void RegisterPlayerHit()
    {
        playerShotsHit++;
        roundShotsHit++;
    }

    public void RegisterPlayerDamageTaken(float damage)
    {
        playerDamageTaken += damage;
        roundDamageTaken += damage;
    }

    public float GetPlayerHitRate()
    {
        if (playerShotsFired == 0) return 0f;

        return playerShotsHit / playerShotsFired;
    }

    public float GetRoundPlayerHitRate()
    {
        if (roundShotsFired == 0) return 0f;

        return roundShotsHit / roundShotsFired;
    }

    public float GetRoundTime()
    {
        return Time.time - startTime;
    }
}
