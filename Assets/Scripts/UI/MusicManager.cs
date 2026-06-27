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

    private Coroutine musicCoroutine;
    public float targetVolume = 0.3f;

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

    public void PlayActMusic(int n)
    {
        if (n < 0 || n >= music.Length)
        {
            Debug.LogWarning($"Music index {n} is out of range.");
            return;
        }

        PlayMusic(music[n], targetVolume);
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null)
            return;

        targetVolume = volume;

        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }

        musicCoroutine = StartCoroutine(SwapMusic(clip, volume));
    }

    public void StopAllMusic()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }

        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 0f;
    }

    public void StopAllMusicWithFade()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }

        musicCoroutine = StartCoroutine(FadeOutAndStop());
    }

    IEnumerator SwapMusic(AudioClip newClip, float volume)
    {
        // If the same clip is already assigned but faded/stopped, revive it.
        if (audioSource.clip == newClip)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            while (audioSource.volume < volume)
            {
                audioSource.volume += Time.unscaledDeltaTime / fadeTime;
                yield return null;
            }

            audioSource.volume = volume;
            musicCoroutine = null;
            yield break;
        }

        // Fade out current music
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 0f;

        // Swap clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music
        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = volume;
        musicCoroutine = null;
    }

    IEnumerator FadeOutAndStop()
    {
        if (audioSource == null)
            yield break;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.clip = null;

        musicCoroutine = null;
    }
}