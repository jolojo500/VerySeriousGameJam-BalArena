using NaughtyAttributes;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Venice
{
    public class Player : BallerinaEntity
    {

        public static Player Instance { get; private set; }

        public Camera PlayerCamera;
        public Transform FreeLookCamera;

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
        }



        public void OnDrawGizmos()
        {
            if (Machine?.IsCurrentState<PS_Attack>() ?? false)
            {
                Gizmos.DrawWireSphere((transform.position + Vector3.up) + Rb.linearVelocity.normalized * 0.3f, 1f);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
       
    }

    [Serializable]
    public class BallerinaAttributes
    {
        public float MaxHealth;
        public float MaxSpin;
        public float MaxSuspicion;

        public float CurrentHealth;

        public float CurrentSpin;

        public float CurrentSuspicion;



        public bool Grounded = false, Damaged = false, IsInSpotLight = false;
        public UnityEvent<Tuple<int, int>> OnHealthChanged = new UnityEvent<Tuple<int, int>>();
        public UnityEvent<Tuple<int, int>> OnSpinChanged = new UnityEvent<Tuple<int, int>>();
        public UnityEvent<Tuple<int, int>> OnSuspicionChanged = new UnityEvent<Tuple<int, int>>();


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

        public void AddToSuspicion(float amount)
        {
            CurrentSuspicion = Mathf.Clamp(CurrentSuspicion + amount, 0, MaxSuspicion);
            OnSuspicionChange((int)CurrentSuspicion, (int)MaxSuspicion);
        }
        public void OnHealthChange(int newHealth, int maxHealth)
        {
            OnHealthChanged?.Invoke(new Tuple<int, int>(newHealth, maxHealth));
        }
        public void OnSpinChange(int newESP, int maxESP)
        {
            OnSpinChanged?.Invoke(new Tuple<int, int>(newESP, maxESP));
        }
        public void OnSuspicionChange(int newSus, int maxSus)
        {
            OnSuspicionChanged?.Invoke(new Tuple<int, int>(newSus, maxSus));
        }

    }
}
