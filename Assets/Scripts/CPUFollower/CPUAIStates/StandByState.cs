using UnityEngine;
using Venice;

public class StandByState : CPUAIState
{
    private Vector3 targetPosition;

    public override void OnEnter()
    {
        base.OnEnter();
        PickNewTarget();
        Debug.Log("Entering StandBy State");
    }

    public override void OnUpdate()
    {
        if (Player.Instance == null || Entity == null)
            return;

        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;
        toPlayer.y = 0f;

        bool playerClose = toPlayer.magnitude <= AIMachine.HostileDistance;
        bool suspicionHigh = Player.Instance.Attributes.CurrentSuspicion >= AIMachine.SuspicionHostileThreshold;

        if (playerClose || suspicionHigh)
        {
            AIMachine.Set<HostileState>();
            return;
        }

        Vector3 dir = targetPosition - Entity.transform.position;
        dir.y = 0f;

        if (dir.magnitude > 1f)
        {
            Vector2 moveInput = new Vector2(dir.normalized.x, dir.normalized.z);
            FollowerCPU.SetAxis2DValue("Move", moveInput);
        }
        else
        {
            FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
            PickNewTarget();
        }
    }

    private void PickNewTarget()
    {
        targetPosition = new Vector3(
            Random.Range(-13f, 13f),
            0f,
            Random.Range(-13f, 13f)
        );
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting StandBy State");
        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
    }
}