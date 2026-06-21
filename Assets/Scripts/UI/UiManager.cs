using System;
using UnityEngine;
using UnityEngine.UI;
using Venice;

public class UIManager : MonoBehaviour
{
    public MeterBar SpotBar;

    void Start()
    {
        Player.Instance.Attributes.OnHealthChanged.AddListener(UpdateHealthBar);
        Player.Instance.Attributes.OnSpinChanged.AddListener(UpdateSpinBar);
        Player.Instance.Attributes.OnSpotLightChanged.AddListener(UpdateSpotBar);
    }

    public void UpdateHealthBar(Tuple<int, int> healthData)
    {
    }
    
    public void UpdateSpinBar(Tuple<int, int>psychoData)
    {
    }
    public void UpdateSpotBar(Tuple<int, int> data)
    {
        SpotBar.SetMaxAmount(data.Item2);

        SpotBar.SetAmount(data.Item1);
    }
}
