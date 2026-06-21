using UnityEngine;
namespace Venice
{
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
        public override void Add(PlayerState state)
        {
            base.Add(state);
            state.Machine = this;
            state.Player = GetComponent<Player>();
            state.OnAddToMachine();
        }

        public override void Set<G>(bool TriggerEnter = true)
        {
            base.Set<G>(TriggerEnter);
        }

        public void Init()
        {
            Add(new PS_Move());
            Add(new PS_Air());
            Add(new PS_Damaged());
            Initialize<PS_Move>();
        }

        public void Update()
        {
            CurrentState?.OnUpdate();
        }

        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }
    }
}