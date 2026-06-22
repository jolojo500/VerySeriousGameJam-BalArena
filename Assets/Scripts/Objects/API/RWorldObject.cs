//using NaughtyAttributes;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace Venice
{
    public class RWorldObject : RColCallback
    {
        public bool ExposeEvents = false;
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity enters contact")] public UnityEvent<Player> OnEnter = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity stays in contact")] public UnityEvent<Player> OnStay = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity exits contact")] public UnityEvent<Player> OnExit = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity enters collision")] public UnityEvent<Player> OnCEnter = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity stays in collision")] public UnityEvent<Player> OnCStay = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or Entity exits collision")] public UnityEvent<Player> OnCExit = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity lands on the object")] public UnityEvent<Player> OnEntityLand = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity stands on the object")] public UnityEvent<Player> OnEntityStand = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a Entity leaves the ground he is standing on")] public UnityEvent<Player> OnEntityLeaveGround = new UnityEvent<Player>();

        public Collider Collider;
        public virtual void Awake()
        {
            TOnEnter += (Col) =>
            {
                if (Col.GetComponentInParent<Player>()) EOnEnter(Col.GetComponentInParent<Player>());
            };

            TOnStay += (Col) =>
            {
                if (Col.GetComponentInParent<Player>()) EOnStay(Col.GetComponentInParent<Player>());
            };

            TOnExit += (Col) =>
            {
                if (Col.GetComponentInParent<Player>()) EOnExit(Col.GetComponentInParent<Player>());
            };
            COnEnter += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Player>()) EOnCEnter(Col.gameObject.GetComponentInParent<Player>());
            };

            COnStay += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Player>()) EOnCStay(Col.gameObject.GetComponentInParent<Player>());
            };

            COnExit += (Col) =>
            {
                if (Col.gameObject.GetComponentInParent<Player>()) EOnCExit(Col.gameObject.GetComponentInParent<Player>());
            };

            Collider = GetComponent<Collider>();
        }

        public virtual void EOnEnter(Player Entity)
        {
            OnEnter.Invoke(Entity);
        }
        public virtual void EOnStay(Player Entity)
        {
            OnStay.Invoke(Entity);
        }
        public virtual void EOnExit(Player Entity)
        {
            OnExit.Invoke(Entity);
        }



        public virtual void EOnCEnter(Player Entity)
        {
            OnCEnter.Invoke(Entity);
        }
        public virtual void EOnCStay(Player Entity)
        {
            OnCStay.Invoke(Entity);
        }
        public virtual void EOnCExit(Player Entity)
        {
            OnCExit.Invoke(Entity);
        }
        public virtual void EOnEntityLand(Player Entity)
        {
            OnEntityLand.Invoke(Entity);
        }
        public virtual void EOnEntityStand(Player Entity)
        {
            OnEntityStand.Invoke(Entity);
        }

        public virtual void EOnEntityLeaveGround(Player Entity)
        {
            OnEntityLeaveGround.Invoke(Entity);
        }
    }
}