using UnityEngine;

public class RunAwayState : CPUAIState
{
    public RunAwayState()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entering RunAway State");
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting RunAway State");
    }
}