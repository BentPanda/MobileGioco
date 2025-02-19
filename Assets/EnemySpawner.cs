using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        // Wczytujemy, które typy wrogów s¹ odblokowane
        hardEnemyPurchased = PlayerPrefs.GetInt("HardEnemyPurchased", 0) == 1;
        bulletEnemyPurchased = PlayerPrefs.GetInt("BulletEnemyPurchased", 0) == 1;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            playerTransform = playerObject.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnRate;
            spawnRate = Mathf.Max(0.5f, spawnRate - spawnIncreaseRate);
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = GetEdgeSpawnPosition();
        GameObject enemyToSpawn = SelectEnemyToSpawn();
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetEdgeSpawnPosition()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float distance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        return playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
    }

    private GameObject SelectEnemyToSpawn()
    {
        List<GameObject> availableEnemies = new List<GameObject> { normalEnemyPrefab };

        if (hardEnemyPurchased)
            availableEnemies.Add(hardEnemyPrefab);
        if (bulletEnemyPurchased)
            availableEnemies.Add(bulletEnemyPrefab);

        int index = Random.Range(0, availableEnemies.Count);
        return availableEnemies[index];
    }
}
