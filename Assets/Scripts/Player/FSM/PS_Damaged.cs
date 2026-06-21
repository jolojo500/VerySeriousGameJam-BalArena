using System;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Venice
{

    public class PS_Damaged : PlayerState
    {

        
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
            Player.Rb.linearVelocity = Vector3.zero;
            Player.Rb.AddForce(-Transform.forward*40000*Time.deltaTime);
            IsDamaged = true;
            base.OnEnter();
        }

        public override void OnExit()
        {
            
            Player.InputManager.BlockInput = false;
            Player.Rb.linearVelocity = -Transform.forward;
            IsDamaged = false;
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnUpdate()
        {
            if (Player.HandleTimer(ref RecoveryTimer))
            {
                Machine.Set<PS_Move>();

                return;
            }
            base.OnUpdate();

        }

        public override void OnVisualUpdate()
        {
            base.OnVisualUpdate();
        }
    }

}