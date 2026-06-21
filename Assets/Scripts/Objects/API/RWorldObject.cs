//using NaughtyAttributes;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace Venice
{
    public class RWorldObject : RColCallback
    {
        public bool ExposeEvents = false;
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity enters contact")] public UnityEvent<Entity> OnEnter = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity stays in contact")] public UnityEvent<Entity> OnStay = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity exits contact")] public UnityEvent<Entity> OnExit = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity enters collision")] public UnityEvent<Entity> OnCEnter = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity stays in collision")] public UnityEvent<Entity> OnCStay = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity exits collision")] public UnityEvent<Entity> OnCExit = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity lands on the object")] public UnityEvent<Entity> OnEntityLand = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity stands on the object")] public UnityEvent<Entity> OnEntityStand = new UnityEvent<Entity>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity leaves the ground he is standing on")] public UnityEvent<Entity> OnEntityLeaveGround = new UnityEvent<Entity>();

        public Collider Collider;
        public virtual void Awake()
        {
            TOnEnter += (Col) =>
            {
                if (Col.GetComponentInParent<Entity>()) EOnEnter(Col.GetComponentInParent<Entity>());
            };

            TOnStay += (Col) =>
            {
                if (Col.GetComponentInParent<Entity>()) EOnStay(Col.GetComponentInParent<Entity>());
            };

            TOnExit += (Col) =>
            {
                if (Col.GetComponentInParent<Entity>()) EOnExit(Col.GetComponentInParent<Entity>());
            };
            COnEnter += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Entity>()) EOnCEnter(Col.gameObject.GetComponentInParent<Entity>());
            };

            COnStay += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Entity>()) EOnCStay(Col.gameObject.GetComponentInParent<Entity>());
            };

            COnExit += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Entity>()) EOnCExit(Col.gameObject.GetComponentInParent<Entity>());
            };

            Collider = GetComponent<Collider>();
        }

        public virtual void EOnEnter(Entity Entity)
        {
            OnEnter.Invoke(Entity);
        }
        public virtual void EOnStay(Entity Entity)
        {
            OnStay.Invoke(Entity);
        }
        public virtual void EOnExit(Entity Entity)
        {
            OnExit.Invoke(Entity);
        }



        public virtual void EOnCEnter(Entity Entity)
        {
            OnCEnter.Invoke(Entity);
        }
        public virtual void EOnCStay(Entity Entity)
        {
            OnCStay.Invoke(Entity);
        }
        public virtual void EOnCExit(Entity Entity)
        {
            OnCExit.Invoke(Entity);
        }
        public virtual void EOnEntityLand(Entity Entity)
        {
            OnEntityLand.Invoke(Entity);
        }
        public virtual void EOnEntityStand(Entity Entity)
        {
            OnEntityStand.Invoke(Entity);
        }

        public virtual void EOnEntityLeaveGround(Entity Entity)
        {
            OnEntityLeaveGround.Invoke(Entity);
        }
    }
}