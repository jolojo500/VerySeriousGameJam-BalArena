using UnityEngine;

namespace Venice
{
    public class PS_Attack : PlayerState
    {
        private float _timer;

        public PS_Attack() : base(3)
        {
        }

        public override void OnEnter()
        {
            Attributes.Damaged = false;
            _timer = PhysicsInfo.AttackDuration;

            Vector3 dir = CalculatedInputs;
            if (dir == Vector3.zero) dir = Transform.forward;
            dir = dir.normalized;

            Player.SetHorizontalVelocity(dir * PhysicsInfo.AttackLungeSpeed);

            Player.AttackHitbox?.Begin();
            base.OnEnter();
        }

        public override void OnExit()
        {
            Player.AttackHitbox?.End();

            Vector3 flat = Player.HorizontalVelocity;
            if (flat.magnitude > PhysicsInfo.MaxSpeed)
                Player.SetHorizontalVelocity(flat.normalized * PhysicsInfo.MaxSpeed);

            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            Collision.GroundCollision();
            base.OnFixedUpdate();
        }

        public override void OnUpdate()
        {
            if (Player.HandleTimer(ref _timer))
            {
                Machine.Set<PS_Move>();
            }
        }
    }
}
