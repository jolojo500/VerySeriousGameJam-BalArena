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
    private bool holdingPositionNearPlayer;
    public override void OnEnter()
    {
        base.OnEnter();
        phase = AttackPhase.Chasing;

        holdingPositionNearPlayer = false;

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
        HandleRandomJump();
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
        bool closeEnoughToAttack = distanceToPlayer <= AIMachine.DashStartMaxDistance;

        if (canAttack && closeEnoughToAttack)
        {
            StartWindUp(toPlayer);
            return;
        }

        if (AIMachine.StopMovingWhenCloseToPlayer)
        {
            if (!holdingPositionNearPlayer && distanceToPlayer <= AIMachine.StopMovingDistance)
            {
                holdingPositionNearPlayer = true;
            }
            else if (holdingPositionNearPlayer && distanceToPlayer >= AIMachine.ResumeMovingDistance)
            {
                holdingPositionNearPlayer = false;
            }

            if (holdingPositionNearPlayer)
            {
                FollowerCPU.SetAxis2DValue("Move", Vector2.zero);
                FollowerCPU.SetButtonState("Attack", false);

                if (AIMachine.ZeroVelocityWhenStopped && Entity != null)
                {
                    Entity.SetHorizontalVelocity(Vector3.zero);
                }

                return;
            }
        }

        Vector3 separation = AIMachine.GetSeparationVector() * AIMachine.AggressiveSeparationWeight;

        Vector3 moveDirection = toPlayer;
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

        FollowerCPU.SetAxis2DValue("Move", AIMachine.WorldDirectionToMoveInput(dashDirection));
        Entity.Attributes.isAttacking = true;
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
        Entity.Attributes.isAttacking = false;
        if (pushedPlayerThisDash)
            return;

        if (Player.Instance == null || Player.Instance.Rb == null)
            return;

        Vector3 toPlayer = Player.Instance.transform.position - Entity.transform.position;

        // Do not hit if the player is too high/low compared to the enemy.
        float verticalDistance = Mathf.Abs(toPlayer.y);

        if (verticalDistance > 1.2f)
            return;

        Vector3 flatToPlayer = toPlayer;
        flatToPlayer.y = 0f;

        if (flatToPlayer.sqrMagnitude <= 0.001f)
            return;

        Vector3 forward = dashDirection.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        float forwardDistance = Vector3.Dot(flatToPlayer, forward);
        float sideDistance = Mathf.Abs(Vector3.Dot(flatToPlayer, right));

        float hitForwardRange = AIMachine.DashPushRadius;
        float hitWidth = 0.8f;

        // Player must be in front of the kick, not beside/on top/behind.
        if (forwardDistance < 0.3f)
            return;

        if (forwardDistance > hitForwardRange)
            return;

        if (sideDistance > hitWidth)
            return;

        Vector3 pushDirection = flatToPlayer.normalized;

        Player.Instance.TriggerDamage(1);

        Player.Instance.Rb.AddForce(
            pushDirection * AIMachine.DashPushForce + Vector3.up * AIMachine.DashPushUpForce,
            ForceMode.Impulse
        );
        HitStopManager.Instance?.HitStop(0.06f);
        CameraShakeManager.Instance?.Shake(0.5f);
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
        FollowerCPU.SetButtonState("Jump", false);
    }
}