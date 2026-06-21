using System;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Venice
{

    public class PS_Damaged : PlayerState
    {
        public HitInfo info;

        
        public float RecoveryTimer = .5f;

        public bool IsDamaged = false;

        public PS_Damaged() : base(0)
        {
        }

        public override void OnEnter()
        {
            Attributes.Grounded = true;
            Attributes.Damaged = true;
            Player.InputManager.BlockInput = true;
            RecoveryTimer = .5f;
            Vector3 source = (info == null) ? Transform.position + Vector3.forward:info.SourcePosition;
            Vector3 dir = (Transform.position - source).normalized;
            Player.SetHorizontalVelocity(dir * (info?.KnockbackForce ?? 5));
            Player.SetVerticalVelocity(Vector3.up * 5f);
            IsDamaged = true;
            Visual.Skin.material.SetInt("_Hurted", 1);
            base.OnEnter();
        }

        public override void OnExit()
        {

            Visual.Skin.material.SetInt("_Hurted", 0);
            Player.InputManager.BlockInput = false;
            Player.Rb.linearVelocity = -Transform.forward;
            IsDamaged = false;
            info = null;
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (Player.YSpeed < 0 && Collision.AirGroundCollision())
            {
                Machine.Set<PS_Move>();
            }
        }

        
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnUpdate()
        {
        }

        public override void OnVisualUpdate()
        {
            base.OnVisualUpdate();
        }
    }

}