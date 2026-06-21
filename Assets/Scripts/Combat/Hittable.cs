using System;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public class HitInfo
    {
        public Vector3 SourcePosition;
        public float KnockbackForce;
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
        }
    }
    public class Hittable : MonoBehaviour
    {
        public float KnockbackForce = 14f;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
        }

        public void Hit(HitInfo hitInfo)
        {
            Debug.Log("test");
            try
            {
                SendMessage("OnHit", hitInfo, SendMessageOptions.RequireReceiver);

            }catch(Exception e)
            {
                if (_rb != null)
                {
                    Vector3 dir = (transform.position - hitInfo.SourcePosition).normalized;
                    dir.y = 0f;
                    if (dir != Vector3.zero)
                        _rb.AddForce(dir.normalized * KnockbackForce, ForceMode.Impulse);
                }
            }

        }
    }
}
