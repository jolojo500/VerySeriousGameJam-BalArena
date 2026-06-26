using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("SFX")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float hoverVolume = 0.4f;
    [SerializeField] private float clickVolume = 0.7f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverClip, hoverVolume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickClip, clickVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }
}