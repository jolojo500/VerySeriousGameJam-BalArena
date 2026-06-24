using System;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public class HitInfo
    {
        public BallerinaEntity SourceEntity;
        public Vector3 SourcePosition;
        public float KnockbackForce;
        public int Damage;
        public float HitCooldown;
        public GameObject Owner;
        public HitInfo()
        {
        }
        public HitInfo(Vector3 sourcePosition, float knockbackForce, float hitCooldown, GameObject owner)
        {
            SourcePosition = sourcePosition;
            KnockbackForce = knockbackForce;
            HitCooldown = hitCooldown;
            Owner = owner;

            if (owner != null)
                SourceEntity = owner.GetComponentInParent<BallerinaEntity>();
        }
    }

    public class Hittable : MonoBehaviour
    {
        public float KnockbackForce = 14f;

        private Rigidbody _rb;
        private BallerinaEntity _targetEntity;

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody>();
            _targetEntity = GetComponentInParent<BallerinaEntity>();
        }

        public void Hit(HitInfo hitInfo)
        {
            BallerinaEntity target = GetComponent<BallerinaEntity>();
            BallerinaEntity attacker = hitInfo.SourceEntity;

            Debug.Log(
                $"Hit: attacker={attacker?.name}, attackerTeam={attacker?.Team}, " +
                $"target={target?.name}, targetTeam={target?.Team}"
            );

            if (attacker != null && target != null)
            {
                if (attacker == target)
                {
                    Debug.Log("Blocked self-hit.");
                    return;
                }

                if (attacker.Team == target.Team)
                {
                    Debug.Log("Blocked friendly fire.");
                    return;
                }
            }

            SendMessage("OnHit", hitInfo, SendMessageOptions.RequireReceiver);
        }

        private void ApplyBasicKnockback(HitInfo hitInfo)
        {
            if (_rb == null)
                return;

            Vector3 dir = transform.position - hitInfo.SourcePosition;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                _rb.AddForce(dir.normalized * KnockbackForce, ForceMode.Impulse);
        }
    }
}