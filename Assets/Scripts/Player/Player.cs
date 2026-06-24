using NaughtyAttributes;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Profiling.RawFrameDataView;

namespace Venice
{
    public class Player : BallerinaEntity
    {

        public static Player Instance { get; private set; }

        public Camera PlayerCamera;
        public Transform FreeLookCamera;

        public float SPINLossRate = 2;
        [Header("Player Kick Energy")]
        public PlayerKickEnergyTiers KickEnergyTiers = new PlayerKickEnergyTiers();

        [Header("Player Kick Feedback")]
        public AudioSource KickImpactAudioSource;
        public AudioClip KickImpactClip;
        public FloatEvent OnKickScreenShake = new FloatEvent();

        public void PlayKickImpactFeedback(PlayerKickTier tier)
        {
            OnKickScreenShake?.Invoke(tier.ScreenShakeStrength);

            if (KickImpactAudioSource != null && KickImpactClip != null)
            {
                KickImpactAudioSource.pitch = tier.ImpactSoundPitch;
                KickImpactAudioSource.PlayOneShot(KickImpactClip, tier.ImpactSoundVolume);
            }
        }
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
        
        public float SpinDuration = 0.65f;
        public float SpinCooldown = 0.75f;
        public float SpinUpwardBurstSpeed = 8.5f;
        public float SpinChargePerSecond = 35f;
        public float NextSpinAllowedTime;
        public bool CanStartSpin()
        {
            return !isSpin && Time.time >= NextSpinAllowedTime;
        }

        public void StartSpinCooldown()
        {
            NextSpinAllowedTime = Time.time + SpinCooldown;
        }

        public bool Grounded = false, Damaged = false, IsInSpotLight = false, isAttacking = false, isSpin = false;
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
    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }

    [Serializable]
    public class PlayerKickEnergyTiers
    {
        public bool ConsumeAllEnergyOnKick = false;

        [Header("Energy Cost")]
        public float MinimumEnergyToKick = 25f;
        public float EnergySpentPerKick = 25f;

        [Header("25% Kick")]
        public PlayerKickTier Tier25 = new PlayerKickTier
        {
            Damage = 1,
            KnockbackForce = 2f,
            HitFreezeLength = 0.06f,
            ScreenShakeStrength = 0.35f,
            KickLungeSpeed = 8f,
            AttackDurationMultiplier = 0.85f,
            ImpactSoundPitch = 0.9f,
            ImpactSoundVolume = 0.6f,
            EnemyLaunchHeight = 1.5f
        };

        [Header("50% Kick")]
        public PlayerKickTier Tier50 = new PlayerKickTier
        {
            Damage = 1,
            KnockbackForce = 4f,
            HitFreezeLength = 0.09f,
            ScreenShakeStrength = 0.6f,
            KickLungeSpeed = 10f,
            AttackDurationMultiplier = 1f,
            ImpactSoundPitch = 1f,
            ImpactSoundVolume = 0.75f,
            EnemyLaunchHeight = 2.5f
        };

        [Header("75% Kick")]
        public PlayerKickTier Tier75 = new PlayerKickTier
        {
            Damage = 2,
            KnockbackForce = 6f,
            HitFreezeLength = 0.12f,
            ScreenShakeStrength = 0.85f,
            KickLungeSpeed = 12f,
            AttackDurationMultiplier = 1.1f,
            ImpactSoundPitch = 1.08f,
            ImpactSoundVolume = 0.9f,
            EnemyLaunchHeight = 3.5f
        };

        [Header("100% Kick")]
        public PlayerKickTier Tier100 = new PlayerKickTier
        {
            Damage = 3,
            KnockbackForce = 8f,
            HitFreezeLength = 0.16f,
            ScreenShakeStrength = 1.15f,
            KickLungeSpeed = 14f,
            AttackDurationMultiplier = 1.25f,
            ImpactSoundPitch = 1.18f,
            ImpactSoundVolume = 1f,
            EnemyLaunchHeight = 4.5f
        };

        public bool TryGetTier(float currentEnergy, float maxEnergy, out PlayerKickTier tier)
        {
            tier = Tier25;

            if (maxEnergy <= 0f)
                return false;

            float percent = Mathf.Clamp01(currentEnergy / maxEnergy) * 100f;

            if (percent < 25f)
                return false;

            int clampedPercent = Mathf.FloorToInt(percent / 25f) * 25;
            clampedPercent = Mathf.Clamp(clampedPercent, 25, 100);

            switch (clampedPercent)
            {
                case 25:
                    tier = Tier25;
                    break;

                case 50:
                    tier = Tier50;
                    break;

                case 75:
                    tier = Tier75;
                    break;

                case 100:
                    tier = Tier100;
                    break;
            }

            return true;
        }

        public float GetEnergyCost(float currentEnergy)
        {
            if (ConsumeAllEnergyOnKick)
                return currentEnergy;

            return EnergySpentPerKick;
        }
    }

    [Serializable]
    public struct PlayerKickTier
    {
        public int Damage;
        public float KnockbackForce;
        public float HitFreezeLength;
        public float ScreenShakeStrength;
        public float KickLungeSpeed;
        public float AttackDurationMultiplier;
        public float ImpactSoundPitch;
        public float ImpactSoundVolume;
        public float EnemyLaunchHeight;
    }
}
