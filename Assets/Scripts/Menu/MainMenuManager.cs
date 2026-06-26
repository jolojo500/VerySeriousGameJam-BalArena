using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        // Make sure only main menu is visible at start
        ShowPanel(mainMenuPanel);

        // Hook up buttons
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    private void OnStartClicked()
    {
        SceneLoader.Instance.LoadScene(1);
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
}