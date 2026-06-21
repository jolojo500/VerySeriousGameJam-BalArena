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
        public PlayerCollision Collision = new PlayerCollision();
        public Collider PlayerCollider;
        public NeoInputManager InputManager;

        // Hit Frame Data
        public bool IsInvincible, IsInIF;
        public float OOCTimer; //Out of control
        public float IFMaxTime = 5.0f, IFTimer;
        public InputLockType InputLockType = InputLockType.None;
        public Camera PlayerCamera;
        public Transform FreeLookCamera;

        [ReadOnly] public string CurrentState = "";
        public float SPINLossRate = 2;
        public PlayerControllers PlayerControllers = new PlayerControllers();

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
            PlayerControllers.Init(this);
            Attributes.MaxHealth = 100;
            Attributes.MaxSpin = 100;
            Attributes.MaxSpotLight = 100;
            Attributes.AddToHealth(Attributes.MaxHealth);
            Attributes.AddToSpin(Attributes.MaxSpin);
            Attributes.AddToSpotLight(0);
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void OnDrawGizmos()
        {
            if (Machine.IsCurrentState<PS_Attack>())
            {
                Gizmos.DrawWireSphere((transform.position + Vector3.up) + Rb.linearVelocity.normalized * 0.3f, 1f);
            }
        }
        public void FixedUpdate()
        {
            PlayerControllers.FixedUpdate();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            PlayerControllers.Update();
            HandleInvulnerability();
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


            if (Attributes.IsInSpotLight)
            {
                Debug.Log("Test");
                Attributes.AddToSpotLight(4f * Time.deltaTime);
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
    }

    [Serializable]
    public class BallerinaAttributes
    {
        public float MaxHealth;
        public float MaxSpin;
        public float MaxSpotLight;

        public float CurrentHealth;

        public float CurrentSpin;

        public float CurrentSpotLight;



        public bool Grounded = false, Damaged = false, IsInSpotLight = false;
        public UnityEvent<Tuple<int, int>> OnHealthChanged = new UnityEvent<Tuple<int, int>>();
        public UnityEvent<Tuple<int, int>> OnSpinChanged = new UnityEvent<Tuple<int, int>>();
        public UnityEvent<Tuple<int, int>> OnSpotLightChanged = new UnityEvent<Tuple<int, int>>();


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
        }

        public void AddToSpotLight(float amount)
        {
            CurrentSpotLight = Mathf.Clamp(CurrentSpotLight + amount, 0, MaxSpotLight);
            OnSpotLightChange((int)CurrentSpotLight, (int)MaxSpotLight);
        }
        public void OnHealthChange(int newHealth, int maxHealth)
        {
            OnHealthChanged?.Invoke(new Tuple<int, int>(newHealth, maxHealth));
        }
        public void OnSpinChange(int newESP, int maxESP)
        {
            OnSpinChanged?.Invoke(new Tuple<int, int>(newESP, maxESP));
        }
        public void OnSpotLightChange(int newESP, int maxESP)
        {
            OnSpotLightChanged?.Invoke(new Tuple<int, int>(newESP, maxESP));
        }

    }
}
