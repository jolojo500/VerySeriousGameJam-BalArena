using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : StateMachine<CPUAIState>
{
    private void Start()
    {
        Add(new StandByState());
        Add(new HostileState());
        Add(new RunAwayState());
        Initialize<StandByState>();
    }
    public override void Add(CPUAIState state)
    {
        base.Add(state);
        state.AIMachine = this;
        state.FollowerCPU = GetComponent<CPUInputManager>();
    }



    public void Update()
    {
        CurrentState?.OnUpdate();
    }


    public void LateUpdate()
    {
        
    }


    public void FixedUpdate()
    {
        
    }

}
