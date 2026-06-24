using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnPercent
{
    public GameObject EnemyPrefab;

    [Range(0f, 100f)]
    public float SpawnPercent = 100f;
}

[System.Serializable]
public class ActData
{
    public string ActName = "Act";

    [Header("Act Rules")]
    public int TotalEnemiesInAct = 20;
    public int MaxEnemiesAlive = 3;
    public float ActDuration = 120f;

    [Header("Spawn Percentages")]
    public EnemySpawnPercent[] SpawnTable;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Acts")]
    public int CurrentAct = 0;
    public ActData[] Acts;

    [Header("Spawning")]
    public Transform[] SpawnPoints;
    public float SpawnDelay = 1f;

    [Header("Debug")]
    [SerializeField] private int enemiesAlive;
    [SerializeField] private int enemiesSpawnedThisAct;
    [SerializeField] private int enemiesKilledThisAct;
    [SerializeField] private float actTimer;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    private Coroutine actCoroutine;
    private bool actRunning;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartAct(0);
    }

    public void StartAct(int act)
    {
        if (actCoroutine != null)
            StopCoroutine(actCoroutine);

        actCoroutine = StartCoroutine(ActRoutine(act));
    }

    IEnumerator ActRoutine(int act)
    {
        if (act >= Acts.Length)
        {
            SpawnBoss();
            yield break;
        }

        CurrentAct = act;

        ActData data = Acts[CurrentAct];

        enemiesAlive = 0;
        enemiesSpawnedThisAct = 0;
        enemiesKilledThisAct = 0;
        actTimer = data.ActDuration;
        actRunning = true;

        ClearRemainingEnemies();

        MusicManager.Instance.PlayActMusic(CurrentAct);

        float spawnCooldown = 0f;

        Debug.Log($"Starting {data.ActName}");

        while (actRunning)
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
            enemiesAlive = activeEnemies.Count;

            actTimer -= Time.deltaTime;
            spawnCooldown -= Time.deltaTime;

            bool actTimeOver = actTimer <= 0f;
            bool allEnemiesKilled = enemiesKilledThisAct >= data.TotalEnemiesInAct;

            if (actTimeOver || allEnemiesKilled)
            {
                EndAct();
                yield break;
            }

            bool canSpawnMoreTotal = enemiesSpawnedThisAct < data.TotalEnemiesInAct;
            bool hasRoomForEnemy = enemiesAlive < data.MaxEnemiesAlive;
            bool spawnReady = spawnCooldown <= 0f;

            if (canSpawnMoreTotal && hasRoomForEnemy && spawnReady)
            {
                SpawnEnemy(data);
                spawnCooldown = SpawnDelay;
            }

            yield return null;
        }
    }

    void SpawnEnemy(ActData data)
    {
        if (SpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to WaveManager.");
            return;
        }

        GameObject prefab = PickEnemyPrefab(data);

        if (prefab == null)
        {
            Debug.LogError($"No valid enemy prefab found for Act {CurrentAct}.");
            return;
        }

        Transform point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];

        GameObject enemy = Instantiate(
            prefab,
            point.position,
            Quaternion.identity
        );

        activeEnemies.Add(enemy);

        enemiesSpawnedThisAct++;
        enemiesAlive = activeEnemies.Count;
    }

    GameObject PickEnemyPrefab(ActData data)
    {
        float totalPercent = 0f;

        foreach (EnemySpawnPercent option in data.SpawnTable)
        {
            if (option.EnemyPrefab != null && option.SpawnPercent > 0f)
                totalPercent += option.SpawnPercent;
        }

        if (totalPercent <= 0f)
            return null;

        float roll = Random.Range(0f, totalPercent);

        foreach (EnemySpawnPercent option in data.SpawnTable)
        {
            if (option.EnemyPrefab == null || option.SpawnPercent <= 0f)
                continue;

            if (roll <= option.SpawnPercent)
                return option.EnemyPrefab;

            roll -= option.SpawnPercent;
        }

        return null;
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (!actRunning)
            return;

        if (enemy != null)
            activeEnemies.Remove(enemy);

        enemiesKilledThisAct++;

        activeEnemies.RemoveAll(e => e == null);
        enemiesAlive = activeEnemies.Count;
    }

    // Optional old version, in case some code already calls EnemyKilled()
    public void EnemyKilled()
    {
        EnemyKilled(null);
    }

    void EndAct()
    {
        actRunning = false;

        Debug.Log($"Act {CurrentAct + 1} finished.");

        ClearRemainingEnemies();

        NextAct();
    }

    void NextAct()
    {
        int nextAct = CurrentAct + 1;

        if (nextAct < Acts.Length)
        {
            StartAct(nextAct);
        }
        else
        {
            SpawnBoss();
        }
    }

    void ClearRemainingEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
                Destroy(activeEnemies[i]);
        }

        activeEnemies.Clear();
        enemiesAlive = 0;
    }

    void SpawnBoss()
    {
        Debug.Log("GINGERBREAD KING");
    }
}