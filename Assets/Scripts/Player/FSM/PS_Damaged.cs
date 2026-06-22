using System;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Venice
{

    public class PS_Damaged : PS_Air
    {
        public HitInfo info;

        
        public float RecoveryTimer = .5f;

        public bool IsDamaged = false;

        public PS_Damaged()
        {
            StateNumber = 2;
        }

        public override void OnEnter()
        {
            Attributes.Grounded = true;
            Attributes.Damaged = true;
            Entity.InputManager.BlockInput = true;
            RecoveryTimer = .5f;
            Vector3 source = (info == null) ? Transform.position + Vector3.forward:info.SourcePosition;
            Vector3 dir = (Transform.position - source).normalized;
            Entity.SetHorizontalVelocity(dir * (info?.KnockbackForce ?? 5));
            Entity.SetVerticalVelocity(Vector3.up * 5f);
            IsDamaged = true;
            Visual.Skin.material.SetInt("_Hurted", 1);
            base.OnEnter();
        }

        public override void OnExit()
        {

            Visual.Skin.material.SetInt("_Hurted", 0);
            Entity.InputManager.BlockInput = false;
            Entity.Rb.linearVelocity = -Transform.forward;
            IsDamaged = false;
            info = null;
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            if (Entity.HandleFixedTimer(ref RecoveryTimer))
            {
                Machine.Set<PS_Air>();
                return;
            }
            base.OnFixedUpdate();
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