using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("Cinemachine")]
    public CinemachineImpulseSource ImpulseSource;
    public CinemachineImpulseSource RumbleSource;

    private void Awake()
    {
        Instance = this;

        if (ImpulseSource == null)
            ImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float strength)
    {
        if (ImpulseSource == null)
            return;

        ImpulseSource.GenerateImpulse(strength);
    }
    public void Rumble(float strength)
    {
        if (ImpulseSource == null)
            return;

        RumbleSource.GenerateImpulse(strength);
    }
}