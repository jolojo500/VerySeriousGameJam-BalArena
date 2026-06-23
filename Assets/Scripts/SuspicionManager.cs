using System.Collections.Generic;
using UnityEngine;
using Venice;

public class SuspicionManager : MonoBehaviour
{
    public static SuspicionManager Instance { get; private set; }

    private enum SuspicionPhase
    {
        NormalLights,
        Spotlight
    }

    [Header("References")]
    public Player Player;
    public SuspicionZone Zone;

    [Header("Lights")]
    public List<GameObject> NormalLights = new List<GameObject>();
    public GameObject SpotlightObject;

    [Header("Suspicion")]
    public float MaxSuspicion = 100f;
    public float SuspicionRiseRate = 8f;
    public float SuspicionDecreaseRate = 18f;

    [Header("Phase Timing")]
    public float NormalLightsDuration = 30f;
    public float SpotlightDuration = 15f;

    [Header("Random Spotlight Position")]
    public Vector2 XRange = new Vector2(-13f, 13f);
    public Vector2 ZRange = new Vector2(-13f, 13f);
    public float YPosition = 0.05f;

    [Header("Debug")]
    public bool PlayerIsInsideZone;
    public float CurrentSuspicion;
    public string CurrentPhaseName;
    public float PhaseTimer;

    private SuspicionPhase currentPhase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Player == null)
            Player = Player.Instance;

        if (Player != null)
        {
            Player.Attributes.MaxSuspicion = MaxSuspicion;
            Player.Attributes.AddToSuspicion(-9999f);
        }

        StartNormalLightsPhase();
    }

    private void Update()
    {
        if (Player == null)
            Player = Player.Instance;

        if (Player == null || Player.IsDead)
            return;

        PhaseTimer -= Time.deltaTime;

        if (currentPhase == SuspicionPhase.NormalLights)
        {
            if (PhaseTimer <= 0f)
            {
                StartSpotlightPhase();
            }

            return;
        }

        if (currentPhase == SuspicionPhase.Spotlight)
        {
            HandleSuspicion();

            if (PhaseTimer <= 0f)
            {
                StartNormalLightsPhase();
            }
        }
    }

    private void StartNormalLightsPhase()
    {
        currentPhase = SuspicionPhase.NormalLights;
        CurrentPhaseName = "Normal Lights";
        PhaseTimer = NormalLightsDuration;

        SetNormalLightsActive(true);

        if (SpotlightObject != null)
            SpotlightObject.SetActive(false);

        if (Zone != null)
            Zone.SetActive(false);

        PlayerIsInsideZone = false;

        Debug.Log("Normal lights phase started.");
    }

    private void StartSpotlightPhase()
    {
        currentPhase = SuspicionPhase.Spotlight;
        CurrentPhaseName = "Spotlight";
        PhaseTimer = SpotlightDuration;

        SetNormalLightsActive(false);

        if (SpotlightObject != null)
            SpotlightObject.SetActive(true);

        MoveZoneToRandomPosition();

        if (Zone != null)
            Zone.SetActive(true);

        Debug.Log("Spotlight phase started.");
    }

    private void SetNormalLightsActive(bool active)
    {
        foreach (GameObject lightObject in NormalLights)
        {
            if (lightObject != null)
                lightObject.SetActive(active);
        }
    }

    private void HandleSuspicion()
    {
        if (Zone == null)
            return;

        PlayerIsInsideZone = Zone.ContainsPlayer(Player);

        float amount = PlayerIsInsideZone
            ? -SuspicionDecreaseRate * Time.deltaTime
            : SuspicionRiseRate * Time.deltaTime;

        Player.Attributes.AddToSuspicion(amount);

        CurrentSuspicion = Player.Attributes.CurrentSuspicion;

        if (CurrentSuspicion >= MaxSuspicion)
        {
            KillPlayerFromSuspicion();
        }
    }

    private void MoveZoneToRandomPosition()
    {
        if (Zone == null)
            return;

        Vector3 randomPosition = new Vector3(
            Random.Range(XRange.x, XRange.y),
            YPosition,
            Random.Range(ZRange.x, ZRange.y)
        );

        Zone.MoveTo(randomPosition);

        Debug.Log($"Suspicion zone moved to {randomPosition}");
    }

    private void KillPlayerFromSuspicion()
    {
        Debug.Log("Suspicion reached 100. Player dies.");

        Player.TriggerDeath();
    }
}