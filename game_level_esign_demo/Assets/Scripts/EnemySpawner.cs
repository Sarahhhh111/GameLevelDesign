using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Assign your enemy prefab in the Inspector
    public int enemyCount = 10;     // Number of enemies to spawn
    public float spawnRadius = 10f; // Radius around the spawner to spawn enemies
    public float spawnHeightOffset = 1f; // Height offset to spawn above the terrain (optional)

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        // Randomize position within the defined spawn radius
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomZ = Random.Range(-spawnRadius, spawnRadius);

        // Use the spawner's position as the center
        Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Optionally, adjust the Y position to be above the terrain at the spawn location
        float y = Terrain.activeTerrain.SampleHeight(spawnPosition) + spawnHeightOffset;

        // Set the spawn position with the correct Y value
        return new Vector3(spawnPosition.x, y, spawnPosition.z);
    }
}
