using System.Collections.Generic;
using UnityEngine;
using Venice;
public enum AIStartMode
{
    WanderUntilHit,
    HostileByDefault
}   
public class AIStateMachine : StateMachine<CPUAIState>
{
    private static readonly List<AIStateMachine> AllBallerinaAIs = new();

    [Header("References")]
    public CPUInputManager CPUInput { get; private set; }
    public BallerinaEntity ControlledEntity;

    [Header("Aggro")]
    public bool IsAggressive { get; private set; }
    public bool AlertAllBallerinasOnHit = false;

    [Header("Wandering")]
    public float ArenaRange = 13f;
    public float WanderPointReachedDistance = 1.2f;
    public float WanderRetargetMinTime = 2f;
    public float WanderRetargetMaxTime = 5f;

    [Header("Spacing From Other Ballerinas")]
    public float SeparationRadius = 3f;
    public float HardSeparationRadius = 1.5f;
    public float WanderSeparationWeight = 1.5f;
    public float AggressiveSeparationWeight = 1.1f;

    [Header("Hostile Movement")]
    public float PreferredPlayerDistance = 3f;
    public float PreferredDistanceBuffer = 0.6f;
    public float StrafeWeight = 0.45f;

    [Header("Dash Attack")]
    public float DashCooldownMin = 2f;
    public float DashCooldownMax = 4f;
    public float DashDuration = 0.45f;
    public float DashImpulse = 7f;
    public float DashStartMaxDistance = 8f;
    public float DashAttackButtonTime = 0.15f;
    public float DashWindUpDuration = 1.0f;
    public float DashRecoveryDuration = 0.65f;
    public float WindUpTurnSpeed = 8f;
    public string WindUpBoolName = "ChargingAttack";
    public string AttackTriggerName = "Attack";

    [Header("Direct Player Push")]
    public bool DirectlyPushPlayerOnDash = true;
    public float DashPushRadius = 1.6f;
    public float DashPushForce = 8f;
    public float DashPushUpForce = 1.5f;

    [Header("Random Jumping")]
    public bool CanRandomJump = true;
    public float RandomJumpMinTime = 1.5f;
    public float RandomJumpMaxTime = 4f;
    public float JumpButtonPressTime = 0.12f;
    public float JumpChanceWhenTimerEnds = 0.7f;
    public float AIJumpSpeed = 12f;

    [Header("Stun")]
    public float DefaultStunDuration = 0.45f;
    private float stunTimer;
    public bool IsStunned => stunTimer > 0f;
    
    [Header("Aggro")]
    public AIStartMode StartMode = AIStartMode.WanderUntilHit;

    private void Awake()
    {
        CPUInput = GetComponent<CPUInputManager>();

        if (ControlledEntity == null)
            ControlledEntity = GetComponent<BallerinaEntity>();

        if (ControlledEntity == null)
            ControlledEntity = GetComponentInParent<BallerinaEntity>();

        if (CPUInput != null && ControlledEntity != null)
        {
            CPUInput.Entity = ControlledEntity;
            CPUInput.AIMachine = this;

            ControlledEntity.InputManager = CPUInput;

            Debug.Log($"{name}: AI connected to {ControlledEntity.name}");
        }
        else
        {
            Debug.LogError($"{name}: AI NOT CONNECTED. CPUInput={CPUInput != null}, ControlledEntity={ControlledEntity != null}");
        }
    }

    private void OnEnable()
    {
        if (!AllBallerinaAIs.Contains(this))
            AllBallerinaAIs.Add(this);
    }

    private void OnDisable()
    {
        AllBallerinaAIs.Remove(this);
    }

    private void Start()
    {
        if (CPUInput == null || ControlledEntity == null)
        {
            Debug.LogError($"{name}: AI missing CPUInputManager or BallerinaEntity.");
            enabled = false;
            return;
        }

        Add(new StandByState());
        Add(new HostileState());
        Add(new RunAwayState());

        if (StartMode == AIStartMode.HostileByDefault)
        {
            IsAggressive = true;
            Initialize<HostileState>();
        }
        else
        {
            IsAggressive = false;
            Initialize<StandByState>();
        }
    }

    public override void Add(CPUAIState state)
    {
        base.Add(state);
        state.AIMachine = this;
        state.FollowerCPU = CPUInput;
    }

    private void Update()
    {
        if (ControlledEntity != null && ControlledEntity.IsDead)
        {
            ClearInputs();
            return;
        }

        if (IsStunned)
        {
            stunTimer -= Time.deltaTime;
            ClearInputs();
            return;
        }

        CurrentState?.OnUpdate();
    }

    public void AlertFromHit()
    {
        Alert(false);

        if (!AlertAllBallerinasOnHit)
            return;

        foreach (AIStateMachine ai in AllBallerinaAIs)
        {
            if (ai == null || ai == this)
                continue;

            ai.Alert(true);
        }
    }

    private void Alert(bool fromAlly)
    {
        if (IsAggressive)
            return;

        IsAggressive = true;

        Set<HostileState>();
    }

    public Vector3 GetRandomWanderPoint()
    {
        return new Vector3(
            Random.Range(-ArenaRange, ArenaRange),
            transform.position.y,
            Random.Range(-ArenaRange, ArenaRange)
        );
    }

    public Vector3 GetSeparationVector()
    {
        Vector3 separation = Vector3.zero;
        Vector3 myPosition = transform.position;

        foreach (AIStateMachine other in AllBallerinaAIs)
        {
            if (other == null || other == this || other.ControlledEntity == null)
                continue;

            Vector3 away = myPosition - other.transform.position;
            away.y = 0f;

            float distance = away.magnitude;

            if (distance <= 0.001f)
            {
                away = Random.insideUnitSphere;
                away.y = 0f;
                distance = 0.1f;
            }

            if (distance < SeparationRadius)
            {
                float strength = 1f - Mathf.Clamp01(distance / SeparationRadius);

                if (distance < HardSeparationRadius)
                    strength += 1f;

                separation += away.normalized * strength;
            }
        }

        return Vector3.ClampMagnitude(separation, 1f);
    }

    public Vector2 WorldDirectionToMoveInput(Vector3 worldDirection)
    {
        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.001f)
            return Vector2.zero;

        worldDirection.Normalize();
        return new Vector2(worldDirection.x, worldDirection.z);
    }

    public void ClearInputs()
    {
        if (CPUInput == null)
            return;

        CPUInput.SetAxis2DValue("Move", Vector2.zero);
        CPUInput.SetButtonState("Attack", false);
        CPUInput.SetButtonState("Jump", false);
    }
    public void Stun(float duration = -1f)
    {
        stunTimer = duration > 0f ? duration : DefaultStunDuration;

        ClearInputs();

        if (ControlledEntity != null)
        {
            ControlledEntity.Attributes.isAttacking = false;

            if (ControlledEntity.Visual != null)
            {
                ControlledEntity.Visual.SetBool(WindUpBoolName, false);
            }
        }

        Debug.Log($"{name} stunned for {stunTimer} seconds.");
    }
}