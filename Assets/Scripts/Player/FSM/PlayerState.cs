using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    public class PlayerState : State
    {
        public int StateNumber = -1;
        public Player Player;
        public PlayerStateMachine Machine;
        public Transform Transform => Player.transform;
        public Rigidbody Rb => Player.Rb;
        public PhysicsInfo PhysicsInfo => Player.PhysicsInfo;
        public PlayerVisual Visual => Player.Visual;
        public PlayerCollision Collision => Player.Collision;
        public PlayerAttributes Attributes => Player.Attributes;

        public NeoInputManager Input => Player.InputManager;
        public bool JumpRequested = false;

        public Vector3 MoveInput => Input.GetAxis2D(GamePreference.MoveInput).xzy().normalized;// new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical")).normalized; //Until better input system i'm lazy
        public Vector3 CalculatedInputs => Quaternion.FromToRotation(Player.PlayerCamera.transform.up, (Attributes.Grounded?Player.SurfaceNormal:Vector3.up)) *
            (Player.PlayerCamera.transform.rotation * MoveInput);

        public PlayerState(int number = -1)
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