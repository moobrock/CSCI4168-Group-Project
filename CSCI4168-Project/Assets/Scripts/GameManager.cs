using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int roundIndex = 1;       // first round index is at 1

    public Transform barnAttackPosition;

    public Transform enemySpawn;
    public GameObject enemyPrefab;

    private float nextEnemySpawnTime = 0f;  // time until next enemy is spawned (enemy spawn rate + random variance) 
    private float enemySpawnRate = 8f;      // in seconds
    private float enemySpawnVariance = 1f;  // in seconds

    private float time = 0f;

    public static GameManager gameManager;

    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this);
        }

        else
        {
            gameManager = this;
            DontDestroyOnLoad(this);
            FindReferences();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            time += Time.deltaTime;

            if (time > nextEnemySpawnTime)
            {
                GameObject enemy = GameObject.Instantiate(enemyPrefab, enemySpawn.position, Quaternion.identity);

                time = 0f;

                nextEnemySpawnTime = enemySpawnRate + Random.Range(-enemySpawnVariance, enemySpawnVariance);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void StartGame()
    {
        roundIndex = 1;

        StartCoroutine(LoadScene());
    }

    public void EndRound()
    {
        enemySpawn = null;
        barnAttackPosition = null;


        if (roundIndex >= 0 && roundIndex < SceneManager.sceneCount)
        {
            Debug.Log("End of round - changing scene to " + SceneManager.GetSceneAt(roundIndex).name);

            // load next round
            StartCoroutine(LoadScene());

            roundIndex++;
        }

        else
        {
            roundIndex = 0;

            Debug.Log("End of round - changing scene to " + SceneManager.GetSceneAt(roundIndex).name);

            SceneManager.LoadScene(roundIndex);
        }
    }

    public void EndGame()
    {
        enemySpawn = null;
        barnAttackPosition = null;

        // load front end
        SceneManager.LoadScene(0);
    }

    public Transform FindBarn()
    {
        if (barnAttackPosition == null)
            barnAttackPosition = GameObject.Find("Level Base")?.transform.Find("Barn")?.transform.GetChild(0);

        return barnAttackPosition;
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(roundIndex);

        while (!op.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        FindReferences();
    }

    private void FindReferences()
    {
        Debug.Log("Finding scene references in scene " + SceneManager.GetActiveScene().name);

        // find spawn and barn positions under the Level Base object
        barnAttackPosition = GameObject.Find("Level Base")?.transform.Find("Barn")?.transform.GetChild(0);
        enemySpawn = GameObject.Find("Level Base")?.transform.Find("Enemy Spawn")?.transform;

        // found the positions, start spawning enemies
        if (enemySpawn != null && barnAttackPosition != null)
        {
            StopAllCoroutines();
            StartCoroutine(SpawnEnemies());
        }

        // positions should be found unless it is the front end
        else
            Debug.LogWarning("No enemy spawn or barn attack position - is this the front end?");
    }
}
