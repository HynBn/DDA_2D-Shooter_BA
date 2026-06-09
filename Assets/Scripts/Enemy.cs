using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firepoint;

    public enum State {Chase, Attack, Retreat, Dodge}
    public State currentState;
    public float obstacleAvoidRange;

    [Header("State Radius")]
    public float awareRadius = 8f;
    public float attackRange = 7f;
    public float retreatRange = 4f;

    [Header("Fair Parameters")]
    public float spread = 15f;
    public float dashSpeed = 10f;
    public float retreatDashChance = 0.2f;
    public float strafeSpeed = 3f;
    public float strafeChangeInterval = 1.5f;
    public float dodgeChance = 0.8f;

    [Header("Unfair Parameters")]
    public float moveSpeed = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    public float bulletForce = 15f;
    public float fireRate = 2f;

    private float retreatDashCheckTimer;
    private float fireCooldownTimer;

    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private Vector2 dodgeDir;

    private float strafeTimer;
    private int strafeDirection = 1; 

    void Start()
    {
        currentState = State.Chase;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            return;
        }

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
        RotateTowardsPlayer();
        Shoot();
    }

    void AttackPlayer()
    {
        RotateTowardsPlayer();
        Shoot();

        if (strafeSpeed > 0)
        {
            strafeTimer -= Time.deltaTime;
            if(strafeTimer <= 0)
            {
                strafeDirection *= -1;
                strafeTimer = strafeChangeInterval;
            }
        }

        Vector3 strafeVector = transform.right * strafeDirection * strafeSpeed * Time.deltaTime;
        transform.position += strafeVector;
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
                dodgeTimer = dashDuration;
                dodgeCooldownTimer = dashCooldown; 
                        if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.dashSFX);

                return; 
            }
        }
        Vector2 directionAway = (transform.position - player.position).normalized;
        Vector2 finalDirection = GetAvoidanceDirection(directionAway);
        
        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
        RotateTowardsPlayer();
        Shoot();
    }

    void DodgeAttack()
    {
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dodgeDir, dashSpeed * Time.deltaTime);
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
            if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSFX);

            float angle = Random.Range(-spread, spread);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
            Quaternion finalRotation = firepoint.rotation * spreadRotation;

            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, finalRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bullet.transform.up * bulletForce, ForceMode2D.Impulse);

            fireCooldownTimer = fireRate;
            
            LocalEnemyTracker myLocalTracker = GetComponent<LocalEnemyTracker>();
            if (myLocalTracker != null)
            {
                myLocalTracker.RegisterEnemyShot();
            }
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
                    if(Random.value > dodgeChance) return;                    

                    Vector2 safeDodge = GetSmartDodgeDirection(hit.transform.position, bulletRb.linearVelocity);
                    if (safeDodge != Vector2.zero)
                    {
                        dodgeDir = safeDodge;
                        currentState = State.Dodge;
                        dodgeTimer = dashDuration;
                        dodgeCooldownTimer = dashCooldown;

                                if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.dashSFX);

                    } else
                    {
                        transform.position +=(Vector3)(safeDodge * moveSpeed * Time.deltaTime);
                    }
                    break;
                }
            }
        }
    }

    Vector2 GetSmartDodgeDirection(Vector3 bulletPos, Vector2 bulletVelocity)
    {
        Vector2 leftDodge = new Vector2(-bulletVelocity.y, bulletVelocity.x).normalized;
        Vector2 rightDodge = new Vector2(bulletVelocity.y, -bulletVelocity.x).normalized;
        
        Vector2 dirToBullet = ((Vector2)bulletPos - (Vector2)transform.position).normalized;
        
        Vector2 preferredDodge;
        Vector2 alternativeDodge;

        if (Vector2.Dot(leftDodge, dirToBullet) > 0)
        {
            preferredDodge = rightDodge;
            alternativeDodge = leftDodge;
        } else
        {
            preferredDodge = leftDodge;
            alternativeDodge = rightDodge;
        }

        float expectedDodgeDistance = dashSpeed * dashDuration;

        if (IsDirectionClear(preferredDodge, expectedDodgeDistance)) return preferredDodge;
        if (IsDirectionClear(alternativeDodge, expectedDodgeDistance)) return alternativeDodge;

        Vector2 backDodge = bulletVelocity.normalized;
        if (IsDirectionClear(backDodge, expectedDodgeDistance)) return backDodge;

        return Vector2.zero;
    }

    bool IsDirectionClear(Vector2 dir, float distance)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, dir, distance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }

        return true;
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
