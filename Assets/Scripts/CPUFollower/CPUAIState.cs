
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUAIState:State
{
    public CPUInputManager FollowerCPU;
    public AIStateMachine AIMachine;
    public Entity Entity => FollowerCPU.Entity;

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public bool GetButtonDown(string name) => FollowerCPU.GetButtonDown(name);
    public bool GetButtonUp(string name) => FollowerCPU.GetButtonUp(name);
    public bool GetButton(string name) => FollowerCPU.GetButton(name);

    public float GetAxis(string name) => FollowerCPU.GetAxis(name);
    public Vector2 GetAxis2D(string name) => FollowerCPU.GetAxis2D(name);

}
