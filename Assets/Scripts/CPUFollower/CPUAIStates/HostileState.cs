using UnityEngine;
using Venice;

public class HostileState : CPUAIState
{
    private enum AttackPhase
    {
        Chasing,
        WindUp,
        Dashing,
        Recovery
    }

    private AttackPhase phase;

    private float dashCooldownTimer;
    private float windUpTimer;
    private float dashTimer;
    private float attackButtonTimer;
    private float recoveryTimer;

    private Vector3 dashDirection;
    private bool pushedPlayerThisDash;

    public override void OnEnter()
    {
        base.OnEnter();

        phase = AttackPhase.Chasing;

        dashCooldownTimer = Random.Range(0.5f, 1.2f);
        windUpTimer = 0f;
        dashTimer = 0f;
        attackButtonTimer = 0f;
        recoveryTimer = 0f;
        pushedPlayerThisDash = false;

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

        switch (phase)
        {
            case AttackPhase.Chasing:
                UpdateChasing();
                break;

            case AttackPhase.WindUp:
                UpdateWindUp();
                break;

            case AttackPhase.Dashing:
                UpdateDash();
                break;

            case AttackPhase.Recovery:
                UpdateRecovery();
                break;
        }
    }

    private void UpdateChasing()
    {
        Vector3 toPlayer = GetDirectionToPlayer(out float distanceToPlayer);

        if (distanceToPlayer <= 0.001f)
            return;

        dashCooldownTimer -= Time.deltaTime;

        bool canAttack = dashCooldownTimer <= 0f;
        bool closeEnough = distanceToPlayer <= AIMachine.DashStartMaxDistance;

        if (canAttack && closeEnough)
        {
            StartWindUp(toPlayer);
            return;
        }

        Vector3 separation = AIMachine.GetSeparationVector() * AIMachine.AggressiveSeparationWeight;

        Vector3 moveDirection = Vector3.zero;

        if (distanceToPlayer > 1.5f)
            moveDirection += toPlayer;
        else
            moveDirection -= toPlayer * 0.5f;

        moveDirection += separation * 0.5f;

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(moveDirection));
        FollowerCPU.SetButtonState("Attack", false);
    }

    private void StartWindUp(Vector3 directionToPlayer)
    {
        phase = AttackPhase.WindUp;

        dashDirection = directionToPlayer.normalized;
        windUpTimer = AIMachine.DashWindUpDuration;
        pushedPlayerThisDash = false;

        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);

        BallerinaEntity ballerina = Entity as BallerinaEntity;

        if (ballerina != null && ballerina.Visual != null)
        {
            ballerina.Visual.SetBool(AIMachine.WindUpBoolName, true);
        }

        Debug.Log("AI WINDING UP ATTACK");
    }

    private void UpdateWindUp()
    {
        Vector3 toPlayer = GetDirectionToPlayer(out float distanceToPlayer);

        if (distanceToPlayer > 0.001f)
            dashDirection = Vector3.Lerp(dashDirection, toPlayer, Time.deltaTime * AIMachine.WindUpTurnSpeed).normalized;

        windUpTimer -= Time.deltaTime;

        // Stay still during wind-up so the player can read it.
        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);

        if (windUpTimer <= 0f)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        phase = AttackPhase.Dashing;

        Debug.Log("AI STARTING ATTACK DASH");

        dashTimer = Mathf.Max(0.25f, AIMachine.DashDuration);
        attackButtonTimer = Mathf.Max(0.12f, AIMachine.DashAttackButtonTime);
        pushedPlayerThisDash = false;

        dashCooldownTimer = Random.Range(
            AIMachine.DashCooldownMin,
            AIMachine.DashCooldownMax
        );

        BallerinaEntity ballerina = Entity as BallerinaEntity;

        if (ballerina != null && ballerina.Visual != null)
        {
            ballerina.Visual.SetBool(AIMachine.WindUpBoolName, false);
            ballerina.Visual.SetTrigger(AIMachine.AttackTriggerName);
        }

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(dashDirection));
        FollowerCPU.SetButtonState("Attack", true);

        ForceDashVelocity();
    }

    private void UpdateDash()
    {
        dashTimer -= Time.deltaTime;
        attackButtonTimer -= Time.deltaTime;

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(dashDirection));
        FollowerCPU.SetButtonState("Attack", attackButtonTimer > 0f);

        ForceDashVelocity();
        TryPushPlayer();

        if (dashTimer <= 0f)
        {
            FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
            FollowerCPU.SetButtonState("Attack", false);

            phase = AttackPhase.Recovery;
            recoveryTimer = AIMachine.DashRecoveryDuration;
        }
    }

    private void UpdateRecovery()
    {
        recoveryTimer -= Time.deltaTime;

        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);

        if (recoveryTimer <= 0f)
        {
            phase = AttackPhase.Chasing;
        }
    }

    private void ForceDashVelocity()
    {
        if (Entity == null || Entity.Rb == null)
            return;

        float lungeSpeed = AIMachine.DashImpulse;

        Entity.SetHorizontalVelocity(dashDirection * lungeSpeed);
    }

    private void TryPushPlayer()
    {
        if (pushedPlayerThisDash)
            return;

        if (Player.Instance == null || Player.Instance.Rb == null)
            return;

        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance > AIMachine.DashPushRadius)
            return;

        if (distance <= 0.001f)
            return;

        Vector3 pushDirection = toPlayer.normalized;

        float facingDot = Vector3.Dot(dashDirection.normalized, pushDirection);

        if (facingDot < 0.25f)
            return;

        Player.Instance.TriggerDamage(1);

        Player.Instance.Rb.AddForce(
            pushDirection * AIMachine.DashPushForce + Vector3.up * AIMachine.DashPushUpForce,
            ForceMode.Impulse
        );

        pushedPlayerThisDash = true;

        Debug.Log("AI dash hit and pushed player.");
    }

    private Vector3 GetDirectionToPlayer(out float distance)
    {
        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;
        toPlayer.y = 0f;

        distance = toPlayer.magnitude;

        if (distance <= 0.001f)
            return Vector3.zero;

        return toPlayer.normalized;
    }

    public override void OnExit()
    {
        base.OnExit();

        BallerinaEntity ballerina = Entity as BallerinaEntity;

        if (ballerina != null && ballerina.Visual != null)
        {
            ballerina.Visual.SetBool(AIMachine.WindUpBoolName, false);
        }

        FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
        FollowerCPU.SetButtonState("Attack", false);
    }
}