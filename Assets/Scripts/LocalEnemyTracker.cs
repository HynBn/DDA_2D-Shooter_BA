using UnityEngine;

public class LocalEnemyTracker : MonoBehaviour
{

    public float shotsFired;
    public float shotsHit;
    public float damageTaken;


    public void RegisterEnemyShot()
    {
        shotsFired++;
    }

    public void RegisterEnemyHit()
    {
        shotsHit++;
    }

    public void RegisterEnemyDamageTaken(float damage)
    {
        damageTaken += damage;
    }

    public float GetHitRate()
    {
        if (shotsFired == 0) return 0f;
        return shotsHit / shotsFired;
    } 

}
