
using UnityEngine;

namespace Venice
{

    public class PS_Move : PlayerState
    {

        public bool isSkidding;

        public PS_Move() : base(0)
        {
        }

        public override void OnEnter()
        {
            Attributes.Grounded = true;
            Attributes.Damaged = false;
            if(Player.InputLockType == InputLockType.StopOnLand)
            {
                Player.UnlockInputs();
            }
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            if (JumpRequested)
            {
                JumpRequested = false;
                //SetAirState
                //Apply Velocity at up;
                Player.Rb.linearVelocity = Player.HorizontalVelocity + Player.SurfaceNormal * PhysicsInfo.JumpStrength;
                Machine.Get<PS_Air>().IsJump = true;
                Machine.Set<PS_Air>();
                return;
            }
            GroundMovement();
            SlopeRepel();
            if (!Collision.GroundCollision())
            {
                Machine.Set<PS_Air>();
            }
            if (Player.Attributes.CurrentSpin > 0)
            {
                //Player.Attributes.AddToSpin(Time.fixedDeltaTime * Player.SPINLossRate);
            }
        }

        private void GroundMovement()
        {
            var input = CalculatedInputs;
            Rb.linearVelocity.Split(Player.SurfaceNormal, out Vector3 vLat, out Vector3 vVer);
            var previousVelocityDirection = vLat.normalized;

            var velocity = vLat.magnitude;
            var velocityDirection = velocity == 0 ? input.normalized : vLat.normalized;
            if (!isSkidding)
            {
                if (input != Vector3.zero)
                {
                    if (velocity < PhysicsInfo.MaxSpeed) velocity = Mathf.Min(velocity + PhysicsInfo.Acceleration * Time.fixedDeltaTime, PhysicsInfo.MaxSpeed);
                    velocityDirection = Vector3.Lerp(velocityDirection,
                        input.normalized, PhysicsInfo.TurnRate * PhysicsInfo.TurnRateCurve.Evaluate(velocity) * Time.fixedDeltaTime).normalized;

                    //velocity -= Mathf.Abs(Mathf.Sin(Vector3.Angle(velocityDirection, previousVelocityDirection) * Mathf.Deg2Rad)) * PhysicsInfo.SpeedLoss * Time.fixedDeltaTime * PhysicsInfo.SpeedLossCurve.Evaluate(velocity);

                    if (Vector3.Dot(velocityDirection, input.normalized) < -0.85f)
                    {
                        isSkidding = true;
                    }
                }
                else
                {
                    if (velocity > 0.1f) velocity = Mathf.Max(velocity - PhysicsInfo.Friction * Time.fixedDeltaTime, 0);
                    else
                    {
                        if (Player.SurfaceNormal.y > 0.8f)
                        {
                            isSkidding = false;
                            velocity = 0;
                        }
                    }
                }

                if (Mathf.Approximately(velocity, 0))
                {
                    isSkidding = false;
                    velocity = 0;
                }
            }
            else
            {
                if (Vector3.Dot(velocityDirection, input.normalized) > -0.85f)
                {
                    isSkidding = false;
                }
                if (velocity > 0) {velocity = Mathf.Max(velocity - PhysicsInfo.Deceleration * Time.fixedDeltaTime, 0); }
                else
                {
                    velocityDirection = input.normalized;
                    isSkidding = false;
                }

                if (input == Vector3.zero) isSkidding = false;
            }


            Rb.linearVelocity = velocityDirection * velocity;

        }
        public void SlopeRepel()
        {
            float slopeRepelTarget = 
                Mathf.Lerp(PhysicsInfo.SlopeRepelUpHill,
                PhysicsInfo.SlopeRepelDownHill,
                1 - (1 + Rb.linearVelocity.normalized.y) / 2);

            if (Player.SurfaceNormal.y < 0.6f)
            {
                Rb.linearVelocity += Vector3.down * slopeRepelTarget * Time.fixedDeltaTime;
            }

        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnUpdate()
        {


            if (Input.GetButtonDown("Crouch"))
            {
                Debug.Log("Bruih");
                Player.Attributes.AddToSpin(-25);
            }
            if (Input.GetButtonDown(GamePreference.JumpButton))
            {
                JumpRequested = true;
            }

        }

        public override void OnVisualUpdate()
        {
            base.OnVisualUpdate();
        }
    }

}