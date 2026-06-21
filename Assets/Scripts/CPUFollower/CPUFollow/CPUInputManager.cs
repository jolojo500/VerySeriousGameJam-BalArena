using Rush;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Venice;




public class CPUInputManager : NeoInputManager
{

    public NeoInputManager RealInput;
    public AIStateMachine AIMachine;
    public TargetInputPosInfo target_inputs;


    public override bool GetButtonDown(string name) { return target_inputs == null ? false : target_inputs.GetButtonDown(name); }
    public override bool GetButtonUp(string name) { return target_inputs == null ? false : target_inputs.GetButtonUp(name); }
    public override bool GetButton(string name) { return target_inputs == null ? false : target_inputs.GetButton(name); }

    public override float GetAxis(string name) { return target_inputs == null ? 0f : target_inputs.GetAxis(name); }
    public override Vector2 GetAxis2D(string name) { return target_inputs == null ? Vector2.zero : target_inputs.GetAxis2D(name); }


}








