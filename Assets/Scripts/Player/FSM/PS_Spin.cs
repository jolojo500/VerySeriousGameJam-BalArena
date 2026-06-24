using UnityEngine;

namespace Venice
{
    public class PS_Spin : BallerinaState
    {
        private float _timer;
        private bool _validSpin;

        public PS_Spin() : base(4)
        {
        }

        public override void OnEnter()
        {
            _validSpin = Player.Instance != null && Entity == Player.Instance;

            if (!_validSpin)
            {
                ExitSpin();
                return;
            }

            _timer = Attributes.SpinDuration;

            Attributes.isSpin = true;
            Attributes.isAttacking = false;
            Attributes.Damaged = false;

            DoSmallAirPop();

            Visual?.ApplySquashAndStretch(0.9f, 0.12f);
        }

        public override void OnUpdate()
        {
            if (!_validSpin)
            {
                ExitSpin();
                return;
            }

            _timer -= Time.deltaTime;

            ChargeSpin(Time.deltaTime);

            if (_timer <= 0f)
            {
                ExitSpin();
            }
        }

        public override void OnExit()
        {
            Attributes.isSpin = false;
        }

        private void DoSmallAirPop()
        {
            if (Rb == null)
                return;

            Vector3 velocity = Rb.linearVelocity;

            if (velocity.y < 0f)
                velocity.y *= 0.35f;

            velocity.y += Attributes.SpinUpwardBurstSpeed;

            velocity.y = Mathf.Min(velocity.y, 4f);

            Rb.linearVelocity = velocity;

            Attributes.Grounded = false;
            Entity.YSpeed = velocity.y;
        }

        private void ChargeSpin(float dt)
        {
            Attributes.AddToSpin(Attributes.SpinChargePerSecond * dt);
        }

        private void ExitSpin()
        {
            Attributes.isSpin = false;

            if (!Attributes.Grounded)
                Entity.Machine.Set<PS_Air>();
        }
    }
}