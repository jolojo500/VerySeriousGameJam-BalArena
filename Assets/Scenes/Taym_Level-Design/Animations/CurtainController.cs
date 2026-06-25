using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using System.Collections;

public class DoubleCurtainController : MonoBehaviour
{
    public GameObject leftOpenModel;
    public GameObject leftCloseModel;
    public GameObject rightOpenModel;
    public GameObject rightCloseModel;

    public AlembicStreamPlayer leftOpenPlayer;
    public AlembicStreamPlayer leftClosePlayer;
    public AlembicStreamPlayer rightOpenPlayer;
    public AlembicStreamPlayer rightClosePlayer;

    public float openEndTime = 14.58333f;
    public float closeEndTime = 25f;

    public float openDuration = 6f;
    public float closeDuration = 6f;

    bool isOpen = false;
    bool isAnimating = false;

    void Start()
    {
        leftOpenModel.SetActive(true);
        rightOpenModel.SetActive(true);

        leftCloseModel.SetActive(false);
        rightCloseModel.SetActive(false);

        leftOpenPlayer.CurrentTime = 0f;
        rightOpenPlayer.CurrentTime = 0f;
        leftClosePlayer.CurrentTime = 0f;
        rightClosePlayer.CurrentTime = 0f;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 200, 50), isOpen ? "Close Curtains" : "Open Curtains"))
        {
            ToggleCurtain();
        }
    }

    public void ToggleCurtain()
    {
        if (isAnimating) return;

        if (isOpen)
            StartCoroutine(PlayClose());
        else
            StartCoroutine(PlayOpen());
    }

    IEnumerator PlayOpen()
    {
        isAnimating = true;

        leftCloseModel.SetActive(false);
        rightCloseModel.SetActive(false);

        leftOpenModel.SetActive(true);
        rightOpenModel.SetActive(true);

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            float n = elapsed / openDuration;
            float t = Mathf.Lerp(0f, openEndTime, n);

            leftOpenPlayer.CurrentTime = t;
            rightOpenPlayer.CurrentTime = t;

            elapsed += Time.deltaTime;
            yield return null;
        }

        leftOpenPlayer.CurrentTime = openEndTime;
        rightOpenPlayer.CurrentTime = openEndTime;

        isOpen = true;
        isAnimating = false;
    }

    IEnumerator PlayClose()
    {
        isAnimating = true;

        leftOpenModel.SetActive(false);
        rightOpenModel.SetActive(false);

        leftCloseModel.SetActive(true);
        rightCloseModel.SetActive(true);

        float elapsed = 0f;

        while (elapsed < closeDuration)
        {
            float n = elapsed / closeDuration;
            float t = Mathf.Lerp(0f, closeEndTime, n);

            leftClosePlayer.CurrentTime = t;
            rightClosePlayer.CurrentTime = t;

            elapsed += Time.deltaTime;
            yield return null;
        }

        leftClosePlayer.CurrentTime = closeEndTime;
        rightClosePlayer.CurrentTime = closeEndTime;

        isOpen = false;
        isAnimating = false;
    }
}