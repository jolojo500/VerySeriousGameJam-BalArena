using System;
using UnityEngine;

namespace Rush
{
    [Serializable]
    public class TargetInputPosInfo : InputHolder
    {
        public int TargetDirection;
        public bool TargetPushing;
        public bool TargetBoosting;
        public float TargetSurfaceAngle;
        public Vector3 TargetPosition;

        public TargetInputPosInfo(InputHolder inputHolder)
        {
            Buttons = inputHolder.Buttons;
            Axis1D = inputHolder.Axis1D;
            Axis2D = inputHolder.Axis2D;
        }
        public TargetInputPosInfo() : base()
        {
        }
    }
}