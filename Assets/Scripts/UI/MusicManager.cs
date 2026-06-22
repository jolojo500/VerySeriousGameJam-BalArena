using UnityEngine;
using System.Collections;
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public float fadeTime = 1f;
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Music")]
    public AudioClip[] music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayActMusic(1);
    }

    public void PlayActMusic(int n)
    {
        StartCoroutine(SwapMusic(music[n], 0.5f));
    }

    IEnumerator SwapMusic(AudioClip newClip, float volume)
    {
        if (audioSource.clip == newClip)
            yield break;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = volume;
    }
}