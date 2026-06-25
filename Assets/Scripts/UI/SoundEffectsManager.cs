using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    // Can call from any script
    // SoundEffectsManager.Instance.PlaySoundFXClip(SoundEffectsManager.soundEffects.sfxname , gameObject.transform);
    public static SoundEffectsManager Instance;
    [SerializeField] private AudioSource m_AudioSource;
    public enum soundEffects
    {
        Spin,
        Hit,
        Scream
    }
    public float volume = 0.1f;
    public AudioClip[] audioClips;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySoundFXClip(soundEffects effect, Transform spawnTransform)
    {
        AudioSource source = Instantiate(m_AudioSource, spawnTransform.position, Quaternion.identity);
        source.clip = audioClips[(int)effect];
        source.volume = volume;
        source.Play();
        float clipLength = source.clip.length;
        Destroy(source.gameObject, clipLength);
    }
}
