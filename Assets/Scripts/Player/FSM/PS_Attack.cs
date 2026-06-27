using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Venice
{
    public class PS_Attack : BallerinaState
    {
        private float _timer;

        private bool _isPlayerKick;
        private Player _player;
        private PlayerKickTier _kickTier;

        private readonly HashSet<Hittable> _hitThisSwing = new HashSet<Hittable>();

        public PS_Attack() : base(3)
        {
        }

        public override void OnEnter()
        {
            _hitThisSwing.Clear();

            _player = Entity as Player;
            _isPlayerKick = _player != null;

            if (_isPlayerKick)
            {
                bool hasEnergy = _player.KickEnergyTiers.TryGetTier(
                    Attributes.CurrentSpin,
                    Attributes.MaxSpin,
                    out _kickTier
                );

                if (!hasEnergy)
                {
                    ExitAttackImmediately();
                    return;
                }

                float energyCost = _player.KickEnergyTiers.GetEnergyCost(Attributes.CurrentSpin);
                Attributes.AddToSpin(-energyCost);
            }

            Entity.Attributes.isAttacking = true;
            Attributes.Damaged = false;

            Visual.ApplySquashAndStretch(1.1f, .2f);

            float durationMultiplier = _isPlayerKick ? _kickTier.AttackDurationMultiplier : 1f;
            _timer = PhysicsInfo.AttackDuration * durationMultiplier;

            Vector3 dir = CalculatedInputs;

            if (dir == Vector3.zero)
                dir = Rb.linearVelocity.normalized;

            if (dir == Vector3.zero)
                dir = Transform.forward;

            dir = dir.normalized;

            float lungeSpeed = _isPlayerKick
                ? _kickTier.KickLungeSpeed
                : PhysicsInfo.AttackLungeSpeed;

            Entity.SetHorizontalVelocity(dir * lungeSpeed);

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

                float comboBonus = .2f * Entity.Controllers.GetController<ComboController>().ComboCount;

                HitInfo info = new HitInfo
                {
                    SourceEntity = Entity,
                    SourcePosition = Transform.position,

                    Damage = _isPlayerKick ? _kickTier.Damage : 1,

                    KnockbackForce = _isPlayerKick
                        ? _kickTier.KnockbackForce
                        : 1f + comboBonus,

                    HitCooldown = .3f,
                    Owner = Entity.gameObject
                };
                BossKnockbackResistance bossResistance = target.GetComponentInParent<BossKnockbackResistance>();

                if (bossResistance != null)
                {
                    info.KnockbackForce *= bossResistance.KnockbackMultiplier;
                }
                target.Hit(info);

                if (_isPlayerKick)
                {
                    HitStopManager.Instance?.HitStop(_kickTier.HitFreezeLength);
                    _player.PlayKickImpactFeedback(_kickTier);
                }
                else
                {
                    HitStopManager.Instance?.HitStop(0.1f);
                }

                Entity.Controllers.GetController<ComboController>().ConfirmHit();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Collision.GroundCollision();

            if (Entity.HandleFixedTimer(ref _timer))
            {
                Entity.SetHorizontalVelocity(Entity.HorizontalVelocity / 3f);
                Machine.Set<PS_Move>();
            }
        }

        public override void OnUpdate()
        {
            Attack();
        }

        private void ExitAttackImmediately()
        {
            Entity.Attributes.isAttacking = false;

            if (Attributes.Grounded)
                Machine.Set<PS_Move>();
            else
                Machine.Set<PS_Air>();
        }
    }
}