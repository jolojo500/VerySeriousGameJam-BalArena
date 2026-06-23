using UnityEngine;
using Venice;

public class HostileState : CPUAIState
{
    private float attackTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        attackTimer = 0f;
        Debug.Log("Entering Hostile State");
    }

    public override void OnUpdate()
    {
        if (Player.Instance == null || Entity == null)
            return;

        Vector3 dir = Player.Instance.transform.position - Entity.transform.position;
        dir.y = 0f;

        float distance = dir.magnitude;

        FollowerCPU.SetButtonState("Attack", false);

        if (distance > AIMachine.LoseDistance)
        {
            AIMachine.Set<StandByState>();
            return;
        }

        if (distance > AIMachine.AttackDistance)
        {
            Vector2 moveInput = new Vector2(dir.normalized.x, dir.normalized.z);
            FollowerCPU.SetAxis2DValue("Move", moveInput);
            return;
        }

        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            FollowerCPU.SetButtonState("Attack", true);
            attackTimer = AIMachine.AttackCooldown;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting Hostile State");
        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);
    }
}