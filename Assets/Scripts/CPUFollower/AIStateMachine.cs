using UnityEngine;

public class AIStateMachine : StateMachine<CPUAIState>
{
    [Header("AI")]
    public float HostileDistance = 7f;
    public float AttackDistance = 1.2f;
    public float LoseDistance = 12f;
    public float AttackCooldown = 1f;
    public float SuspicionHostileThreshold = 60f;

    public CPUInputManager CPUInput { get; private set; }
    public BallerinaEntity ControlledEntity;

    private void Awake()
    {
        CPUInput = GetComponent<CPUInputManager>();

        if (CPUInput != null)
        {
            CPUInput.Entity = ControlledEntity;
            CPUInput.AIMachine = this;
        }
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

        Initialize<StandByState>();
    }

    public override void Add(CPUAIState state)
    {
        base.Add(state);
        state.AIMachine = this;
        state.FollowerCPU = CPUInput;
    }

    public void Update()
    {
        if (ControlledEntity != null && ControlledEntity.IsDead)
            return;

        CurrentState?.OnUpdate();
    }
}