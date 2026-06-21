using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    [RequireComponent(typeof(Collider))]
    public class AttackHitbox : MonoBehaviour
    {
        public GameObject Owner;

        public bool Active { get; private set; }

        private Collider _collider;
        private readonly HashSet<Hittable> _hitThisSwing = new HashSet<Hittable>();

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        public void Begin()
        {
            _hitThisSwing.Clear();
            Active = true;
            if (_collider != null) _collider.enabled = true;
        }

        public void End()
        {
            Active = false;
            if (_collider != null) _collider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Active) return;

            var target = other.GetComponentInParent<Hittable>();
            if (target == null) return;

            if (Owner != null && target.transform.IsChildOf(Owner.transform)) return;

            if (!_hitThisSwing.Add(target)) return;

            target.Hit(transform.position);
        }
    }
}
