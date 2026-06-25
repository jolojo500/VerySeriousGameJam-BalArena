
using UnityEngine;

namespace Venice
{

    public class PS_Move : BallerinaState
    {

        public bool isSkidding;

        public PS_Move() : base(0)
        {
        }

        public override void OnEnter()
        {
            Attributes.Grounded = true;
            Attributes.Damaged = false;
            if(Entity.InputLockType == InputLockType.StopOnLand)
            {
                Entity.UnlockInputs();
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
                Entity.Rb.linearVelocity = Entity.HorizontalVelocity + Entity.SurfaceNormal * PhysicsInfo.JumpStrength;
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
            if (Entity.Attributes.CurrentSpin > 0)
            {
                //Player.Attributes.AddToSpin(Time.fixedDeltaTime * Player.SPINLossRate);
            }

        }

        private void GroundMovement()
        {
            var input = CalculatedInputs;
            Rb.linearVelocity.Split(Entity.SurfaceNormal, out Vector3 vLat, out Vector3 vVer);
            var previousVelocityDirection = vLat.normalized;

            var velocity = vLat.magnitude;
            var velocityDirection = velocity == 0 ? input.normalized : vLat.normalized;
            if (!isSkidding)
            {
                if (input != Vector3.zero)
                {
                    if (input != Vector3.zero)
                    {
                        Vector3 targetDirection = input.normalized;

                        if (velocity < PhysicsInfo.MaxSpeed)
                        {
                            velocity = Mathf.Min(
                                velocity + PhysicsInfo.Acceleration * Time.fixedDeltaTime,
                                PhysicsInfo.MaxSpeed
                            );
                        }

                        float dot = Vector3.Dot(velocityDirection, targetDirection);

                        // Hard reverse: left -> right or right -> left
                        if (dot < -0.85f)
                        {
                            // Turn immediately, but keep most momentum.
                            velocityDirection = targetDirection;

                            // Optional small speed loss so it does not feel too instant.
                            velocity *= 0.75f;
                        }
                        else
                        {
                            velocityDirection = Vector3.Lerp(
                                velocityDirection,
                                targetDirection,
                                PhysicsInfo.TurnRate * PhysicsInfo.TurnRateCurve.Evaluate(velocity) * Time.fixedDeltaTime
                            ).normalized;
                        }
                    }
                }
                else
                {
                    if (velocity > 0.1f) velocity = Mathf.Max(velocity - PhysicsInfo.Friction * Time.fixedDeltaTime, 0);
                    else
                    {
                        if (Entity.SurfaceNormal.y > 0.8f)
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


            Rb.linearVelocity = new Vector3(velocityDirection.x * velocity,velocityDirection.y * velocity,0f);

        }
        public void SlopeRepel()
        {
            float slopeRepelTarget = 
                Mathf.Lerp(PhysicsInfo.SlopeRepelUpHill,
                PhysicsInfo.SlopeRepelDownHill,
                1 - (1 + Rb.linearVelocity.normalized.y) / 2);

            if (Entity.SurfaceNormal.y < 0.6f)
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


            if (Input.GetButtonDown(GamePreference.JumpButton))
            {
                JumpRequested = true;
            }

            if (Input.GetButtonDown(GamePreference.AttackButton))
            {
                if (Entity.Attributes.CurrentSpin >= Entity.SpinKickCost)
                {
                    Machine.Set<PS_Attack>();
                }
            }
            if (Input.GetButtonDown(GamePreference.PoseButton) && Attributes.IsInSpotLight)
            {
                Machine.Set<PS_Pose>();
                return;
            }

        }

        public override void OnVisualUpdate()
        {
            base.OnVisualUpdate();
        }
    }

}