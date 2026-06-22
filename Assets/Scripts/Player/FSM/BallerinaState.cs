using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    public class BallerinaState : State
    {
        public int StateNumber = -1;
        public BallerinaEntity Entity;
        public BallerinaStateMachine Machine;
        public Transform Transform => Entity.transform;
        public Rigidbody Rb => Entity.Rb;
        public PhysicsInfo PhysicsInfo => Entity.PhysicsInfo;
        public BallerinaVisual Visual => Entity.Visual;
        public EntityCollision Collision => Entity.Collision;
        public BallerinaAttributes Attributes => Entity.Attributes;

        public NeoInputManager Input => Entity.InputManager;
        public bool JumpRequested = false;

        public Vector3 MoveInput => Input.GetAxis2D(GamePreference.MoveInput).xzy().normalized;// new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical")).normalized; //Until better input system i'm lazy
        public Vector3 CalculatedInputs => Quaternion.FromToRotation(Camera.main.transform.up, (Attributes.Grounded?Entity.SurfaceNormal:Vector3.up)) *
            (Camera.main.transform.rotation * MoveInput);

        public BallerinaState(int number = -1)
        {
            StateNumber = number;
        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }
        public override void OnUpdate()
        {
            if (Input.GetButtonDown(GamePreference.JumpButton))
            {
                JumpRequested = true;
            }
        }


    }
}