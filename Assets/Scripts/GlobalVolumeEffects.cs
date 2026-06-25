using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class GlobalVolumeEffects : MonoBehaviour
{
    public static GlobalVolumeEffects Instance { get; private set; }

    [System.Serializable]
    public struct VolumeLook
    {
        [Header("Bloom")]
        public float BloomIntensity;
        public float BloomThreshold;

        [Header("Color")]
        public float Saturation;
        public float Contrast;
        public float Temperature;
        public Color ColorFilter;

        [Header("Screen Effects")]
        public float VignetteIntensity;
        public float ChromaticAberrationIntensity;
        public float LensDistortionIntensity;
        public float FilmGrainIntensity;
    }

    [Header("Setup")]
    [SerializeField] private Volume volume;
    public float DefaultTransitionTime = 1f;

    [Header("Act Looks")]
    public VolumeLook Act1Look = new VolumeLook
    {
        BloomIntensity = 0.65f,
        BloomThreshold = 1.1f,
        Saturation = 15f,
        Contrast = 5f,
        Temperature = 15f,
        ColorFilter = Color.white,
        VignetteIntensity = 0.08f,
        ChromaticAberrationIntensity = 0.02f,
        LensDistortionIntensity = 0f,
        FilmGrainIntensity = 0f
    };

    public VolumeLook Act2Look = new VolumeLook
    {
        BloomIntensity = 0.9f,
        BloomThreshold = 1f,
        Saturation = 3f,
        Contrast = 15f,
        Temperature = -5f,
        ColorFilter = Color.white,
        VignetteIntensity = 0.28f,
        ChromaticAberrationIntensity = 0.08f,
        LensDistortionIntensity = -0.03f,
        FilmGrainIntensity = 0.18f
    };

    public VolumeLook Act3Look = new VolumeLook
    {
        BloomIntensity = 1.25f,
        BloomThreshold = 0.9f,
        Saturation = -5f,
        Contrast = 25f,
        Temperature = -15f,
        ColorFilter = new Color(1f, 0.82f, 0.82f),
        VignetteIntensity = 0.42f,
        ChromaticAberrationIntensity = 0.18f,
        LensDistortionIntensity = -0.08f,
        FilmGrainIntensity = 0.35f
    };

    public VolumeLook DeathLook = new VolumeLook
    {
        BloomIntensity = 0.25f,
        BloomThreshold = 1.2f,
        Saturation = -75f,
        Contrast = 35f,
        Temperature = -25f,
        ColorFilter = new Color(0.75f, 0.15f, 0.15f),
        VignetteIntensity = 0.7f,
        ChromaticAberrationIntensity = 0.35f,
        LensDistortionIntensity = -0.25f,
        FilmGrainIntensity = 0.5f
    };

    [Header("Spin Charge Effect")]
    public float SpinBloomBoost = 0.9f;
    public float SpinChromaticBoost = 0.12f;
    public float SpinVignetteBoost = 0.08f;

    [Header("Low Health Effect")]
    public Color LowHealthColor = new Color(1f, 0.35f, 0.35f);
    public float LowHealthColorStrength = 0.35f;
    public float LowHealthVignetteBoost = 0.25f;
    public float LowHealthSaturationLoss = 30f;
    public float LowHealthChromaticBoost = 0.08f;

    [Header("Hit Impact Pulse")]
    public float HitPulseDuration = 0.12f;
    public float HitBloomBoost = 1.0f;
    public float HitChromaticBoost = 0.35f;
    public float HitVignetteBoost = 0.12f;
    public float HitLensDistortionBoost = -0.22f;

    [Header("Player Hurt Pulse")]
    public float HurtPulseDuration = 0.22f;
    public float HurtChromaticBoost = 0.45f;
    public float HurtVignetteBoost = 0.25f;
    public float HurtLensDistortionBoost = -0.18f;
    public Color HurtColor = new Color(1f, 0.25f, 0.25f);

    [Header("Enemy Wind-Up Warning")]
    public float WindUpPulseDuration = 0.35f;
    public float WindUpVignetteBoost = 0.15f;
    public float WindUpChromaticBoost = 0.1f;
    public Color WindUpColor = new Color(1f, 0.45f, 0.45f);

    private Bloom bloom;
    private ColorAdjustments colorAdjustments;
    private WhiteBalance whiteBalance;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private FilmGrain filmGrain;

    private VolumeLook currentBaseLook;
    private int currentAct = 1;

    private float spinCharge01;
    private float lowHealthAmount01;

    private Coroutine transitionRoutine;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetupVolume();
    }

    private void SetupVolume()
    {
        if (volume == null)
            volume = GetComponent<Volume>();

        volume.isGlobal = true;

        // Important: this prevents editing the original Volume Profile asset permanently.
        if (volume.profile != null)
            volume.profile = Instantiate(volume.profile);
        else
            volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

        bloom = GetOrAdd<Bloom>();
        colorAdjustments = GetOrAdd<ColorAdjustments>();
        whiteBalance = GetOrAdd<WhiteBalance>();
        vignette = GetOrAdd<Vignette>();
        chromaticAberration = GetOrAdd<ChromaticAberration>();
        lensDistortion = GetOrAdd<LensDistortion>();
        filmGrain = GetOrAdd<FilmGrain>();

        EnableOverrides();
    }

    private T GetOrAdd<T>() where T : VolumeComponent
    {
        if (!volume.profile.TryGet(out T component))
            component = volume.profile.Add<T>(true);

        return component;
    }

    private void EnableOverrides()
    {
        bloom.intensity.overrideState = true;
        bloom.threshold.overrideState = true;

        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.contrast.overrideState = true;
        colorAdjustments.colorFilter.overrideState = true;

        whiteBalance.temperature.overrideState = true;

        vignette.intensity.overrideState = true;

        chromaticAberration.intensity.overrideState = true;

        lensDistortion.intensity.overrideState = true;

        filmGrain.intensity.overrideState = true;
    }

    // --------------------------------------------------------------------
    // Main act functions
    // --------------------------------------------------------------------

    public void PlayAct(int act, float transitionTime = -1f)
    {
        currentAct = act;
        switch (act)
        {
            case 0:
                SetBaseLook(Act1Look, transitionTime);
                break;
            case 1:
                SetBaseLook(Act2Look, transitionTime);
                break;
            case 2:
                SetBaseLook(Act3Look, transitionTime);
                break;
        }
        
    }

    public void PlayDeath(float transitionTime = 0.25f)
    {
        SetBaseLook(DeathLook, transitionTime);
    }

    public void ResetToCurrentAct(float transitionTime = -1f)
    {
        PlayAct(currentAct,transitionTime);
    }

    // --------------------------------------------------------------------
    // Combat / juice functions
    // --------------------------------------------------------------------

    public void PlayHitImpact()
    {
        VolumeLook boost = new VolumeLook
        {
            BloomIntensity = HitBloomBoost,
            ChromaticAberrationIntensity = HitChromaticBoost,
            VignetteIntensity = HitVignetteBoost,
            LensDistortionIntensity = HitLensDistortionBoost,
            ColorFilter = Color.white
        };

        PlayPulse(boost, HitPulseDuration, 0f);
    }

    public void PlayPlayerHurt()
    {
        VolumeLook boost = new VolumeLook
        {
            ChromaticAberrationIntensity = HurtChromaticBoost,
            VignetteIntensity = HurtVignetteBoost,
            LensDistortionIntensity = HurtLensDistortionBoost,
            ColorFilter = HurtColor
        };

        PlayPulse(boost, HurtPulseDuration, 0.45f);
    }

    public void PlayEnemyWindUpWarning()
    {
        VolumeLook boost = new VolumeLook
        {
            ChromaticAberrationIntensity = WindUpChromaticBoost,
            VignetteIntensity = WindUpVignetteBoost,
            ColorFilter = WindUpColor
        };

        PlayPulse(boost, WindUpPulseDuration, 0.25f);
    }

    public void SetSpinCharge(float charge01)
    {
        spinCharge01 = Mathf.Clamp01(charge01);
        RefreshLook();
    }

    public void ClearSpinCharge()
    {
        spinCharge01 = 0f;
        RefreshLook();
    }

    /// <summary>
    /// 0 = normal health look.
    /// 1 = very low health look.
    /// </summary>
    public void SetLowHealthAmount(float amount01)
    {
        lowHealthAmount01 = Mathf.Clamp01(amount01);
        RefreshLook();
    }

    public void ClearLowHealth()
    {
        lowHealthAmount01 = 0f;
        RefreshLook();
    }

    // --------------------------------------------------------------------
    // Internal logic
    // --------------------------------------------------------------------

    private void SetBaseLook(VolumeLook targetLook, float transitionTime)
    {
        if (transitionTime < 0f)
            transitionTime = DefaultTransitionTime;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionToLook(targetLook, transitionTime));
    }

    private IEnumerator TransitionToLook(VolumeLook targetLook, float duration)
    {
        VolumeLook startLook = currentBaseLook;

        if (duration <= 0f)
        {
            currentBaseLook = targetLook;
            RefreshLook();
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = Smooth01(t);

            currentBaseLook = LerpLook(startLook, targetLook, t);
            RefreshLook();

            yield return null;
        }

        currentBaseLook = targetLook;
        RefreshLook();
    }

    private void RefreshLook()
    {
        VolumeLook finalLook = BuildModifiedLook(currentBaseLook);
        ApplyLook(finalLook);
    }

    private VolumeLook BuildModifiedLook(VolumeLook look)
    {
        // Spin charge modifier
        look.BloomIntensity += SpinBloomBoost * spinCharge01;
        look.ChromaticAberrationIntensity += SpinChromaticBoost * spinCharge01;
        look.VignetteIntensity += SpinVignetteBoost * spinCharge01;

        // Low health modifier
        look.VignetteIntensity += LowHealthVignetteBoost * lowHealthAmount01;
        look.Saturation -= LowHealthSaturationLoss * lowHealthAmount01;
        look.ChromaticAberrationIntensity += LowHealthChromaticBoost * lowHealthAmount01;
        look.ColorFilter = Color.Lerp(
            look.ColorFilter,
            LowHealthColor,
            LowHealthColorStrength * lowHealthAmount01
        );

        ClampLook(ref look);

        return look;
    }

    private void ApplyLook(VolumeLook look)
    {
        ClampLook(ref look);

        bloom.intensity.value = look.BloomIntensity;
        bloom.threshold.value = look.BloomThreshold;

        colorAdjustments.saturation.value = look.Saturation;
        colorAdjustments.contrast.value = look.Contrast;
        colorAdjustments.colorFilter.value = look.ColorFilter;

        whiteBalance.temperature.value = look.Temperature;

        vignette.intensity.value = look.VignetteIntensity;

        chromaticAberration.intensity.value = look.ChromaticAberrationIntensity;

        lensDistortion.intensity.value = look.LensDistortionIntensity;

        filmGrain.intensity.value = look.FilmGrainIntensity;
    }

    private void PlayPulse(VolumeLook boost, float duration, float colorStrength)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine(boost, duration, colorStrength));
    }

    private IEnumerator PulseRoutine(VolumeLook boost, float duration, float colorStrength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            // Goes 0 -> 1 -> 0
            float pulseAmount = 1f - Mathf.Abs((t * 2f) - 1f);
            pulseAmount = Smooth01(pulseAmount);

            VolumeLook baseLook = BuildModifiedLook(currentBaseLook);
            VolumeLook pulsedLook = baseLook;

            pulsedLook.BloomIntensity += boost.BloomIntensity * pulseAmount;
            pulsedLook.ChromaticAberrationIntensity += boost.ChromaticAberrationIntensity * pulseAmount;
            pulsedLook.VignetteIntensity += boost.VignetteIntensity * pulseAmount;
            pulsedLook.LensDistortionIntensity += boost.LensDistortionIntensity * pulseAmount;
            pulsedLook.FilmGrainIntensity += boost.FilmGrainIntensity * pulseAmount;

            if (colorStrength > 0f)
            {
                pulsedLook.ColorFilter = Color.Lerp(
                    baseLook.ColorFilter,
                    boost.ColorFilter,
                    colorStrength * pulseAmount
                );
            }

            ApplyLook(pulsedLook);

            yield return null;
        }

        RefreshLook();
    }

    private VolumeLook LerpLook(VolumeLook a, VolumeLook b, float t)
    {
        return new VolumeLook
        {
            BloomIntensity = Mathf.Lerp(a.BloomIntensity, b.BloomIntensity, t),
            BloomThreshold = Mathf.Lerp(a.BloomThreshold, b.BloomThreshold, t),

            Saturation = Mathf.Lerp(a.Saturation, b.Saturation, t),
            Contrast = Mathf.Lerp(a.Contrast, b.Contrast, t),
            Temperature = Mathf.Lerp(a.Temperature, b.Temperature, t),
            ColorFilter = Color.Lerp(a.ColorFilter, b.ColorFilter, t),

            VignetteIntensity = Mathf.Lerp(a.VignetteIntensity, b.VignetteIntensity, t),
            ChromaticAberrationIntensity = Mathf.Lerp(a.ChromaticAberrationIntensity, b.ChromaticAberrationIntensity, t),
            LensDistortionIntensity = Mathf.Lerp(a.LensDistortionIntensity, b.LensDistortionIntensity, t),
            FilmGrainIntensity = Mathf.Lerp(a.FilmGrainIntensity, b.FilmGrainIntensity, t)
        };
    }

    private void ClampLook(ref VolumeLook look)
    {
        look.BloomIntensity = Mathf.Max(0f, look.BloomIntensity);
        look.BloomThreshold = Mathf.Max(0f, look.BloomThreshold);

        look.Saturation = Mathf.Clamp(look.Saturation, -100f, 100f);
        look.Contrast = Mathf.Clamp(look.Contrast, -100f, 100f);
        look.Temperature = Mathf.Clamp(look.Temperature, -100f, 100f);

        look.VignetteIntensity = Mathf.Clamp01(look.VignetteIntensity);
        look.ChromaticAberrationIntensity = Mathf.Clamp01(look.ChromaticAberrationIntensity);
        look.LensDistortionIntensity = Mathf.Clamp(look.LensDistortionIntensity, -1f, 1f);
        look.FilmGrainIntensity = Mathf.Clamp01(look.FilmGrainIntensity);
    }

    private float Smooth01(float t)
    {
        return t * t * (3f - 2f * t);
    }
}