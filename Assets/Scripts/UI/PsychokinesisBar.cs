using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PsychokinesisBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxPsycho(int psycho)
    {
        slider.maxValue = psycho;
        slider.value = psycho;
    }
   
    public void SetPsycho(int psycho)
    {
        slider.value = psycho;
    }
}
