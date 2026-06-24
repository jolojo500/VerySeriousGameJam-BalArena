using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Venice
{
    public class PS_Attack : BallerinaState
    {
        private float _timer;


        private readonly HashSet<Hittable> _hitThisSwing = new HashSet<Hittable>();
        public PS_Attack() : base(3)
        {
        }

        public override void OnEnter()
        {
            Entity.Attributes.isAttacking = true;
            _hitThisSwing.Clear();
            Attributes.Damaged = false;
            Entity.Attributes.AddToSpin(-Entity.SpinKickCost);
            Visual.ApplySquashAndStretch(1.1f, .2f);
            _timer = PhysicsInfo.AttackDuration;

            Vector3 dir = CalculatedInputs;
            if (dir == Vector3.zero) dir = Rb.linearVelocity.normalized;
            if(dir == Vector3.zero) dir = Transform.forward;
            dir = dir.normalized;

            Entity.SetHorizontalVelocity(dir * PhysicsInfo.AttackLungeSpeed);

            base.OnEnter();
        }

        public override void OnExit()
        {
            Entity.Attributes.isAttacking = false;
            Vector3 flat = Entity.HorizontalVelocity;
            if (flat.magnitude > PhysicsInfo.MaxSpeed)
                Entity.SetHorizontalVelocity(flat.normalized * PhysicsInfo.MaxSpeed);

            base.OnExit();
        }

        public void Attack()
        {
            Vector3 attackDirection = Rb.linearVelocity.normalized;

            if (attackDirection == Vector3.zero)
                attackDirection = Transform.forward;

            Vector3 center = Transform.position + Vector3.up + attackDirection * 0.3f;

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
                if (target == null)
                    continue;

                if (target.transform.IsChildOf(Entity.transform))
                    continue;

                if (!_hitThisSwing.Add(target))
                    continue;

                HitInfo info = new HitInfo
                {
                    SourceEntity = Entity,
                    SourcePosition = Transform.position,
                    KnockbackForce = 1f + (.2f * Entity.Controllers.GetController<ComboController>().ComboCount),
                    HitCooldown = .3f,
                    Owner = Entity.gameObject
                };

                target.Hit(info);
                HitStopManager.Instance?.HitStop(0.1f);
                Entity.Controllers.GetController<ComboController>().ConfirmHit();
            }
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Collision.GroundCollision();
            if (Entity.HandleFixedTimer(ref _timer))
            {
                Entity.SetHorizontalVelocity(Entity.HorizontalVelocity/3f);
                Machine.Set<PS_Move>();
            }
        }

        public override void OnUpdate()
        {
            Attack();
        }
    }
}
