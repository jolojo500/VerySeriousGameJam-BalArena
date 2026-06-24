using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("Cinemachine")]
    public CinemachineImpulseSource ImpulseSource;

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
}