using NaughtyAttributes;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Venice
{
    public class Player : PlayerEntity
    {

        public static Player Instance { get; private set; }
        public PlayerStateMachine Machine;
        public PlayerControllers Controllers = new PlayerControllers();
        public PlayerCollision Collision = new PlayerCollision();
        public Collider PlayerCollider;
        public NeoInputManager InputManager;
        public BallerinaAttributes Attributes = new BallerinaAttributes();

        // Hit Frame Data
        public bool IsInvincible, IsInIF;
        public float OOCTimer; //Out of control
        public float IFMaxTime = 5.0f, IFTimer;
        public InputLockType InputLockType = InputLockType.None;
        public Camera PlayerCamera;
        public Transform FreeLookCamera;

        [ReadOnly] public string CurrentState = "";
        public float SPINLossRate = 2;

        private void Awake()
        {
            Instance = this;
            PlayerCamera.transform.parent = null;
            FreeLookCamera.transform.parent = null;
        }

        public override void Init()
        {
            base.Init();
            Visual = GetComponentInChildren<PlayerVisual>();
            Machine = GetComponent<PlayerStateMachine>();
            Machine.Init();
            Controllers.Init(this);
            Attributes.MaxHealth = 100;
            Attributes.MaxSpin = 100;
            Attributes.AddToHealth(Attributes.MaxHealth);
            Attributes.AddToSpin(Attributes.MaxSpin);
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void FixedUpdate()
        {
            Controllers.FixedUpdate();
        }

        // Update is called once per frame
        void Update()
        {
           
            Controllers.Update();
            HandleInvulnerability();
            CurrentState = Machine.CurrentState?.GetType().Name ?? "";

            if (IsInIF)
            {
                if (HandleTimer(ref IFTimer))
                {
                    IsInIF = false;
                }
            }

            if(HandleTimer(ref OOCTimer))
            {
                UnlockInputs();
            }
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
    }

    [Serializable]
    public class BallerinaAttributes
    {
        public float MaxHealth;
        public float MaxSpin;
        public float CurrentHealth
        {
            get;
            private set;
        }
        public float CurrentSpin
        {
            get;
            private set;
        }


        public bool Grounded = false, Damaged = false;
        public UnityEvent<Tuple<int, int>> OnHealthChanged= new UnityEvent<Tuple<int, int>>();
        public UnityEvent<Tuple<int, int>> OnSpinChanged = new UnityEvent<Tuple<int, int>>();

        public BallerinaAttributes()
        {
        }

        public void AddToHealth(float amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            OnHealthChange((int)CurrentHealth, (int)MaxHealth);
        }
        public void AddToSpin(float amount)
        {
            CurrentSpin = Mathf.Clamp(CurrentSpin + amount, 0, MaxSpin);
            OnSpinChange((int)CurrentSpin, (int)MaxSpin);
            Debug.Log(CurrentSpin);
        }

        public void OnHealthChange(int newHealth, int maxHealth)
        {
            OnHealthChanged?.Invoke( new Tuple<int, int>(newHealth, maxHealth));
        }
        public void OnSpinChange(int newESP, int maxESP)
        {
            OnSpinChanged?.Invoke( new Tuple<int, int>(newESP, maxESP));
        }

    }
}
