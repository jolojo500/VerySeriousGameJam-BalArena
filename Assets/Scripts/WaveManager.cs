using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Acts")]
    public int CurrentAct = 0;

    [Header("Spawning")]
    public Transform[] SpawnPoints;
    public GameObject EnemyPrefab;

    [Header("Wave Data")]
    public int[] EnemiesPerAct;
    public float SpawnDelay = 1f;

    int enemiesAlive;
    bool spawning;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartAct(0));
    }

    IEnumerator StartAct(int act)
    {
        CurrentAct = act;

        spawning = true;

        for (int i = 0; i < EnemiesPerAct[act]; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(SpawnDelay);
        }

        spawning = false;
    }

    void SpawnEnemy()
    {
        Transform point =
            SpawnPoints[Random.Range(0, SpawnPoints.Length)];

        Instantiate(EnemyPrefab,
                    point.position,
                    Quaternion.identity);

        enemiesAlive++;
    }

    public void EnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && !spawning)
        {
            NextAct();
        }
    }

    void NextAct()
    {
        CurrentAct++;

        if (CurrentAct < EnemiesPerAct.Length)
            StartCoroutine(StartAct(CurrentAct));
        else
            SpawnBoss();
    }

    void SpawnBoss()
    {
        Debug.Log("GINGERBREAD KING");
    }
}