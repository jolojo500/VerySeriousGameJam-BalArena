using System;
using UnityEngine;

[Serializable]
public class TargetInputPosInfo : InputHolder
{
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