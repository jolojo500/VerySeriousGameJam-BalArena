using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Venice;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player UI")]
    public MeterBar HealthBar;
    public MeterBar SpinBar;
    public MeterBar SuspicionBar;
    public TMP_Text ComboCountText;

    [Header("Boss UI")]
    public MeterBar BossHealthBar;
    public string BossObjectName = "Boss";
    public float BossSearchInterval = 0.25f;

    private BallerinaEntity currentBoss;
    private float bossSearchTimer;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        Venice.Player.Instance.Attributes.OnHealthChanged.AddListener(UpdateHealthBar);
        Venice.Player.Instance.Attributes.OnSpinChanged.AddListener(UpdateSpinBar);
        Venice.Player.Instance.Attributes.OnSuspicionChanged.AddListener(UpdateSuspicionBar);

        if (BossHealthBar != null)
            BossHealthBar.gameObject.SetActive(false);
    }

    void Update()
    {
        bossSearchTimer -= Time.deltaTime;

        if (bossSearchTimer <= 0f)
        {
            bossSearchTimer = BossSearchInterval;
            TryFindBoss();
        }
    }

    void TryFindBoss()
    {
        // If we already have a boss, do nothing.
        if (currentBoss != null)
            return;

        GameObject bossObject = GameObject.Find(BossObjectName);

        if (bossObject == null)
        {
            if (BossHealthBar != null)
                BossHealthBar.gameObject.SetActive(false);

            return;
        }

        BallerinaEntity bossEntity = bossObject.GetComponent<BallerinaEntity>();

        if (bossEntity == null)
        {
            bossEntity = bossObject.GetComponentInChildren<BallerinaEntity>();
        }

        if (bossEntity == null)
        {
            Debug.LogWarning("Boss object found, but it has no BallerinaEntity.");
            return;
        }

        currentBoss = bossEntity;

        currentBoss.Attributes.OnHealthChanged.AddListener(UpdateBossHealthBar);

        if (BossHealthBar != null)
        {
            BossHealthBar.gameObject.SetActive(true);

            UpdateBossHealthBar(
                new Tuple<int, int>(
                    (int)(currentBoss.Attributes.CurrentHealth),
                    (int)(currentBoss.Attributes.MaxHealth)
                )
            );
        }
    }

    public void UpdateHealthBar(Tuple<int, int> healthData)
    {
        UpdateBar(HealthBar, healthData);
    }

    public void UpdateSpinBar(Tuple<int, int> data)
    {
        UpdateBar(SpinBar, data);
    }

    public void UpdateSuspicionBar(Tuple<int, int> data)
    {
        UpdateBar(SuspicionBar, data);
    }

    public void UpdateBossHealthBar(Tuple<int, int> data)
    {
        UpdateBar(BossHealthBar, data);

        if (BossHealthBar != null)
            BossHealthBar.gameObject.SetActive(data.Item1 > 0);
    }

    private void UpdateBar(MeterBar bar, Tuple<int, int> data)
    {
        if (bar == null) return;

        bar.SetMaxAmount(data.Item2);
        bar.SetAmount(data.Item1);
    }
}