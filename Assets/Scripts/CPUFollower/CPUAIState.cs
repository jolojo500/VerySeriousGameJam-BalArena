using UnityEngine;
using Venice;

public class CPUAIState : State
{
    public CPUInputManager FollowerCPU;
    public AIStateMachine AIMachine;

    public BallerinaEntity Entity => FollowerCPU != null ? FollowerCPU.Entity as BallerinaEntity : null;

    private float randomJumpTimer;

    public override void OnEnter()
    {
        ResetRandomJumpTimer();
    }

    protected void HandleRandomJump()
    {
        if (AIMachine == null || FollowerCPU == null || Entity == null)
            return;

        if (!AIMachine.CanRandomJump)
            return;

        if (!Entity.Attributes.Grounded)
            return;

        randomJumpTimer -= Time.deltaTime;

        if (randomJumpTimer > 0f)
            return;

        ResetRandomJumpTimer();

        if (Random.value > AIMachine.JumpChanceWhenTimerEnds)
            return;

        ForceJump();
    }

    private void ForceJump()
    {
        if (Entity == null || Entity.Rb == null)
            return;

        if (Entity.Rb.isKinematic)
            return;

        Debug.Log($"{Entity.name} AI FORCE JUMP.");

        Entity.Attributes.Grounded = false;

        Entity.Rb.linearVelocity = new Vector3(
            Entity.Rb.linearVelocity.x,
            AIMachine.AIJumpSpeed,
            Entity.Rb.linearVelocity.z
        );

        Entity.YSpeed = AIMachine.AIJumpSpeed;

        if (Entity.Machine != null)
            Entity.Machine.Set<PS_Air>();
    }

    private void ResetRandomJumpTimer()
    {
        if (AIMachine == null)
        {
            randomJumpTimer = 1f;
            return;
        }

        randomJumpTimer = Random.Range(
            AIMachine.RandomJumpMinTime,
            AIMachine.RandomJumpMaxTime
        );
    }
    public override void OnExit()
    {
    }


}