using UnityEngine;
using Venice;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GameObject PauseScreen;
    private bool gameEnded = false;
    private void Awake()
    {
        gameEnded = false;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        MusicManager.Instance.audioSource.pitch = 1f;
    }
    private void Update()
    {
        if (GameInput.GetButtonDown("Pause"))
            TogglePause();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EndGame(bool isDead)
    {
        gameEnded = true;
        SceneLoader.gameLost = isDead;
        if (isDead)
        {
            GlobalVolumeEffects.Instance.PlayDeath();
            MusicManager.Instance.audioSource.pitch = 0.5f;
            Invoke("loadScene", 5);
        }
        else
        {
            SceneLoader.Instance.LoadScene(2);
        }
        
    }
    public void TogglePause()
    {
        if (gameEnded) return;
        bool isPaused = (Time.timeScale == 0f);
        PauseScreen.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1f : 0f;
        
    }
    private void loadScene()
    {
        SceneLoader.Instance.LoadScene(2);
    }
}
