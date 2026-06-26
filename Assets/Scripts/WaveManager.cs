using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;

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

    [Header("Act objects")]
    public GameObject Props;
    public Material floor;
    public Color floor_color;

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

    [Header("Act Transition")]
    public DoubleCurtainController CurtainController;

    public GameObject[] ActTransitionPanels;

    public float TransitionPanelWaitTime = 5f;

    [Header("Freeze Gameplay During Transition")]
    public MonoBehaviour[] ScriptsToDisableDuringTransition;
    public Rigidbody[] RigidbodiesToFreezeDuringTransition;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    private Coroutine actCoroutine;
    private bool actRunning;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        HideAllTransitionPanels();

        StartCoroutine(StartActWithTransitionRoutine(0));
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
        point.position = new Vector3(point.position.x, point.position.y, Random.Range(-0.1f, 0.1f));

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
    public void EnemyKilled()
    {
        EnemyKilled(null);
    }

    void EndAct()
    {
        actRunning = false;

        Debug.Log($"Act {CurrentAct + 1} finished.");

        ClearRemainingEnemies();

        int nextAct = CurrentAct + 1;

        StartCoroutine(StartActWithTransitionRoutine(nextAct));
    }

    void NextAct()
    {
        int nextAct = CurrentAct + 1;
        GlobalVolumeEffects.Instance.PlayAct(nextAct);
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
    IEnumerator StartActWithTransitionRoutine(int actToStart)
    {
        MusicManager.Instance.StopAllMusicWithFade();
        GlobalVolumeEffects.Instance.PlayAct(actToStart);
        FreezeGameplay();

        // close curtains
        if (CurtainController != null && CurtainController.isOpen)
        {
            SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.Cheer, gameObject.transform);
            StartCoroutine(CurtainController.PlayClose());
            yield return new WaitForSeconds(TransitionPanelWaitTime);
        }

        // show act transition panel
        GameObject transitionPanel = null;

        if (ActTransitionPanels != null && actToStart < ActTransitionPanels.Length)
        {
            transitionPanel = ActTransitionPanels[actToStart];

            if (transitionPanel != null)
                transitionPanel.SetActive(true);
        }

        yield return new WaitForSeconds(TransitionPanelWaitTime);

        // hide act transition panel
        if (transitionPanel != null)
            transitionPanel.SetActive(false);
        for (int i = 0; i < Acts.Length; i++)
        {
            if (Acts[i] != null && Acts[i].Props != null)
                Acts[i].Props.SetActive(i == actToStart);
        }
        SetMaterialColor(Acts[actToStart].floor, Acts[actToStart].floor_color);
        // open curtains
        if (CurtainController != null)
        {
            StartCoroutine(CurtainController.PlayOpen());
            yield return new WaitForSeconds(TransitionPanelWaitTime);
        }
        MusicManager.Instance.PlayActMusic(actToStart);
        UnfreezeGameplay();
        
        // start act after transition is fully finished
        if (actToStart < Acts.Length-1)
        {            
            StartAct(actToStart);
        }
        else
        {
            SpawnBoss();
        }
    }
    void HideAllTransitionPanels()
    {
        if (ActTransitionPanels == null)
            return;

        foreach (GameObject panel in ActTransitionPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
    void FreezeGameplay()
    {
        // Block custom player input
        if (NeoInputManager.Instance != null)
            NeoInputManager.Instance.BlockInput = true;

        // Disable gameplay scripts
        foreach (MonoBehaviour script in ScriptsToDisableDuringTransition)
        {
            if (script != null)
                script.enabled = false;
        }

        // Freeze rigidbodies
        foreach (Rigidbody rb in RigidbodiesToFreezeDuringTransition)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    void UnfreezeGameplay()
    {
        // Re-enable gameplay scripts
        foreach (MonoBehaviour script in ScriptsToDisableDuringTransition)
        {
            if (script != null)
                script.enabled = true;
        }

        // Unfreeze rigidbodies
        foreach (Rigidbody rb in RigidbodiesToFreezeDuringTransition)
        {
            if (rb != null)
                rb.isKinematic = false;
        }

        // Unblock custom player input
        if (NeoInputManager.Instance != null)
            NeoInputManager.Instance.BlockInput = false;
    }
    void SetMaterialColor(Material mat, Color color)
    {
        // URP Lit shader usually uses _BaseColor
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        // Standard shader usually uses _Color
        else if (mat.HasProperty("_Color"))
        {
            mat.SetColor("_Color", color);
        }
        else
        {
            Debug.LogWarning($"{mat.name} does not have _BaseColor or _Color.");
        }
    }

}