using System.Collections;
using UnityEngine;
using Venice;

public class SpotLightObject : RWorldObject
{
   

    public void OnEnterIt(Entity entity)
    {

    }



    public void OnExitIt(Entity entity)
    {

    }

    [Header("Area")]
    public Vector3 center;
    public float radius = 10f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float steeringSpeed = 2f;

    [Header("Curve Motion")]
    public float wobbleRadius = 1f;
    public float wobbleSpeed = 1f;

    [Header("Pause")]
    [Range(0f, 1f)]
    public float pauseChance = 0.3f;
    public Vector2 pauseDuration = new Vector2(1f, 3f);

    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    public bool paused;

    private void Start()
    {
        PickNewTarget();
    }

    private void Update()
    {
        if (paused)
            return;
        Vector3 desiredDirection =
            (targetPosition - transform.position).normalized;

        currentVelocity = Vector3.Lerp(
            currentVelocity,
            desiredDirection * moveSpeed,
            steeringSpeed * Time.deltaTime);

        float t = Time.time * wobbleSpeed;

        Vector3 wobble =
            transform.right * Mathf.Sin(t) * wobbleRadius +
            transform.forward * Mathf.Cos(t * 0.7f) * wobbleRadius;

        transform.position +=
            (currentVelocity + wobble * 0.25f) * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            StartCoroutine(ChooseNextAction());
        }
    }

    private IEnumerator ChooseNextAction()
    {
        paused = true;

        if (Random.value < pauseChance)
        {
            yield return new WaitForSeconds(
                Random.Range(pauseDuration.x, pauseDuration.y));
        }

        PickNewTarget();
        paused = false;
    }

    private void PickNewTarget()
    {
        Vector2 p = Random.insideUnitCircle * radius;

        targetPosition = center + new Vector3(
            p.x,
            0f,
            p.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.25f);
    }
}
