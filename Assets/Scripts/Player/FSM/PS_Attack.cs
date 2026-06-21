using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Venice
{
    public class PS_Attack : PlayerState
    {
        private float _timer;

        private readonly HashSet<Hittable> _hitThisSwing = new HashSet<Hittable>();
        public PS_Attack() : base(3)
        {
        }

        public override void OnEnter()
        {
            _hitThisSwing.Clear();
            Attributes.Damaged = false;
            Visual.ApplySquashAndStretch(1.1f, .2f);
            _timer = PhysicsInfo.AttackDuration;

            Vector3 dir = CalculatedInputs;
            if (dir == Vector3.zero) dir = Rb.linearVelocity.normalized;
            if(dir == Vector3.zero) dir = Transform.forward;
            dir = dir.normalized;

            Player.SetHorizontalVelocity(dir * PhysicsInfo.AttackLungeSpeed);

            base.OnEnter();
        }

        public override void OnExit()
        {

            Vector3 flat = Player.HorizontalVelocity;
            if (flat.magnitude > PhysicsInfo.MaxSpeed)
                Player.SetHorizontalVelocity(flat.normalized * PhysicsInfo.MaxSpeed);

            base.OnExit();
        }

        public void Attack()
        {
            Vector3 center = Transform.position + Vector3.up + Rb.linearVelocity.normalized * 0.3f;

            Debug.DrawLine(center, center + Vector3.up * 0.1f, Color.green);

            Collider[] hits = Physics.OverlapSphere(
                                center,
                                1.5f,
                                ~0,
                                QueryTriggerInteraction.Collide
                            );
            foreach (var hit in hits)
            {
                var target = hit.GetComponentInParent<Hittable>();
                if (target == null) continue;

                if (target.transform.IsChildOf(Player.transform)) continue;

                if (!_hitThisSwing.Add(target)) continue;

                target.Hit(new HitInfo
                {
                    SourcePosition = Transform.position,
                    KnockbackForce = 1f,
                    HitCooldown = .3f,
                    Owner = Player.gameObject
                });
            }
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Collision.GroundCollision();
            if (Player.HandleFixedTimer(ref _timer))
            {
                Player.SetHorizontalVelocity(Player.HorizontalVelocity/3f);
                Machine.Set<PS_Move>();
            }
        }

        public override void OnUpdate()
        {
            Attack();
        }
    }
}
