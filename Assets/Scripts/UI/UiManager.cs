using System;
using UnityEngine;
using UnityEngine.UI;
using Venice;

public class UIManager : MonoBehaviour
{
    public MeterBar SpinBar;       // kick-charge energy
    public MeterBar SuspicionBar;  // audience suspicion

    void Start()
    {
        Venice.Player.Instance.Attributes.OnHealthChanged.AddListener(UpdateHealthBar);
        Venice.Player.Instance.Attributes.OnSpinChanged.AddListener(UpdateSpinBar);
        Venice.Player.Instance.Attributes.OnSuspicionChanged.AddListener(UpdateSuspicionBar);
    }

    public void UpdateHealthBar(Tuple<int, int> healthData)
    {
    }

    public void UpdateSpinBar(Tuple<int, int> data)
    {
        UpdateBar(SpinBar, data);
    }
    public void UpdateSuspicionBar(Tuple<int, int> data)
    {
        UpdateBar(SuspicionBar, data);
    }

    private void UpdateBar(MeterBar bar, Tuple<int, int> data)
    {
        if (bar == null) return;
        bar.SetMaxAmount(data.Item2);
        bar.SetAmount(data.Item1);
    }
}
