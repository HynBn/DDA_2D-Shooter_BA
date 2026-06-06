using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public Vector2 movement;
    public Vector2 mousePos;

    [Header("Parameters")]
    public float moveSpeed = 3f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    public float bulletForce = 10f;
    public float fireRate = 2f;    
    public float baseSpread = 2f;
    public float maxSpread = 20f;

    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction mouseAction;
    public InputAction dashAction;
    public InputAction fireAction;

    public bool isDashing;
    private float dashTimeCounter;
    private float dashCooldownCounter;
    private float fireCooldownTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        else if (cam == null)
        {
            Debug.LogError("Player script requires a reference to a Camera.");
        }
    }

    private void OnEnable()
    {
        moveAction.Enable();
        mouseAction.Enable();
        dashAction.Enable();
        fireAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        mouseAction.Disable();  
        dashAction.Disable();
        fireAction.Disable();    
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            return;
        }
        movement = moveAction.ReadValue<Vector2>();

        Vector2 screenPos = mouseAction.ReadValue<Vector2>();
        mousePos = cam.ScreenToWorldPoint(screenPos);

        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.deltaTime;
        }
        if (isDashing)
        {
            dashTimeCounter -= Time.deltaTime;
            if (dashTimeCounter <= 0)
            {
                isDashing = false;
            }

            Vector2 dashScreenPos = mouseAction.ReadValue<Vector2>();
            mousePos = cam.ScreenToWorldPoint(dashScreenPos);
            
            return;
        }

        if (dashAction.WasPressedThisFrame() && dashCooldownCounter <= 0 && movement != Vector2.zero)
        {
            isDashing = true;
            dashTimeCounter = dashDuration;
            dashCooldownCounter = dashCooldown;

            if(DataTracker.Instance != null)
            {
                DataTracker.Instance.totalDashes++;
            }
        }

        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        if (fireAction.WasPressedThisFrame() && fireCooldownTimer <= 0)
        {
            Shoot();

            fireCooldownTimer = fireRate;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            return;
        }
        
        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Shoot()
    {
        float currentSpeed = movement.magnitude;
        float speedFactor = Mathf.Clamp01(currentSpeed);

        float currentSpread = Mathf.Lerp(baseSpread, maxSpread, speedFactor);
        float randomAngle = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
        Quaternion finalRotation = firepoint.rotation * spreadRotation;

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, finalRotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.AddForce(bullet.transform.up * bulletForce, ForceMode2D.Impulse);
        
        if (DataTracker.Instance != null)
        {
            DataTracker.Instance.RegisterPlayerShot();           
        }
    }
}
