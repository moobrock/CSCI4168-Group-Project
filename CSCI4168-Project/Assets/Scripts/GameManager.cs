using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Button startGameButton;

    public static int roundIndex = 1;           // first round index is at 1

    public Transform barnAttackPosition;

    public Transform[] enemySpawns;
    public GameObject groundEnemyPrefab;
    public GameObject ufoEnemyPrefab;

    private int enemySpawnIndex = 0;

    private float nextEnemySpawnTime = 0f;      // time until next enemy is spawned (enemy spawn rate + random variance) 
    private float enemySpawnRate = 10f;         // in seconds
    private float enemySpawnVariance = 5f;      // in seconds

    private float startTime = 0f;
    private float roundTime = 5 * 60f;
    public Text roundTimerText;

    private int killCounter = 0;

    public static GameManager gameManager;

    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(gameObject);
        }

        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);

            //StartCoroutine(StartGame());

            startTime = Time.realtimeSinceStartup;
            FindReferences();
            StartCoroutine(ShowTimer());
        }

        startTime = Time.realtimeSinceStartup;

        FindReferences();
    }

    private IEnumerator StartGame()
    {
        roundIndex = 0;

        SceneManager.LoadScene("FrontEnd");

        yield return new WaitForEndOfFrame();

        FindReferences();
    }

    // return index between 0 and second last scene index
    private int GetNextLevelIndex()
    {
        return (SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings - 1);
    }

    public void StartRoundCoroutine()
    {
        roundIndex = GetNextLevelIndex();

        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        Debug.Log("Starting Round " + (roundIndex));

        StopAllCoroutines();

        startTime = Time.realtimeSinceStartup;

        SceneManager.LoadScene(roundIndex);

        yield return new WaitForEndOfFrame();

        FindReferences();

        StartCoroutine(ShowTimer());
    }

    public void EndRound()
    {
        StopAllCoroutines();

        StartCoroutine(LoadEndOfRoundScene());
    }
    
    private IEnumerator SpawnEnemies()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time > nextEnemySpawnTime)
            {
                enemySpawnIndex = (enemySpawnIndex + 1) % enemySpawns.Length;

                if (enemySpawns[enemySpawnIndex] != null)

                {
                    GameObject enemy;

                    // spawn ground units 90% of the time
                    if (Random.value > 0.9f)
                    {
                        enemy = GameObject.Instantiate(ufoEnemyPrefab, enemySpawns[enemySpawnIndex].position, Quaternion.identity);
                    }

                    // spawn ufo units 10% of the time
                    else
                    {
                        enemy = GameObject.Instantiate(groundEnemyPrefab, enemySpawns[enemySpawnIndex].position, Quaternion.identity);
                    }
                }

                time = 0f;

                nextEnemySpawnTime = enemySpawnRate + Random.Range(-enemySpawnVariance, enemySpawnVariance);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator LoadEndOfRoundScene()
    {
        int index = GetNextLevelIndex();        // save index of next level

        SceneManager.LoadScene("EndOfRound");

        Text timerText = GameObject.Find("Canvas")?.transform?.Find("Panel")?.transform?.Find("Timer")?.GetComponent<Text>();

        for (int i = 0; i < 5; i++)
        {
            if (timerText != null)
            {
                timerText.text = (i + 1).ToString();
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }

        Debug.Log("Finishing end of round");

        roundIndex = index;                     // go to next level
        StartCoroutine(StartRound());
    }

    private IEnumerator ShowTimer()
    {
        float time = 0f;

        while (time < roundTime)
        {
            time += Time.deltaTime;

            if (roundTimerText != null)
            {
                Debug.Log("Setting round timer text");

                roundTimerText.text = (int)(roundTime - time) / 60 + ":" + ((int)(roundTime - time) % 60);
            }

            yield return new WaitForEndOfFrame();
        }

        EndRound();
    }

    private void FindReferences()
    {
        Debug.Log("Finding scene references in scene " + SceneManager.GetActiveScene().name);

        // front end
        if (SceneManager.GetActiveScene().buildIndex == 0 && startGameButton == null)
        {
            startGameButton = GameObject.Find("Canvas")?.transform.Find("Button")?.GetComponent<Button>();
            startGameButton?.onClick.AddListener(StartRoundCoroutine);
        }

        else
        {
            Transform levelBase = GameObject.Find("Level Base")?.transform;

            Debug.Log("Level Base found? " + (levelBase != null));

            // find spawn and barn positions under the levelBase object
            if (levelBase != null)
            {
                roundTimerText = levelBase.Find("UI")?.Find("Round Timer")?.GetComponent<Text>();
                roundTimerText.text = (int)(roundTime) / 60 + ":" + ((int)roundTime % 60);

                Debug.Log("Round Timer found? " + (roundTimerText != null));

                barnAttackPosition = levelBase.Find("Barn")?.transform.GetChild(0);

                int count = levelBase.Find("Enemy Spawns")?.childCount ?? 0;
                enemySpawns = new Transform[count];

                for (int i = 0; i < count; i++)
                {
                    enemySpawns[i] = levelBase.Find("Enemy Spawns")?.GetChild(i).transform;
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
    }

    public void LogKill()
    {
        killCounter++;
    }

    public Transform FindBarn()
    {
        if (barnAttackPosition == null)
            FindReferences();
        return barnAttackPosition;
    }
}
