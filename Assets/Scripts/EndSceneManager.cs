using UnityEngine;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    [Header("Result Image")]
    public GameObject LoseObject;
    public GameObject WinObject;
    public Image normalText;
    public Sprite pacifistText;

    [Header("Sprites")]
    public Sprite WinSprite;
    public Sprite LostSprite;

    private void Start()
    {
        Time.timeScale = 1f;


        if (SceneLoader.gameLost)
        {
            LoseObject.SetActive(true);
            WinObject.SetActive(false);
        }
        else
        {
            LoseObject.SetActive(false);
            WinObject.SetActive(true);
            normalText.sprite = SceneLoader.wasPassive ? pacifistText: normalText.sprite;
        }
        
        SceneLoader.wasPassive = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneLoader.gameLost = false;
        SceneLoader.Instance.LoadScene(1);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.gameLost = false;
        SceneLoader.Instance.LoadScene(0);
    }
}