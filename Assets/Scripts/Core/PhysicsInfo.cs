using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public struct CapsuleSize
    {
        public Vector2 size;
        public Vector2 offset;
    }

    [CreateAssetMenu(fileName = "PhysicsInfo", menuName = "Neo/ScriptableObjects/PhysicsInfo", order = 1)]
    public class PhysicsInfo : ScriptableObject
    {
        [Header("Player Physics")]
        public float Acceleration = 15f;
        public float Deceleration = 30f;
        public float Friction = 40f;
        public float MaxSpeed = 9.7f;
        public float TopSpeed = 35f;


        public float SlopeRepelDownHill = 70f;
        public float SlopeRepelUpHill = 30f;

        public float TurnRate = 2.6f;
        public float SpeedLoss = 4f;

        public AnimationCurve TurnRateCurve;
        public AnimationCurve SpeedLossCurve;

        public float AirAcceleration = 15f;
        public float AirDeceleration = 80f;
        public float AirDrag = 60f;
        public int JumpAmount = 2;
        public float Gravity = 20f;
        public float JumpStrength = 11f;
        public float DoubleJumpMultiplier = 0.6f;
        public float JumpCutoff = 4f;
        public float MaxFallSpeed = 20f;
        public float CoyoteTime = 0.15f;




        public float FloatAcceleration = 15f;
        public float FloatDeceleration = 80f;
        public float FloatDrag = 10f;
        public float MinFloatSpeed = 1f;
        public float MaxFloatSpeed = 6f;
        public float FloatTurnRate = 2.6f;
        public AnimationCurve FloatTurnRateCurve;

        public float FloatDashAcceleration = 150f;
        public float FloatDashMaxSpeed = 20f;
        public float FloatDashTurnRateMultiplier = .15f;
        public float FloatDashESPDrainRate = 10f;

    }
}