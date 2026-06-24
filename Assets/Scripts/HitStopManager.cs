using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    [Header("Hit Stop")]
    public float DefaultFreezeTime = 0.06f;
    public float FrozenTimeScale = 0.05f;

    private float originalFixedDeltaTime;
    private Coroutine hitStopRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void HitStop()
    {
        HitStop(DefaultFreezeTime);
    }

    public void HitStop(float duration)
    {
        if (hitStopRoutine != null)
            StopCoroutine(hitStopRoutine);

        hitStopRoutine = StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = FrozenTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        CameraShakeManager.Instance?.Shake(0.2f);
        hitStopRoutine = null;
    }
}