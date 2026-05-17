using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Enemy : MonoBehaviour
{
    public Transform player;

    public float moveSpeed;
    public float attackRange;
    public float retreatRange;

    public float retreatDashChance = 0.3f;
    private float retreatDashCheckTimer;

    public float awareRadius;
    public float dodgeSpeed;
    public float dodgeDuration;
    public float dodgeCooldown;

    public GameObject bulletPrefab;
    public Transform firepoint;
    public float bulletForce;
    public float fireRate;
    public float spread;

    public float obstacleAvoidRange;

    public enum State {Chase, Attack, Retreat, Dodge}
    public State currentState;

    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private Vector2 dodgeDir;

    private float fireCooldownTimer;

    void Start()
    {
        currentState = State.Chase;
    }

    void Update()
    {
        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }

        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case State.Chase:
                ChasePlayer();
                break;
            case State.Attack:
                AttackPlayer();
                break;
            case State.Retreat:
                RetreatFromPlayer();
                break;
            case State.Dodge:
                DodgeAttack();
                break;
        }

        if (currentState != State.Dodge && dodgeCooldownTimer <= 0)
        {
            CheckForBullets();
        }

        if (currentState == State.Dodge)
        {
            dodgeTimer -= Time.deltaTime;
            if(dodgeTimer <= 0)
            {
                currentState = State.Chase;
            }
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < retreatRange) currentState = State.Retreat;
            else if (distanceToPlayer <= attackRange && HasLineOfSight()) currentState = State.Attack;
            else currentState = State.Chase;
        }
    }

    void ChasePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        Vector2 finalDirection = GetAvoidanceDirection(directionToPlayer);

        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
        //transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        RotateTowardsPlayer();
        Shoot();
    }

    void AttackPlayer()
    {
        RotateTowardsPlayer();

        Shoot();
    }

    void RetreatFromPlayer()
    {
        if (retreatDashCheckTimer > 0) retreatDashCheckTimer -= Time.deltaTime;
 
        if (dodgeCooldownTimer <= 0 && retreatDashCheckTimer <= 0)
        {
            retreatDashCheckTimer = 1f;

            if (Random.value <= retreatDashChance)
            {
                Vector2 baseDirectionAway = (transform.position - player.position).normalized;
                float randomAngle = Random.Range(-100f, 100f);
                dodgeDir = Quaternion.Euler(0, 0, randomAngle) * baseDirectionAway;
                
                currentState = State.Dodge;
                dodgeTimer = dodgeDuration;
                dodgeCooldownTimer = dodgeCooldown; 
                return; 
            }
        }
        Vector2 directionAway = (transform.position - player.position).normalized;
        Vector2 finalDirection = GetAvoidanceDirection(directionAway);
        
        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
        //transform.position = Vector2.MoveTowards(transform.position, player.position, -moveSpeed * Time.deltaTime);
        RotateTowardsPlayer();
        Shoot();
    }

    void DodgeAttack()
    {
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dodgeDir, dodgeSpeed * Time.deltaTime);
    }

    void RotateTowardsPlayer()
    {
        Vector2 lookDir = player.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (fireCooldownTimer <= 0)
        {
            float angle = Random.Range(-spread, spread);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
            Quaternion finalRotation = firepoint.rotation * spreadRotation;

            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, finalRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bullet.transform.up * bulletForce, ForceMode2D.Impulse);

            fireCooldownTimer = fireRate;
        }
    }

    void CheckForBullets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, awareRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("PlayerBullet"))
            {
                Rigidbody2D bulletRb = hit.GetComponent<Rigidbody2D>();
                if (bulletRb != null && bulletRb.linearVelocity.magnitude > 0.1f)
                {
                    dodgeDir = Vector2.Perpendicular(bulletRb.linearVelocity).normalized;
                    if (Random.value > 0.5f)
                    {
                        dodgeDir *= -1;
                    }
                    
                    currentState = State.Dodge;
                    dodgeTimer = dodgeDuration;
                    dodgeCooldownTimer = dodgeCooldown;
                    break;
                }
            }
        }
    }

    bool HasLineOfSight()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer.normalized, distance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }
        return true;
    }

    Vector2 GetAvoidanceDirection(Vector2 desiredDirection)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, desiredDirection, obstacleAvoidRange);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && (hit.collider.CompareTag("Wall")) && hit.normal != Vector2.zero)
            {
                Vector2 slideDirection = new Vector2(-hit.normal.y, hit.normal.x);
                
                if (Vector2.Dot(slideDirection, desiredDirection) < 0)
                {
                    slideDirection = -slideDirection;
                }

                return (slideDirection + hit.normal * 0.8f).normalized;
            }
        }
        return desiredDirection;
    }
}
