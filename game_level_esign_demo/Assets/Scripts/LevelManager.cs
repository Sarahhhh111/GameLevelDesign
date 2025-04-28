using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("How many enemies must die before we spawn the portal")]
    public int totalEnemies = 5;

    [Tooltip("Portal prefab (with Portal.cs attached)")]
    public GameObject portalPrefab;

    [Tooltip("Where in the scene to put the portal")]
    public Transform portalSpawnPoint;

    private int killCount = 0;
    private bool portalSpawned = false;

    // Call this from each EnemyAI when it dies:
    public void NotifyEnemyDeath()
    {
        killCount++;
        if (!portalSpawned && killCount >= totalEnemies)
        {
            SpawnPortal();
        }
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null || portalSpawnPoint == null) return;

        Instantiate(
            portalPrefab,
            portalSpawnPoint.position,
            portalSpawnPoint.rotation);

        portalSpawned = true;
        Debug.Log("All enemies dead! Portal spawned.");
    }
}
