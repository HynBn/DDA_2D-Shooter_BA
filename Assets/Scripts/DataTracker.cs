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

    public void RegisterPlayerShot()
    {
        playerShotsFired++;
    }

    public void RegisterPlayerHit()
    {
        playerShotsHit++;
    }

    public void RegisterPlayerDamageTaken(float damage)
    {
        playerDamageTaken += damage;
    }

    public float GetPlayerHitRate()
    {
        if (playerShotsFired == 0) return 0f;

        return playerShotsHit / playerShotsFired;
    }

    public float GetRoundTime()
    {
        return Time.time - startTime;
    }
}
