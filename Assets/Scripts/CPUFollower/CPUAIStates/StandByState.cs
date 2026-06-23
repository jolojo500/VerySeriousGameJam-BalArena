using UnityEngine;

public class StandByState : CPUAIState
{
    private Vector3 targetPosition;
    private float retargetTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        PickNewTarget();
        Debug.Log("Entering StandBy State");
    }

    public override void OnUpdate()
    {
        if (Entity == null)
            return;

        if (AIMachine.IsAggressive)
        {
            AIMachine.Set<HostileState>();
            return;
        }

        retargetTimer -= Time.deltaTime;

        Vector3 toTarget = targetPosition - Entity.transform.position;
        toTarget.y = 0f;

        bool reachedTarget = toTarget.magnitude <= AIMachine.WanderPointReachedDistance;
        bool retargetTimeExpired = retargetTimer <= 0f;

        if (reachedTarget || retargetTimeExpired)
        {
            PickNewTarget();
            toTarget = targetPosition - Entity.transform.position;
            toTarget.y = 0f;
        }

        Vector3 separation = AIMachine.GetSeparationVector() * AIMachine.WanderSeparationWeight;

        Vector3 moveDirection = Vector3.zero;

        if (toTarget.magnitude > AIMachine.WanderPointReachedDistance)
            moveDirection += toTarget.normalized;

        moveDirection += separation;

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(moveDirection));
        FollowerCPU.SetButtonState("Attack", false);
    }

    private void PickNewTarget()
    {
        targetPosition = AIMachine.GetRandomWanderPoint();
        retargetTimer = Random.Range(
            AIMachine.WanderRetargetMinTime,
            AIMachine.WanderRetargetMaxTime
        );
    }

    public override void OnExit()
    {
        base.OnExit();
        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);
    }
}