using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Venice; // Needed for GameInput

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button leaveButton;

    [Header("Intro Video")]
    [SerializeField] private VideoPlayer introVideoPlayer;
    [SerializeField] private GameObject introVideoPlayerObject;

    [Header("Scene Loading")]
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private string attackButtonName = "Attack";

    [Header("Music")]
    [SerializeField] private AudioSource menuMusic;
    private bool videoFinished;
    private bool waitingForTutorialInput;
    private bool sequenceRunning;

    private void Start()
    {
        menuMusic.Play();
        ShowPanel(mainMenuPanel);

        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);

        if (introVideoPlayer != null)
        {
            introVideoPlayerObject.SetActive(false);
            introVideoPlayer.loopPointReached += OnIntroVideoFinished;
            introVideoPlayer.Stop();
        }
    }

    private void Update()
    {
        if (!waitingForTutorialInput || sequenceRunning)
            return;
        Debug.Log("Checking for input");
        if (GameInput.GetButtonDown(attackButtonName))
        {
            Debug.Log("finish tutorial");
            StartCoroutine(FinishTutorialAndLoad());
        }
    }

    public void OnStartClicked()
    {
        if (sequenceRunning)
            return;

        StartCoroutine(StartIntroSequence());
    }

    private IEnumerator StartIntroSequence()
    {
        sequenceRunning = true;
        waitingForTutorialInput = false;

        yield return StartCoroutine(SceneLoader.Instance.FadeOutRoutine());
        menuMusic.Stop();
        ShowPanel(videoPanel);

        videoFinished = false;

        if (introVideoPlayer != null)
        {
            introVideoPlayer.Stop();
            introVideoPlayer.Prepare();

            while (!introVideoPlayer.isPrepared)
                yield return null;

            introVideoPlayer.Play();
            introVideoPlayerObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No intro VideoPlayer assigned.");
            videoFinished = true;
        }

        yield return StartCoroutine(SceneLoader.Instance.FadeInRoutine());

        while (!videoFinished)
            yield return null;

        yield return StartCoroutine(SceneLoader.Instance.FadeOutRoutine());
        introVideoPlayerObject.SetActive(false);
        ShowPanel(tutorialPanel);

        waitingForTutorialInput = true;
        sequenceRunning = false;
    }

    private IEnumerator FinishTutorialAndLoad()
    {
        sequenceRunning = true;
        waitingForTutorialInput = false;

        yield return StartCoroutine(SceneLoader.Instance.FadeOutRoutine());
        SceneLoader.Instance.LoadSceneAlreadyFadedOut(gameSceneIndex);
    }

    private void OnIntroVideoFinished(VideoPlayer player)
    {
        videoFinished = true;
    }
    public void OnReturnClicked()
    {
        ShowPanel(mainMenuPanel);
    }
    private void OnCreditsClicked()
    {
        ShowPanel(creditsPanel);
    }

    private void OnSettingsClicked()
    {
        ShowPanel(settingsPanel);
    }

    private void OnLeaveClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowPanel(GameObject panel)
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        videoPanel.SetActive(false);
        tutorialPanel.SetActive(false);

        panel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (introVideoPlayer != null)
            introVideoPlayer.loopPointReached -= OnIntroVideoFinished;
    }
}