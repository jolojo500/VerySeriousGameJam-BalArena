using UnityEngine;
namespace Venice
{
    public class BallerinaStateMachine : StateMachine<BallerinaState>
    {
        public override void Add(BallerinaState state)
        {
            base.Add(state);
            state.Machine = this;
            state.Entity = GetComponent<BallerinaEntity>();
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
            Add(new PS_Attack());
            Add(new PS_Pose());
            Add(new PS_Death());
            Add(new PS_Spin());
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