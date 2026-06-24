using UnityEngine;

namespace Venice
{
    public class RColCallback : MonoBehaviour
    {
        public Entity Owner;

        public delegate void CollisionEvent(UnityEngine.Collision Col);
        public delegate void TriggerEvent(Collider Col);

        public event CollisionEvent COnEnter, COnStay, COnExit;
        public event TriggerEvent TOnEnter, TOnStay, TOnExit;

        private void Awake()
        {
            if (Owner == null)
                Owner = GetComponentInParent<Entity>();
        }

        public void OnCollisionEnter(UnityEngine.Collision collision) => COnEnter?.Invoke(collision);
        public void OnCollisionStay(UnityEngine.Collision collision) => COnStay?.Invoke(collision);
        public void OnCollisionExit(UnityEngine.Collision collision) => COnExit?.Invoke(collision);

        public void OnTriggerEnter(Collider collider) => TOnEnter?.Invoke(collider);
        public void OnTriggerStay(Collider collider) => TOnStay?.Invoke(collider);
        public void OnTriggerExit(Collider collider) => TOnExit?.Invoke(collider);
    }
}