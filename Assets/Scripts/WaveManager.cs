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
    public float SpawnDelay = 1f;

    [Header("Spawn Percentages")]
    public EnemySpawnPercent[] SpawnTable;

    [Header("Boss")]
    public GameObject BossPrefab;
    public Transform BossSpawnPoint;
    public bool SpawnBossInThisAct = false;
    public float BossSpawnDelay = 20f;
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
    [Header("Player Reset Between Acts")]
    public Transform PlayerTransform;
    public Vector3 PlayerResetPosition = Vector3.zero;
    public float PlayerResetGlideDuration = 0.75f;
    [Header("Win Transition")]
    public float WinPanelWaitTime = 5f;
    public bool KeepGameplayFrozenAfterWin = true;

    private GameObject activeBoss;
    public Player player;
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
        CurrentAct = act;

        ActData data = Acts[CurrentAct];

        enemiesAlive = 0;
        enemiesSpawnedThisAct = 0;
        enemiesKilledThisAct = 0;
        actTimer = data.ActDuration;
        actRunning = true;

        ClearRemainingEnemies();
        StartCoroutine(SpawnBossAfterDelay(data, CurrentAct));
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
                spawnCooldown = data.SpawnDelay; ;
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

        if (nextAct >= Acts.Length)
        {
            StartCoroutine(WinTransitionRoutine());
            return;
        }

        StartCoroutine(StartActWithTransitionRoutine(nextAct));
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


    IEnumerator StartActWithTransitionRoutine(int actToStart)
    {
        MusicManager.Instance.StopAllMusicWithFade();
        FreezeGameplay();
        

        bool hasValidAct = actToStart >= 0 && actToStart < Acts.Length;

        if (!hasValidAct)
        {
            Debug.Log("No more acts.");
            UnfreezeGameplay();
            yield break;
        }

        GlobalVolumeEffects.Instance.PlayAct(actToStart);

        if (CurtainController != null && CurtainController.isOpen)
        {
            SoundEffectsManager.Instance.PlaySoundFXClip(
                SoundEffectsManager.soundEffects.Cheer,
                gameObject.transform
            );

            StartCoroutine(CurtainController.PlayClose());
            yield return new WaitForSeconds(TransitionPanelWaitTime);
        }

        GameObject transitionPanel = null;

        if (ActTransitionPanels != null && actToStart < ActTransitionPanels.Length)
        {
            transitionPanel = ActTransitionPanels[actToStart];

            if (transitionPanel != null)
                transitionPanel.SetActive(true);
        }
        yield return GlidePlayerToResetPosition();
        yield return new WaitForSeconds(TransitionPanelWaitTime);
        player.Attributes.AddToHealth(1);
        if (transitionPanel != null)
            transitionPanel.SetActive(false);

        for (int i = 0; i < Acts.Length; i++)
        {
            if (Acts[i] != null && Acts[i].Props != null)
                Acts[i].Props.SetActive(i == actToStart);
        }

        if (Acts[actToStart].floor != null)
            SetMaterialColor(Acts[actToStart].floor, Acts[actToStart].floor_color);

        
        if (CurtainController != null)
        {
            StartCoroutine(CurtainController.PlayOpen());
            yield return new WaitForSeconds(TransitionPanelWaitTime);
        }

        MusicManager.Instance.PlayActMusic(actToStart);

        UnfreezeGameplay();

        StartAct(actToStart);
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
    IEnumerator GlidePlayerToResetPosition()
    {
        Transform player = PlayerTransform;

        if (player == null && Player.Instance != null)
            player = Player.Instance.transform;

        if (player == null)
        {
            Debug.LogWarning("No player transform assigned for act reset.");
            yield break;
        }

        Rigidbody playerRb = null;

        if (Player.Instance != null)
            playerRb = Player.Instance.Rb;

        if (playerRb == null)
            playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        Vector3 startPos = player.position;
        float timer = 0f;

        while (timer < PlayerResetGlideDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / PlayerResetGlideDuration);

            // smooth easing
            t = t * t * (3f - 2f * t);

            player.position = Vector3.Lerp(startPos, PlayerResetPosition, t);

            yield return null;
        }

        player.position = PlayerResetPosition;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
    }

    IEnumerator SpawnBossAfterDelay(ActData data, int actIndex)
    {
        if (data == null)
            yield break;

        if (!data.SpawnBossInThisAct)
            yield break;

        float timer = 0f;

        while (timer < data.BossSpawnDelay)
        {
            if (!actRunning)
                yield break;

            if (CurrentAct != actIndex)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        SpawnBossForAct(data);
        yield return new WaitForSeconds(1.25f);
        SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.ThudBoss, gameObject.transform);
        CameraShakeManager.Instance.Rumble(1.0f);
    }

    void SpawnBossForAct(ActData data)
    {
        
        if (data == null)
            return;

        if (data.BossPrefab == null)
        {
            Debug.LogWarning($"Act {CurrentAct + 1} wants to spawn a boss, but no BossPrefab is assigned.");
            return;
        }

        if (activeBoss != null)
            Destroy(activeBoss);

        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        if (data.BossSpawnPoint != null)
        {
            spawnPosition = data.BossSpawnPoint.position;
            spawnRotation = data.BossSpawnPoint.rotation;
        }
        else if (SpawnPoints != null && SpawnPoints.Length > 0)
        {
            Transform randomPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            spawnPosition = randomPoint.position;
            spawnRotation = randomPoint.rotation;
        }

        activeBoss = Instantiate(data.BossPrefab, spawnPosition, spawnRotation);

        Debug.Log($"Spawned boss for {data.ActName} after {data.BossSpawnDelay} seconds.");
    }
    IEnumerator WinTransitionRoutine()
    {
        MusicManager.Instance.StopAllMusicWithFade();

        FreezeGameplay();

        if (CurtainController != null && CurtainController.isOpen)
        {
            SoundEffectsManager.Instance.PlaySoundFXClip(
                SoundEffectsManager.soundEffects.Cheer,
                gameObject.transform
            );

            StartCoroutine(CurtainController.PlayClose());
            yield return new WaitForSeconds(TransitionPanelWaitTime);
        }
        GameManager.Instance.EndGame(false);

        if (!KeepGameplayFrozenAfterWin)
            UnfreezeGameplay();
    }

}