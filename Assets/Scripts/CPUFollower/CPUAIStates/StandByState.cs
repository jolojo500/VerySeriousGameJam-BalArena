using UnityEngine;
using Venice;

public class StandByState : CPUAIState
{
    Vector3 targetPosition;
    public StandByState()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entering StandBy State");
        targetPosition = new Vector3(Random.Range(-13, 13), 0, Random.Range(-13, 13)); // Random position within a range

    }

    public override void OnUpdate()
    {
        Vector3 dir = (targetPosition - Entity.transform.position);
        dir.y = 0;
        if(dir.magnitude > 1f){

            Debug.Log("Moving");
            FollowerCPU.SetAxis2DValue("Move", dir.normalized.xzy());
        }else if(dir.magnitude > 0.5f)
        {
            FollowerCPU.SetAxis2DValue("Move", -dir.normalized.xzy());
        }
        else
        {
            targetPosition = new Vector3(Random.Range(-13, 13), 0, Random.Range(-13, 13)); // Random position within a range
        }

        if(Entity is BallerinaEntity be)
        {
            Debug.Log(be.Machine.CurrentState.MoveInput);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting StandBy State");
        FollowerCPU.SetAxis2DValue("Move", Vector3.zero);
    }
}