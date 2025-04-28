using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Loot")]
    [Tooltip("Prefab of the Strawberry (must have StrawberryItem.cs)")]
    public GameObject strawberryPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // 1) Notify LevelManager (portal logic)
        LevelManager gm = FindObjectOfType<LevelManager>();
        if (gm != null)
            gm.NotifyEnemyDeath();

        // 2) Spawn a strawberry at this position
        if (strawberryPrefab != null)
            Instantiate(strawberryPrefab, transform.position, Quaternion.identity);

        // 3) Destroy the enemy
        Destroy(gameObject);
    }
}
