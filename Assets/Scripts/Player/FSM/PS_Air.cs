using System;
using Unity.VisualScripting;
using UnityEngine;
using Venice;

public class PS_Air : PlayerState
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
            Visual.Play("Jump");
        }
        base.OnEnter();
        Player.SurfaceNormal = Vector3.zero;
    }

    public override void OnExit()
    {
        IsJump = false;
    }

    public override void OnFixedUpdate()
    {
        if (Player.YSpeed <= 0)
        {
            if (Collision.AirGroundCollision())
            {
                Machine.Set<PS_Move>();
                return;
            }
        }
        AirMovement();
        AirDrag();
    }

    private void AirMovement()
    {
        var input = CalculatedInputs;
        Rb.linearVelocity.Split(Vector3.up, out Vector3 vLat, out Vector3 vVer);
        var previousVelocityDirection = vLat.normalized;

        var velocity = vLat.magnitude;
        var velocityDirection = velocity == 0 ? input.normalized : vLat.normalized;
        if (!isSkidding)
        {
            if (input != Vector3.zero)
            {
                if (velocity < PhysicsInfo.MaxSpeed) velocity = Mathf.Min(velocity + PhysicsInfo.AirAcceleration * Time.fixedDeltaTime, PhysicsInfo.MaxSpeed);
                velocityDirection = Vector3.Lerp(velocityDirection,
                    input.normalized, PhysicsInfo.TurnRate * PhysicsInfo.TurnRateCurve.Evaluate(velocity) * Time.fixedDeltaTime / 6).normalized;

                velocity -= Mathf.Abs(Mathf.Sin(Vector3.Angle(velocityDirection, previousVelocityDirection) * Mathf.Deg2Rad) * 5) * PhysicsInfo.SpeedLoss * Time.fixedDeltaTime * PhysicsInfo.SpeedLossCurve.Evaluate(velocity);

                if (Vector3.Dot(velocityDirection, input.normalized) < -0.25f)
                {
                    isSkidding = true;
                }
            }
        }
        else
        {
            if (Vector3.Dot(velocityDirection, input.normalized) > -0.25f)
            {
                isSkidding = false;
            }
            if (velocity > 0) velocity = Mathf.Max(velocity - PhysicsInfo.AirAcceleration * Time.fixedDeltaTime, 0);
            else
            {
                velocityDirection = input.normalized;
                isSkidding = false;
            }

            if (input == Vector3.zero) isSkidding = false;
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
        if (IsJump && CanAscend)
        {
            if(Player.YSpeed > PhysicsInfo.JumpCutoff && Input.GetButtonUp(GamePreference.JumpButton))
            {
                CanAscend = false;
                Player.YSpeed = PhysicsInfo.JumpCutoff;
            }
        }

        base.OnUpdate();
    }

    public override void OnVisualUpdate()
    {
        base.OnVisualUpdate();
    }
}