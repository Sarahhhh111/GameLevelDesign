using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip walkingClip;
    public AudioClip enemyHitClip;

    private float walkSoundCooldown = 0.5f;
    private float walkSoundTimer = 0f;

    [Header("Movement")]
    public Camera playerCamera;
    public float walkSpeed = 12f;
    public float runSpeed = 24f;
    public float jumpPower = 7f;
    public float gravity = 9.5f;

    [Header("Look")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    public bool canMove = true;
    private CharacterController characterController;

    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthText;
    public float damageCooldown = 1.5f;
    private float lastHitTime = -999f;

    [Header("Gold System")]
    public int gold = 0;
    public TMP_Text goldText;

    [Header("Built-in Shooting (optional)")]
    public bool useBuiltInShooting = false;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateGoldUI();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();

        if (useBuiltInShooting)
            HandleShooting();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = (right * moveHorizontal + forward * moveVertical) * speed;

        moveDirection.x = movement.x;
        moveDirection.z = movement.z;

        if (characterController.isGrounded)
        {
            moveDirection.y = (Input.GetButton("Jump") && canMove) ? jumpPower : 0f;

            if (movement.magnitude > 0.1f && canMove)
            {
                walkSoundTimer -= Time.deltaTime;
                if (walkSoundTimer <= 0f)
                {
                    AudioSource.PlayClipAtPoint(walkingClip, transform.position);
                    walkSoundTimer = walkSoundCooldown;
                }
            }
            else walkSoundTimer = 0f;
        }
        else moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (!canMove) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
        {
            var enemy = hit.transform.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(Mathf.RoundToInt(damage));

            if (impactEffect != null)
            {
                var go = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(go, 2f);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (Time.time - lastHitTime < damageCooldown)
            return;

        lastHitTime = Time.time;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
        if (enemyHitClip != null)
            AudioSource.PlayClipAtPoint(enemyHitClip, transform.position);

        Debug.Log("Player took damage! Current health: " + currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
        Debug.Log("Healed! New health: " + currentHealth);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
        Debug.Log("Gold collected! Total: " + gold);
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = "Gold: " + gold;
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Add respawn or game over logic here
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (Time.time - lastHitTime >= damageCooldown)
            {
                TakeDamage(5); // Normal enemy does 5 damage
                lastHitTime = Time.time;
            }
        }
        else if (hit.gameObject.CompareTag("Boss"))
        {
            if (Time.time - lastHitTime >= damageCooldown)
            {
                TakeDamage(20); // Boss does 20 damage!
                lastHitTime = Time.time;
            }
        }

        var item = hit.gameObject.GetComponent<HealthItem>();
        if (item != null)
        {
            Heal(item.healAmount);
            Destroy(hit.gameObject);
        }
    }
}
