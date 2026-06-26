using UnityEngine;

public class AnimationAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Call this from the animation clip event
    public void PlayAudio()
    {
        if (audioSource == null)
        {
            Debug.LogWarning($"{name} has no AudioSource.");
            return;
        }

        audioSource.Play();
    }
}