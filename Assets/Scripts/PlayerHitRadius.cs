using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class PlayerHitRadius : MonoBehaviour
{
    private Player playerScript;
    private HashSet<Collider2D> dodgedBullets = new HashSet<Collider2D>();

    void Awake()
    {
        playerScript = GetComponentInParent<Player>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            if (playerScript != null && playerScript.isDashing && !dodgedBullets.Contains(collision))
            {
                dodgedBullets.Add(collision); 
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            bool wasDodged = dodgedBullets.Contains(collision);

            if (wasDodged)
            {
                dodgedBullets.Remove(collision);
            }

            StartCoroutine(CheckOutcomeRoutine(collision.gameObject, wasDodged));
        }
    }

    private IEnumerator CheckOutcomeRoutine(GameObject bulletObj, bool wasDodged)
    {
        yield return new WaitForEndOfFrame();

        if (bulletObj != null)
        {
            if (DataTracker.Instance != null)
            {
                if (wasDodged)
                {
                    DataTracker.Instance.perfectDodges++;
                    Debug.Log("Perfect dodge");
                }
                else
                {
                    DataTracker.Instance.nearHits++;
                    Debug.Log("Near hit");
                }
            }
        }
        else
        {
            //took damage
        }
    }
}