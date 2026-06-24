using System.Collections.Generic;
using UnityEngine;
using Venice;

public class CPUInputManager : NeoInputManager
{
    public BallerinaEntity Entity;
    public AIStateMachine AIMachine;

    [SerializeField] private string aiMapName = "AI";

    private void Awake()
    {
        CurrentMap = aiMapName;
        BlockInput = false;

        Buttons = new List<UButton>
        {
            new UButton { Name = "Jump", MapName = aiMapName },
            new UButton { Name = "Attack", MapName = aiMapName },
            new UButton { Name = "Spin", MapName = aiMapName },
            new UButton { Name = "Dash", MapName = aiMapName }
        };

        Axis1D = new List<UAxis1D>();

        Axis2D = new List<UAxis2D>
        {
            new UAxis2D { Name = "Move", MapName = aiMapName }
        };
    }

    public override UButton UBGet(string name, string map)
    {
        return Buttons.Find(b => b.Name == name);
    }

    public override UAxis1D UA1Get(string name, string map)
    {
        return Axis1D.Find(a => a.Name == name);
    }

    public override UAxis2D UA2Get(string name, string map)
    {
        return Axis2D.Find(a => a.Name == name);
    }
}