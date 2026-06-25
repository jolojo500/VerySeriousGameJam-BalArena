using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class PlayAlembicCurtain : MonoBehaviour
{
    private AlembicStreamPlayer player;

    public float startTime = 0.04166f;
    public float endTime = 0.9647f;
    public float speed = 0.25f;

    private float currentTime;

    void Start()
    {
        player = GetComponent<AlembicStreamPlayer>();
        currentTime = startTime;
    }

    void Update()
    {
        if (currentTime >= endTime) return;

        currentTime += Time.deltaTime * speed;

        if (currentTime > endTime)
            currentTime = endTime;

        player.CurrentTime = currentTime;
        player.UpdateImmediately(currentTime);
    }
}