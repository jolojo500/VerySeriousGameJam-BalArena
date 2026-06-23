using UnityEngine;
using Venice;

public class SuspicionZone : MonoBehaviour
{
    public Collider ZoneCollider;

    private void Awake()
    {
        if (ZoneCollider == null)
            ZoneCollider = GetComponent<Collider>();

        if (ZoneCollider != null)
            ZoneCollider.isTrigger = true;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);

        if (ZoneCollider != null)
            ZoneCollider.enabled = active;
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public bool ContainsPlayer(Player player)
    {
        if (player == null || ZoneCollider == null || !ZoneCollider.enabled)
            return false;

        Vector3 playerPosition = player.transform.position;

        Vector3 closestPoint = ZoneCollider.ClosestPoint(playerPosition);

        float distance = Vector3.Distance(playerPosition, closestPoint);

        return distance <= 0.05f;
    }
}