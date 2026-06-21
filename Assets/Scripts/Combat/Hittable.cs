using UnityEngine;

namespace Venice
{
    public class Hittable : Entity
    {
        public float KnockbackForce = 14f;
        public float HitCooldown = 0.1f;

        private float _cooldownTimer;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            HandleTimer(ref _cooldownTimer);
        }

        public void Hit(Vector3 sourcePosition)
        {
            if (_cooldownTimer > 0f) return;
            _cooldownTimer = HitCooldown;

            if (_rb != null)
            {
                Vector3 dir = transform.position - sourcePosition;
                dir.y = 0f;
                if (dir != Vector3.zero)
                    _rb.AddForce(dir.normalized * KnockbackForce, ForceMode.Impulse);
            }
        }
    }
}
