using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int roundIndex = 1;       // first round index is at 1

    public Transform barnAttackPosition;

    public Transform[] enemySpawns;
    public GameObject enemyPrefab;

    private int enemySpawnIndex = 0;

    private float nextEnemySpawnTime = 0f;  // time until next enemy is spawned (enemy spawn rate + random variance) 
    private float enemySpawnRate = 8f;      // in seconds
    private float enemySpawnVariance = 1f;  // in seconds

    private float time = 0f;
    private float startTime = 0f;
    private float roundTime = 5 * 60f;

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

        startTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
       if (Time.realtimeSinceStartup > startTime + roundTime)
        {
            EndRound();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            time += Time.deltaTime;

            if (time > nextEnemySpawnTime)
            {
                enemySpawnIndex = (enemySpawnIndex + 1) % enemySpawns.Length;

                GameObject enemy = GameObject.Instantiate(enemyPrefab, enemySpawns[enemySpawnIndex].position, Quaternion.identity);

                time = 0f;

                nextEnemySpawnTime = enemySpawnRate + Random.Range(-enemySpawnVariance, enemySpawnVariance);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void StartGame()
    {
        StopAllCoroutines();

        roundIndex = 1;

        StartCoroutine(LoadScene());
    }

    public void EndRound()
    {
        StopAllCoroutines();

        enemySpawns = null;
        barnAttackPosition = null;

        roundIndex = SceneManager.GetActiveScene().buildIndex + 1;

        Debug.Log("End of round - changing scene." + 
                  "\n\tScene index: " + roundIndex + 
                  "\n\tScene count: " + SceneManager.sceneCountInBuildSettings +
                  "\n\tOld scene: " + SceneManager.GetActiveScene().name + 
                  ((SceneManager.sceneCount > roundIndex) ? 
                  ("\n\tNext scene: " + SceneManager.GetSceneAt(roundIndex)) : ""));

        if (roundIndex >= 0 && roundIndex < SceneManager.sceneCountInBuildSettings)
        {
            // load next round
            StartCoroutine(LoadScene());
        }

        else
        {
            roundIndex = 0;

            SceneManager.LoadScene(roundIndex);
        }
    }

    public void EndGame()
    {
        enemySpawns = null;
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

        int count = GameObject.Find("Level Base")?.transform.Find("Enemy Spawns")?.childCount ?? 0;
        enemySpawns = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            enemySpawns[i] = GameObject.Find("Level Base")?.transform.Find("Enemy Spawns")?.GetChild(i).transform;
        }

        // found the positions, start spawning enemies
        if (enemySpawns != null && enemySpawns.Length > 0 && barnAttackPosition != null)
        {
            StopAllCoroutines();
            StartCoroutine(SpawnEnemies());
        }

        // positions should be found unless it is the front end
        else
            Debug.LogWarning("No enemy spawn or barn attack position - is this the front end?");
    }
}
