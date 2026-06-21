//using NaughtyAttributes;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace Venice
{
    public class RWorldObject : RColCallback
    {
        public bool ExposeEvents = false;
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player enters contact")] public UnityEvent<Player> OnEnter = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player stays in contact")] public UnityEvent<Player> OnStay = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player exits contact")] public UnityEvent<Player> OnExit = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player enters collision")] public UnityEvent<Player> OnCEnter = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player stays in collision")] public UnityEvent<Player> OnCStay = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when an object or player exits collision")] public UnityEvent<Player> OnCExit = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a player lands on the object")] public UnityEvent<Player> OnPlayerLand = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a player stands on the object")] public UnityEvent<Player> OnPlayerStand = new UnityEvent<Player>();
        [ShowIf("ExposeEvents")][Tooltip("Event that triggers when a player leaves the ground he is standing on")] public UnityEvent<Player> OnPlayerLeaveGround = new UnityEvent<Player>();

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

        public virtual void EOnEnter(Player player)
        {
            OnEnter.Invoke(player);
        }
        public virtual void EOnStay(Player player)
        {
            OnStay.Invoke(player);
        }
        public virtual void EOnExit(Player player)
        {
            OnExit.Invoke(player);
        }



        public virtual void EOnCEnter(Player player)
        {
            OnCEnter.Invoke(player);
        }
        public virtual void EOnCStay(Player player)
        {
            OnCStay.Invoke(player);
        }
        public virtual void EOnCExit(Player player)
        {
            OnCExit.Invoke(player);
        }
        public virtual void EOnPlayerLand(Player player)
        {
            OnPlayerLand.Invoke(player);
        }
        public virtual void EOnPlayerStand(Player player)
        {
            OnPlayerStand.Invoke(player);
        }

        public virtual void EOnPlayerLeaveGround(Player player)
        {
            OnPlayerLeaveGround.Invoke(player);
        }
    }
}