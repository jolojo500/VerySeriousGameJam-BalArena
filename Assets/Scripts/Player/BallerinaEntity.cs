using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;
public enum EntityTeam
{
    Player,
    Enemy
}
public class BallerinaEntity : Entity
{

    public bool IsDead { get; private set; }

    [Header("Death")]
    public string DeathBoolName = "Death";
    public Collider[] CollidersToDisableOnDeath;
    public bool FreezeRigidbodyOnDeath = true;
    public bool IsInvulnerable => _invulnerabilityTimer > 0f;

    public float InputDisableTimer = 0f;

    private float _invulnerabilityTimer = 0f;

    public bool DefinitiveInputLock = false;

    public float SpinKickCost = 25f;      // energy spent each time you kick

    [ReadOnly] public string CurrentState = "";

    // Hit Frame Data
    public bool IsInvincible, IsInIF;
    public float OOCTimer; //Out of control
    public float IFMaxTime = 5.0f, IFTimer;
    public InputLockType InputLockType = InputLockType.None;
    public EntityTeam Team;

    public PhysicsInfo PhysicsInfo;
    public BallerinaAttributes Attributes = new BallerinaAttributes();


    public EntityCollision Collision = new EntityCollision();
    public Collider PlayerCollider;
    public NeoInputManager InputManager;
    public BallerinaVisual Visual;
    public BallerinaStateMachine Machine;

    [Header("Hit Launch")]
    public float HitLaunchHorizontalMultiplier = 1f;
    public float HitLaunchUpMultiplier = 0.35f;
    public float MaxHitLaunchSpeed = 15f;

    public Controllers<BallerinaEntity> Controllers = new Controllers<BallerinaEntity>();

    [Header("Death Arc Motion")]
    private bool UseDeathArcMotion = true;
    private float DeathArcDuration = 0.6f;
    private bool RandomizeDeathArcX = true;
    private float DeathArcRandomXRange = 5f;
    private float DeathArcTargetZOffset = -10f;
    private float DeathArcTargetY = -10f;
    private float DeathArcUpAmount = 2.5f;
    private bool MakeRigidbodyKinematicOnDeath = true;
    private AIStateMachine ai;
    public override void Init()
    {
        base.Init(); // Important: initialize Rb first

        Collision.BallerinaEntity = this;

        if (InputManager == null)
            InputManager = GetComponent<NeoInputManager>();

        Visual = GetComponentInChildren<BallerinaVisual>();
        if (Visual != null)
            Visual.Entity = this;

        Machine = GetComponent<BallerinaStateMachine>();
        if (Machine != null)
            Machine.Init();
        ai = GetComponent<AIStateMachine>();
        Attributes.MaxSpin = 100;
        Attributes.MaxSuspicion = 100;

        Attributes.AddToHealth(Attributes.MaxHealth);
        Attributes.AddToSpin(0.0f);
        Attributes.AddToSuspicion(0);

        Controllers.AddController(new ComboController());
        Controllers.Init(this);
    }

    public void ToggleInvulnerability(float v)
    {
        _invulnerabilityTimer = v;
    }

    public void HandleInvulnerability()
    {
        HandleTimer(ref _invulnerabilityTimer);
        if (!DefinitiveInputLock)
        {
            if (HandleTimer(ref InputDisableTimer))
            {
                ObjectEnableInput();
            }
        }
    }

    protected override void Update()
    {
        if (IsDead)
        {
            CurrentState = "Dead";
            return;
        }

        CurrentState = Machine?.CurrentState?.GetType().Name ?? "";

        if (IsInIF)
        {
            if (HandleTimer(ref IFTimer))
            {
                IsInIF = false;
            }
        }

        if (HandleTimer(ref OOCTimer))
        {
            UnlockInputs();
        }

        HandleInvulnerability();
        Controllers.Update();
    }

    public void FixedUpdate()
    {
        if (IsDead)
            return;
        Controllers.FixedUpdate();
    }
    public void BlockInput(StageObject stageObject)
    {
        OOCTimer = stageObject.OutOfControlTime;
        InputLockType = stageObject.InputLockType;
        InputManager.BlockInput = true;
    }
    public void OnObject(StageObject Obj, bool ToAir)
    {
        BlockInput(Obj);
        if (ToAir)
        {
            Machine.Set<PS_Air>();
        }
    }



    public void TriggerDamage(int damage)
    {
        if (IsDead || IsInvincible || IsInIF)
            return;

        int finalDamage = Mathf.Max(1, Mathf.Abs(damage));

        Attributes.AddToHealth(-finalDamage);

        Debug.Log($"{name} took {finalDamage} damage. HP: {Attributes.CurrentHealth}/{Attributes.MaxHealth}");

        if (Attributes.CurrentHealth <= 0)
        {
            TriggerDeath();
            return;
        }

        Invulnerable();
        Machine?.Set<PS_Damaged>();
    }

    public virtual void OnHit(HitInfo hitInfo)
    {
        
        BallerinaEntity attacker = hitInfo.SourceEntity;

        if (attacker != null && attacker.Team == Team)
            return;
        if (Team == EntityTeam.Player)
        {
            GlobalVolumeEffects.Instance.PlayPlayerHurt();
        }
        else
        {
            GlobalVolumeEffects.Instance.PlayHitImpact();
        }
        Instantiate(ParticleFx, transform.position, Quaternion.identity);

        if (ai == null)
            ai = GetComponentInChildren<AIStateMachine>();

        if (ai == null)
            ai = GetComponentInParent<AIStateMachine>();

        if (ai != null)
            ai.AlertFromHit();

        if (IsDead || IsInvincible || IsInIF)
            return;

        PS_Damaged damagedState = Machine?.Get<PS_Damaged>();
        if (damagedState != null)
            damagedState.info = hitInfo;

        ApplyHitLaunch(hitInfo);

        TriggerDamage(hitInfo.Damage);
    }

    public void TriggerDeath()
    {
        if (IsDead)
            return;
        if (!(this is Player))
        {
            WaveManager.Instance.EnemyKilled();
        }
        else
        {
            GameManager.Instance.EndGame();
        }
        IsDead = true;
        SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.Hit, gameObject.transform);
        SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.Scream, gameObject.transform);
        IsInIF = false;
        IFTimer = 0f;
        ToggleInvulnerability(0f);

        DefinitiveInputLock = true;

        if (InputManager != null)
            InputManager.BlockInput = true;

        CPUInputManager cpu = GetComponent<CPUInputManager>();
        if (cpu != null)
        {
            cpu.SetAxis2DValue("Move", Vector2.zero);
            cpu.SetButtonState("Attack", false);
            cpu.enabled = false;
        }

        AIStateMachine ai = GetComponent<AIStateMachine>();
        if (ai != null)
            ai.enabled = false;

        if (PlayerCollider != null)
            PlayerCollider.enabled = false;

        foreach (Collider col in CollidersToDisableOnDeath)
        {
            if (col != null)
                col.enabled = false;
        }

        if (Rb != null && FreezeRigidbodyOnDeath)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }
        Machine.enabled = false;
        Attributes.Damaged = false;
        Visual?.SetBool(DeathBoolName,true);
        
        if (UseDeathArcMotion)
        {
            StartCoroutine(DeathArcRoutine());
        }
        Debug.Log($"{name} died.");
    }

    private void Invulnerable()
    {
        SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.Hit, gameObject.transform);
        IFTimer = IFMaxTime;
        IsInIF = true;
        ToggleInvulnerability(IFMaxTime);
        HandleInvulnerability();
    }

    public void UnlockInputs()
    {
        InputLockType = InputLockType.None;
        InputManager.BlockInput = false;
    }


    public void SetDefinitiveInputLock(bool inputLocked)
    {
        DefinitiveInputLock = inputLocked;
        //LockInput
    }

    public void ObjectEnableInput()
    {

        InputDisableTimer = 0;
        //UnlockInput
    }


    public void ObjectDisableInput(float time)
    {

        InputDisableTimer = time;
        //LockInput
    }
    private void ApplyHitLaunch(HitInfo hitInfo)
    {
        if (Rb == null)
            return;

        Vector3 launchDirection = transform.position - hitInfo.SourcePosition;
        launchDirection.y = 0f;

        if (launchDirection.sqrMagnitude < 0.001f)
            launchDirection = -transform.forward;

        launchDirection.Normalize();

        float force = hitInfo.KnockbackForce;

        Vector3 launchVelocity =
            launchDirection * force * HitLaunchHorizontalMultiplier +
            Vector3.up * force * HitLaunchUpMultiplier;

        Rb.linearVelocity = Vector3.ClampMagnitude(
            Rb.linearVelocity + launchVelocity,
            MaxHitLaunchSpeed
        );
    }
    private IEnumerator DeathArcRoutine()
    {
        if (Rb != null && MakeRigidbodyKinematicOnDeath)
        {
            Rb.isKinematic = true;
        }

        Vector3 startPosition = transform.position;

        float randomXOffset = RandomizeDeathArcX
    ? Random.Range(-DeathArcRandomXRange, DeathArcRandomXRange)
    : 0f;

        Vector3 endPosition = new Vector3(
            startPosition.x + randomXOffset,
            DeathArcTargetY,
            startPosition.z + DeathArcTargetZOffset
        );

        float timer = 0f;

        while (timer < DeathArcDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / DeathArcDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 position = Vector3.Lerp(startPosition, endPosition, smoothT);

            // This makes it go slightly up first, then fall down to Y = -10.
            float upwardArc = Mathf.Sin(t * Mathf.PI) * DeathArcUpAmount;
            position.y += upwardArc;

            transform.position = position;

            yield return null;
        }

        transform.position = endPosition;
        Visual.selfDestruct();
    }
}
