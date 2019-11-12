using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] towerAttackPositions;
    public Transform barnAttackPosition;

    public Transform enemySpawn;
    public GameObject enemyPrefab;

    private float enemySpawnRate = 3f;      // in seconds
    private float enemySpawnVariance = 1f;  // in seconds

    private float time = 0f;

    public static GameManager gameManager;

    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(gameManager);
        }

        gameManager = this;

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            time += Time.deltaTime;

            if (time > enemySpawnRate)
            {
                SpawnEnemy();

                time = 0f;
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = GameObject.Instantiate(enemyPrefab, enemySpawn.position, Quaternion.identity);
    }
}
