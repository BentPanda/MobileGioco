using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject normalEnemyPrefab;
    public GameObject hardEnemyPrefab;
    public GameObject bulletEnemyPrefab;

    public float spawnRate = 2f;
    private float spawnTimer = 0f;
    public float spawnIncreaseRate = 0.01f;
    public float spawnDistanceMin = 8f;
    public float spawnDistanceMax = 10f;
    private Transform playerTransform;

    private bool hardEnemyPurchased;
    private bool bulletEnemyPurchased;

    void Start()
    {
        // za³aduj kupionych wrogów i jak s¹ kupieni to ich spawnuje
        hardEnemyPurchased = PlayerPrefs.GetInt("HardEnemyPurchased", 0) == 1;
        bulletEnemyPurchased = PlayerPrefs.GetInt("BulletEnemyPurchased", 0) == 1;

        // znajdŸ gracza po tagu
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnRate;
            spawnRate -= spawnIncreaseRate;
            spawnRate = Mathf.Clamp(spawnRate, 0.5f, float.MaxValue);
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition = GetEdgeSpawnPosition();
        GameObject enemyToSpawn = SelectEnemyToSpawn();
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    Vector2 GetEdgeSpawnPosition()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(spawnDistanceMin, spawnDistanceMax);

        Vector2 spawnPosition = new Vector2(
            playerTransform.position.x + Mathf.Cos(angle) * distance,
            playerTransform.position.y + Mathf.Sin(angle) * distance
        );

        return spawnPosition;
    }

    GameObject SelectEnemyToSpawn()
    {
        List<GameObject> availableEnemies = new List<GameObject> { normalEnemyPrefab };

        if (hardEnemyPurchased)
        {
            availableEnemies.Add(hardEnemyPrefab);
        }
        if (bulletEnemyPurchased)
        {
            availableEnemies.Add(bulletEnemyPrefab);
        }

        int index = Random.Range(0, availableEnemies.Count);
        return availableEnemies[index];
    }
}
