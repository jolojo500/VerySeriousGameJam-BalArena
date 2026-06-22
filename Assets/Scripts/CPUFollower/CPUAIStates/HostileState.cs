using UnityEngine;
using Venice;

public class HostileState : CPUAIState
{
    Vector3 targetPosition;
    public HostileState()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entering Hostile State");
        targetPosition = Player.Instance.transform.position;

    }

    public override void OnUpdate()
    {
        FollowerCPU.SetButtonState("Attack", false);
        Vector3 dir = (targetPosition - Entity.transform.position);
        dir.y = 0;
        if (dir.magnitude > .5f)
        {
            FollowerCPU.SetAxis2DValue("Move", dir.normalized);
        }
        else
        {
            FollowerCPU.SetButtonState("Attack", true);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting StandBy State");
        FollowerCPU.SetAxis2DValue("Move", Vector3.zero);
        FollowerCPU.SetButtonState("Attack", false);
    }
}