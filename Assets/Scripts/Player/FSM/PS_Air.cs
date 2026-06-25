using System;
using Unity.VisualScripting;
using UnityEngine;
using Venice;

public class PS_Air : BallerinaState
{

    public float Speed;
    public bool IsJump = false;
    public bool CanAscend = false;
    public bool isSkidding;

    public PS_Air() : base(1)
    {
    }
    public override void OnEnter()
    {
        Attributes.Grounded = false;
        if (IsJump)
        {
            CanAscend = true;
            Visual.ApplySquashAndStretch(1.2f, .2f);
            Visual.Play("Jump");
        }
        base.OnEnter();
        Entity.SurfaceNormal = Vector3.zero;
    }

    public override void OnExit()
    {
        IsJump = false;
    }

    public override void OnFixedUpdate()
    {
        if (Entity.YSpeed <= 0)
        {
            if (Collision.AirGroundCollision())
            {
                Machine.Set<PS_Move>();
                return;
            }
        }
        AirMovement();
        AirDrag();


        if(Transform.position.y < -5f)
        {
            Machine.Set<PS_Death>();
            return;
        }
    }

    private void AirMovement()
    {
        var input = CalculatedInputs;

        Rb.linearVelocity.Split(Vector3.up, out Vector3 vLat, out Vector3 vVer);

        float velocity = vLat.magnitude;
        Vector3 velocityDirection = velocity <= 0.01f
            ? input.normalized
            : vLat.normalized;

        if (input != Vector3.zero)
        {
            Vector3 targetDirection = input.normalized;

            if (velocity < PhysicsInfo.MaxSpeed)
            {
                velocity = Mathf.Min(
                    velocity + PhysicsInfo.AirAcceleration * Time.fixedDeltaTime,
                    PhysicsInfo.MaxSpeed
                );
            }

            float dot = Vector3.Dot(velocityDirection, targetDirection);

            // If the player fully reverses in the air, flip direction but keep most speed.
            if (dot < -0.85f)
            {
                velocityDirection = targetDirection;

                // Air reverse speed loss. Raise this for more momentum preservation.
                velocity *= 0.85f;
            }
            else
            {
                float airTurnAmount =
                    PhysicsInfo.TurnRate *
                    PhysicsInfo.TurnRateCurve.Evaluate(velocity) *
                    Time.fixedDeltaTime;

                // Air control should be weaker than ground, but not /6.
                airTurnAmount *= 0.5f;

                velocityDirection = Vector3.Lerp(
                    velocityDirection,
                    targetDirection,
                    airTurnAmount
                ).normalized;
            }
        }

        Rb.linearVelocity = velocityDirection * velocity + vVer;
        Rb.linearVelocity += Vector3.down * PhysicsInfo.Gravity * Time.fixedDeltaTime;
    }
    public void AirDrag()
    {
        Rb.linearVelocity -= ((Vector3.ProjectOnPlane(Rb.linearVelocity, Vector3.up) / PhysicsInfo.AirDrag)) * Time.fixedDeltaTime;
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
    }

    public override void OnUpdate()
    {
        if (Input.GetButtonDown(GamePreference.AttackButton))
        {
            if (Attributes.CurrentSpin >= Entity.SpinKickCost)
            {
                Machine.Set<PS_Attack>();
                return;
            }
        }

        if (IsJump && CanAscend)
        {
            if (Entity.YSpeed > PhysicsInfo.JumpCutoff && Input.GetButtonUp(GamePreference.JumpButton))
            {
                CanAscend = false;
                Entity.YSpeed = PhysicsInfo.JumpCutoff;
            }
        }

        base.OnUpdate();
    }

    public override void OnVisualUpdate()
    {
        base.OnVisualUpdate();
    }
}