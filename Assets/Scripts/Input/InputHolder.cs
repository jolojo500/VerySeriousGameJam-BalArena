using System;
using UnityEngine;
using Venice;

public class InputHolder
{
    public bool BlockInputs = false;

    public UButton[] Buttons { get; set; } = new UButton[0];
    public UAxis1D[] Axis1D { get; set; } = new UAxis1D[0];
    public UAxis2D[] Axis2D { get; set; } = new UAxis2D[0];

    public void ClearInputs()
    {
        //Really not good but you understand what i'm doing.
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i] = new UButton
            {
                Name = Buttons[i].Name,
                hold = Buttons[i].hold,
            };
        }
        for (int i = 0; i < Axis1D.Length; i++)
        {
            Axis1D[i] = new UAxis1D
            {
                Name = Axis1D[i].Name,
                Value = 0
            };
        }

        for (int i = 0; i < Axis2D.Length; i++)
        {
            Axis2D[i] = new UAxis2D
            {
                Name = Axis2D[i].Name,
                Value = Vector2.zero
            };
        }
    }

    public bool GetAnyDown() => (Array.Find(Buttons, s => s.pressed == true)?.pressed) ?? false;
    public UButton UBGet(string name) => Array.Find(Buttons, s => s.Name == name) ?? new UButton();
    public UAxis1D UA1Get(string name) => Array.Find(Axis1D, s => s.Name == name) ?? new UAxis1D();
    public UAxis2D UA2Get(string name) => Array.Find(Axis2D, s => s.Name == name) ?? new UAxis2D();
    public bool GetButtonDown(string name) { return !BlockInputs && UBGet(name).pressed; }
    public bool GetButtonUp(string name) { return !BlockInputs && UBGet(name).released; ; }
    public bool GetButton(string name) { return !BlockInputs && UBGet(name).hold; }

    public float GetAxis(string name) { return BlockInputs ? 0 : UA1Get(name).Value; }
    public Vector2 GetAxis2D(string name) { return BlockInputs ? Vector2.zero : UA2Get(name).Value; }

    public InputHolder GetInputList()
    {


        UButton[] copiedButtons = new UButton[Buttons.Length];
        for (int i = 0; i < Buttons.Length; i++)
        {
            copiedButtons[i] = new UButton
            {
                Name = Buttons[i].Name,
                hold = Buttons[i].hold,
                PressedTime = Buttons[i].PressedTime,
                FPressedTime = Buttons[i].FPressedTime,
                ReleasedTime = Buttons[i].ReleasedTime,
                FReleasedTime = Buttons[i].FReleasedTime
            };
        }
        UAxis1D[] copiedAxis1D = new UAxis1D[Axis1D.Length];
        for (int i = 0; i < Axis1D.Length; i++)
        {
            copiedAxis1D[i] = new UAxis1D
            {
                Name = Axis1D[i].Name,
                Value = Axis1D[i].Value
            };
        }
        UAxis2D[] copiedAxis2D = new UAxis2D[Axis2D.Length];
        for (int i = 0; i < Axis2D.Length; i++)
        {
            copiedAxis2D[i] = new UAxis2D
            {
                Name = Axis2D[i].Name,
                Value = Axis2D[i].Value
            };
        }

        return new InputHolder
        {
            Buttons = copiedButtons,
            Axis1D = copiedAxis1D,
            Axis2D = copiedAxis2D
        };
    }
}