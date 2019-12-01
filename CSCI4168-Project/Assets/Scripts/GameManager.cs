using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text playerCoinText;
    private int playerCoins = 0;

    private Button startGameButton;

    public static int roundIndex = 1;           // first round index is at 1

    public Transform barnAttackPosition;
    public Transform[] enemySpawns;
    public GameObject groundEnemyPrefab;
    public GameObject ufoEnemyPrefab;

    private int enemySpawnIndex = 0;

    private float nextEnemySpawnTime = 0f;      // time until next enemy is spawned (enemy spawn rate + random variance) 
    public float enemySpawnRate = 14f;         // in seconds
    public float enemySpawnVariance = 3f;      // in seconds

    private float startTime = 0f;
    private float roundTime = 3 * 60f;
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
        }

        FindReferences();
        startTime = Time.realtimeSinceStartup;
        StartCoroutine(ShowTimer());
    }

    private IEnumerator StartGame()
    {
        roundIndex = 0;

        SceneManager.LoadScene("FrontEnd");

        yield return new WaitForEndOfFrame();

        FindReferences();
    }

    public Transform GetPenTransform()
    {
        return barnAttackPosition?.parent?.Find("Pen");
    }

    public BarnController GetBarnController()
    {
        return barnAttackPosition?.parent?.GetComponent<BarnController>();
    }

    public Transform GetNearestSpawn(Vector3 position)
    {
        Transform nearest = null;

        float dist = float.MaxValue;

        foreach (Transform enemySpawn in enemySpawns)
        {
            if ((enemySpawn.position - position).sqrMagnitude < dist)
            {
                nearest = enemySpawn;
                dist = (enemySpawn.position - position).sqrMagnitude;
            }
        }

        return nearest;
    }

    // return index between 0 and second last scene index
    private int GetNextLevelIndex()
    {
        return (SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings - 1);
    }

    public void StartRoundCoroutine()
    {
        roundIndex = GetNextLevelIndex();
    }

    public void StartRound(int roundNum)
    {
        roundIndex = roundNum;
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        killCounter = 0;
        startTime = Time.realtimeSinceStartup;

        SceneManager.LoadScene(roundIndex);

        yield return new WaitForEndOfFrame();

        FindReferences();

        StartCoroutine(ShowTimer());
    }

    public void EndRound(bool playerWon = false)
    {
        StartCoroutine(LoadEndOfRoundScene(playerWon));
    }
    
    private IEnumerator SpawnEnemies()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time > nextEnemySpawnTime)
            {
                enemySpawnIndex = Mathf.Min(Random.Range(0, enemySpawns.Length + 1), enemySpawns.Length - 1);

                if (enemySpawns[enemySpawnIndex] != null)

                {
                    GameObject enemy;

                    // spawn ufo units 10% of the time
                    if (Random.value > 0.9f)
                    {
                        enemy = GameObject.Instantiate(ufoEnemyPrefab, enemySpawns[enemySpawnIndex].position, Quaternion.identity);
                    }
                    
                    // spawn ground units 90% of the time
                    //else
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

    private IEnumerator LoadEndOfRoundScene(bool playerWon)
    {
        int index = GetNextLevelIndex();        // save index of next level

        SceneManager.LoadScene("EndOfRound");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Text timerText = GameObject.Find("Canvas")?.transform?.Find("Panel")?.transform?.Find("Timer")?.GetChild(0)?.GetComponent<Text>();
        Text killText = GameObject.Find("Canvas")?.transform?.Find("Panel")?.transform?.Find("Kill Counter")?.GetChild(0)?.GetComponent<Text>();
        Text resultText = GameObject.Find("Canvas")?.transform?.Find("Panel")?.transform?.Find("Result")?.Find("Text")?.GetComponent<Text>();

        if (timerText == null)
            Debug.LogWarning("Cant find timer text");

        if (killText == null)
            Debug.LogWarning("Cant find kill text");

        if (resultText == null)
            Debug.LogWarning("Cant find result text");

        killText.text = killCounter.ToString();
        resultText.text = playerWon ? "You Won!" : "You Lost!";

        for (int i = 0; i < 5; i++)
        {
            if (timerText != null)
            {
                timerText.text = (5 - i).ToString();
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }


        roundIndex = playerWon ? index : 0;      // go to next level if player won, or go back to main menu
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
                roundTimerText.text = (int)(roundTime - time) / 60 + ":" + ((int)(roundTime - time) % 60);
            }

            // add approx 1 coin per every 10s to avoid player running out of resources
            if ((roundTime - time) % 10 < 0.01f)
            {
                AddCoins(1);
            }

            yield return new WaitForEndOfFrame();
        }

        EndRound(true);
    }

    private void FindReferences()
    {
        // front end
        if (SceneManager.GetActiveScene().buildIndex == 0 && startGameButton == null)
        {
            startGameButton = GameObject.Find("Canvas")?.transform.Find("Button")?.GetComponent<Button>();
            startGameButton?.onClick.AddListener(StartRoundCoroutine);
        }

        else
        {
            Transform levelBase = GameObject.Find("Level Base")?.transform;

            // find spawn and barn positions under the levelBase object
            if (levelBase != null)
            {
                playerCoinText = levelBase.Find("UI")?.Find("Coin Text")?.GetComponent<Text>();

                roundTimerText = levelBase.Find("UI")?.Find("Round Timer")?.Find("Text")?.GetComponent<Text>();
                roundTimerText.text = (int)(roundTime) / 60 + ":" + ((int)roundTime % 60);

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

    public bool SpendCoins(int num)
    {
        if (playerCoins >= num)
        {
            playerCoins -= num;
            playerCoinText.text = playerCoins.ToString();

            return true;
        }

        return false;
    }

    public void AddCoins(int num)
    {
        playerCoins += num;
        if (playerCoinText != null)
            playerCoinText.text = playerCoins.ToString();
    }
}
