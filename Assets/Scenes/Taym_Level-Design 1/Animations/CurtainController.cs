using UnityEngine;
using System.Collections;

public class DoubleCurtainController : MonoBehaviour
{
    [Header("Curtain Objects")]
    public Transform leftCurtain;
    public Transform rightCurtain;

    [Header("X Positions")]
    public float leftClosedX = -1f;
    public float rightClosedX = 1f;

    public float leftOpenX = -6f;
    public float rightOpenX = 6f;

    [Header("Timing")]
    public float openDuration = 6f;
    public float closeDuration = 6f;

    [Header("State")]
    public bool isOpen = false;

    private bool isAnimating = false;

    void Start()
    {
        SetCurtainX(leftCurtain, leftClosedX);
        SetCurtainX(rightCurtain, rightClosedX);

        isOpen = false;
    }

    public void ToggleCurtain()
    {
        if (isAnimating)
            return;

        if (isOpen)
            StartCoroutine(PlayClose());
        else
            StartCoroutine(PlayOpen());
    }

    public IEnumerator PlayOpen()
    {
        if (isAnimating)
            yield break;

        isAnimating = true;

        if (SoundEffectsManager.Instance != null)
        {
            SoundEffectsManager.Instance.PlaySoundFXClip(
                SoundEffectsManager.soundEffects.CurtainOpen,
                gameObject.transform
            );
        }

        float leftStartX = leftCurtain.position.x;
        float rightStartX = rightCurtain.position.x;

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / openDuration);
            t = SmoothEase(t);

            SetCurtainX(leftCurtain, Mathf.Lerp(leftStartX, leftOpenX, t));
            SetCurtainX(rightCurtain, Mathf.Lerp(rightStartX, rightOpenX, t));

            yield return null;
        }

        SetCurtainX(leftCurtain, leftOpenX);
        SetCurtainX(rightCurtain, rightOpenX);

        isOpen = true;
        isAnimating = false;
    }

    public IEnumerator PlayClose()
    {
        if (isAnimating)
            yield break;

        isAnimating = true;

        if (SoundEffectsManager.Instance != null)
        {
            SoundEffectsManager.Instance.PlaySoundFXClip(
                SoundEffectsManager.soundEffects.CurtainClose,
                gameObject.transform
            );
        }

        float leftStartX = leftCurtain.position.x;
        float rightStartX = rightCurtain.position.x;

        float elapsed = 0f;

        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / closeDuration);
            t = SmoothEase(t);

            SetCurtainX(leftCurtain, Mathf.Lerp(leftStartX, leftClosedX, t));
            SetCurtainX(rightCurtain, Mathf.Lerp(rightStartX, rightClosedX, t));

            yield return null;
        }

        SetCurtainX(leftCurtain, leftClosedX);
        SetCurtainX(rightCurtain, rightClosedX);

        isOpen = false;
        isAnimating = false;
    }

    private void SetCurtainX(Transform curtain, float x)
    {
        if (curtain == null)
            return;

        Vector3 pos = curtain.position;
        pos.x = x;
        curtain.position = pos;
    }

    private float SmoothEase(float t)
    {
        return t * t * (3f - 2f * t);
    }
}