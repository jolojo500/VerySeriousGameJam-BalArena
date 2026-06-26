using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static int gameVolume = 1;
    public static bool cameraShake = true;
    public static SceneLoader Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    public void LoadScene(int index)
    {
        StartCoroutine(FadeAndLoad(index));
    }

    public void LoadSceneAlreadyFadedOut(int index)
    {
        StartCoroutine(LoadSceneFromBlack(index));
    }

    private IEnumerator FadeAndLoad(int index)
    {
        yield return StartCoroutine(FadeOutRoutine());

        SceneManager.LoadScene(index);

        yield return StartCoroutine(FadeInRoutine());
    }

    private IEnumerator LoadSceneFromBlack(int index)
    {
        SceneManager.LoadScene(index);

        yield return StartCoroutine(FadeInRoutine());
    }

    public IEnumerator FadeInRoutine()
    {
        fadeImage.gameObject.SetActive(true);

        float t = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutRoutine()
    {
        fadeImage.gameObject.SetActive(true);

        float t = 0f;
        Color c = fadeImage.color;

        float startAlpha = c.a;
        float targetAlpha = 1f;

        if (startAlpha >= 0.99f)
        {
            c.a = 1f;
            fadeImage.color = c;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }
    public void setGlobalVolume(Slider slider)
    {
        AudioListener.volume = slider.value;
    }
}