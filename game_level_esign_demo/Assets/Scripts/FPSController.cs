using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public AudioClip walkingClip;
    public AudioClip enemyHitClip;

    private float walkSoundCooldown = 0.5f;
    private float walkSoundTimer = 0f;

    public Camera playerCamera;
    public float walkSpeed = 12f;
    public float runSpeed = 24f;
    public float jumpPower = 7f;
    public float gravity = 9.5f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    public bool canMove = true;

    private CharacterController characterController;

    // Health system
    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthText;

    public float damageCooldown = 1.5f;
    private float lastHitTime = -999f;

    // Gold system
    public int gold = 0;
    public TMP_Text goldText;

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
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 horizontalMovement = right * moveHorizontal * speed;
        Vector3 verticalMovement = forward * moveVertical * speed;
        Vector3 movement = horizontalMovement + verticalMovement;

        moveDirection.x = movement.x;
        moveDirection.z = movement.z;

        if (characterController.isGrounded)
        {
            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = 0;
            }

            if (movement.magnitude > 0.1f && canMove)
            {
                walkSoundTimer -= Time.deltaTime;
                if (walkSoundTimer <= 0f)
                {
                    AudioSource.PlayClipAtPoint(walkingClip, transform.position);
                    walkSoundTimer = walkSoundCooldown;
                }
            }
            else
            {
                walkSoundTimer = 0f;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthUI();

        if (enemyHitClip != null)
        {
            AudioSource.PlayClipAtPoint(enemyHitClip, transform.position);
        }

        Debug.Log("Player took damage! Current health: " + currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

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
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }
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

        if (hit.gameObject.GetComponent<HealthItem>() != null)
        {
            var item = hit.gameObject.GetComponent<HealthItem>();
            Heal(item.healAmount);
            Destroy(hit.gameObject);
        }
    }

}
