using System;
using UnityEngine;
using UnityEngine.UI;
using Venice;

public class UIManager : MonoBehaviour
{
    public MeterBar SpinBar;

    void Start()
    {
        Player.Instance.Attributes.OnHealthChanged.AddListener(UpdateHealthBar);
        Player.Instance.Attributes.OnSpinChanged.AddListener(UpdateSpinBar);
    }

    public void UpdateHealthBar(Tuple<int, int> healthData)
    {
    }
    
    public void UpdateSpinBar(Tuple<int, int>psychoData)
    {
        SpinBar.SetMaxAmount(psychoData.Item2);

        SpinBar.SetAmount(psychoData.Item1);
    }
}
