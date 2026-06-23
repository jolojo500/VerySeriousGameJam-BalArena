using UnityEngine;
using Venice;

public class HostileState : CPUAIState
{
    private float dashCooldownTimer;
    private float dashTimer;
    private float attackButtonTimer;
    private float strafeSwitchTimer;
    private int strafeDirection;
    private Vector3 dashDirection;
    private bool pushedPlayerThisDash;

    public override void OnEnter()
    {
        base.OnEnter();

        dashCooldownTimer = Random.Range(0.5f, 1.2f);
        dashTimer = 0f;
        attackButtonTimer = 0f;
        pushedPlayerThisDash = false;

        PickStrafeDirection();

        Debug.Log("Entering Hostile State");
    }

    public override void OnUpdate()
    {
        if (Player.Instance == null || Entity == null)
            return;

        if (!AIMachine.IsAggressive)
        {
            AIMachine.Set<StandByState>();
            return;
        }

        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;
        toPlayer.y = 0f;

        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= 0.001f)
            return;

        Vector3 playerDirection = toPlayer.normalized;
        Vector3 separation = AIMachine.GetSeparationVector() * AIMachine.AggressiveSeparationWeight;

        if (dashTimer > 0f)
        {
            UpdateDash(separation);
            return;
        }

        dashCooldownTimer -= Time.deltaTime;

        if (dashCooldownTimer <= 0f && distanceToPlayer <= AIMachine.DashStartMaxDistance)
        {
            StartDash(playerDirection);
            return;
        }

        UpdateNormalHostileMovement(playerDirection, distanceToPlayer, separation);
    }

    private void UpdateNormalHostileMovement(Vector3 playerDirection, float distanceToPlayer, Vector3 separation)
    {
        strafeSwitchTimer -= Time.deltaTime;

        if (strafeSwitchTimer <= 0f)
            PickStrafeDirection();

        Vector3 moveDirection = Vector3.zero;

        float preferred = AIMachine.PreferredPlayerDistance;
        float buffer = AIMachine.PreferredDistanceBuffer;

        if (distanceToPlayer > preferred + buffer)
        {
            moveDirection += playerDirection;
        }
        else if (distanceToPlayer < preferred - buffer)
        {
            moveDirection -= playerDirection;
        }
        else
        {
            Vector3 strafe = Vector3.Cross(Vector3.up, playerDirection).normalized;
            moveDirection += strafe * strafeDirection * AIMachine.StrafeWeight;
        }

        moveDirection += separation;

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(moveDirection));
        FollowerCPU.SetButtonState("Attack", false);
    }

    private void StartDash(Vector3 direction)
    {
        dashDirection = direction;
        dashTimer = AIMachine.DashDuration;
        attackButtonTimer = AIMachine.DashAttackButtonTime;
        pushedPlayerThisDash = false;

        dashCooldownTimer = Random.Range(
            AIMachine.DashCooldownMin,
            AIMachine.DashCooldownMax
        );

        if (Entity.Rb != null)
        {
            Entity.Rb.AddForce(dashDirection * AIMachine.DashImpulse, ForceMode.Impulse);
        }

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(dashDirection));
        FollowerCPU.SetButtonState("Attack", true);
    }

    private void UpdateDash(Vector3 separation)
    {
        dashTimer -= Time.deltaTime;
        attackButtonTimer -= Time.deltaTime;

        Vector3 moveDirection = dashDirection + separation * 0.35f;

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(moveDirection));
        FollowerCPU.SetButtonState("Attack", attackButtonTimer > 0f);

        TryPushPlayer();

        if (dashTimer <= 0f)
        {
            FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
            FollowerCPU.SetButtonState("Attack", false);
        }
    }

    private void TryPushPlayer()
    {
        if (!AIMachine.DirectlyPushPlayerOnDash)
            return;

        if (pushedPlayerThisDash)
            return;

        if (Player.Instance == null || Player.Instance.Rb == null)
            return;

        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;
        toPlayer.y = 0f;

        if (toPlayer.magnitude > AIMachine.DashPushRadius)
            return;

        Vector3 pushDirection = toPlayer.normalized;

        Player.Instance.Rb.AddForce(
            pushDirection * AIMachine.DashPushForce + Vector3.up * AIMachine.DashPushUpForce,
            ForceMode.Impulse
        );

        pushedPlayerThisDash = true;
    }

    private void PickStrafeDirection()
    {
        strafeDirection = Random.value < 0.5f ? -1 : 1;
        strafeSwitchTimer = Random.Range(1.2f, 2.5f);
    }

    public override void OnExit()
    {
        base.OnExit();

        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);
    }
}