using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 5f;
    public string targetTag;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Health targetHealth = collision.gameObject.GetComponent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
