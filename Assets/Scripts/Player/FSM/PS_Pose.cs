using System;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Venice
{

    public class PS_Pose : BallerinaState
    {
        public float RecoveryTimer = .8f;
        public float SpamTimer = .5f;

        public PS_Pose() : base(5)
        {
        }

        public override void OnEnter()
        {
            SpamTimer = .5f;
            RecoveryTimer = .8f;
            Attributes.AddToSpin(UnityEngine.Random.Range(5, 15));
            Entity.SetHorizontalVelocity(Vector3.zero);
            Visual.ApplySquashAndStretch(1.1f, .4f);
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            SpamTimer -= Time.fixedDeltaTime;

            if(SpamTimer <= 0)
            {
                if(Input.GetButtonDown(GamePreference.PoseButton) && Attributes.IsInSpotLight)
                {
                    Machine.Set<PS_Pose>();
                    return;
                }
            }
            if (Entity.HandleFixedTimer(ref RecoveryTimer))
            {
                Machine.Set<PS_Move>();
                return;
            }


            if (!Collision.GroundCollision())
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