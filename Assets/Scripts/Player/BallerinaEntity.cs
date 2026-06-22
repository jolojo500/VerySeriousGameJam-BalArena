using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;

public class BallerinaEntity : Entity
{
    

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


    public PhysicsInfo PhysicsInfo;
    public BallerinaAttributes Attributes = new BallerinaAttributes();


    public EntityCollision Collision = new EntityCollision();
    public Collider PlayerCollider;
    public NeoInputManager InputManager;
    public BallerinaVisual Visual;
    public BallerinaStateMachine Machine;


    public override void Init()
    {
        Collision.BallerinaEntity = this;
        Visual = GetComponentInChildren<BallerinaVisual>();
        Machine = GetComponent<BallerinaStateMachine>();
        Machine.Init();
        Attributes.MaxHealth = 100;
        Attributes.MaxSpin = 100;
        Attributes.MaxSuspicion = 100;
        Attributes.AddToHealth(Attributes.MaxHealth);
        Attributes.AddToSpin(Attributes.MaxSpin);
        Attributes.AddToSuspicion(0);
        base.Init();
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
        CurrentState = Machine.CurrentState?.GetType().Name ?? "";

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
        //Set Damage state if health > 0 else TriggerDeath();
        if (!IsInIF)
        {
            Attributes.AddToHealth(-Mathf.Abs(damage));
            if (Attributes.CurrentHealth > 0)
            {
                Invulnerable();
                Machine.Set<PS_Damaged>();
            }
            else
            {
                TriggerDeath();
            }
        }
    }

    public void OnHit(HitInfo hitInfo)
    {

        Machine.Get<PS_Damaged>().info = hitInfo;
        Machine.Set<PS_Damaged>();
    }

    private void TriggerDeath()
    {
        Debug.Log("Oh no");
    }

    private void Invulnerable()
    {
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
}
