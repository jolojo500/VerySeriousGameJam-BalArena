using System;
using UnityEngine;
using UnityEngine.UI;
using Venice;

public class UIManager : MonoBehaviour
{
    public HealthBar HealthBar;
    public PsychokinesisBar PsychokinesisBar;

    void Start()
    {
        Player.Instance.Attributes.OnHealthChanged.AddListener(UpdateHealthBar);
        Player.Instance.Attributes.OnSpinChanged.AddListener(UpdatePsychoBar);
    }

    public void UpdateHealthBar(Tuple<int, int> healthData)
    {
        HealthBar.SetMaxHealth(healthData.Item2);

        HealthBar.SetHealth(healthData.Item1);
    }
    
    public void UpdatePsychoBar(Tuple<int, int>psychoData)
    {
        PsychokinesisBar.SetMaxPsycho(psychoData.Item2);

        PsychokinesisBar.SetPsycho(psychoData.Item1);
    }
}
