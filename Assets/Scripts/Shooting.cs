using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform firepoint;
    public float bulletForce;

    public InputAction fireAction;

    public float fireRate;
    private float fireCooldownTimer;
    
    public float baseSpread;
    public float maxSpread;
    public Player playerScript;

    private void OnEnable()
    {
        fireAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

    void Shoot()
    {
        float currentSpeed = playerScript.movement.magnitude;
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
