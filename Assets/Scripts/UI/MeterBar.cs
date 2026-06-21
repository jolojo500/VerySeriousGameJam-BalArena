using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MeterBar : MonoBehaviour
{
    public Image slider;
    public Image sliderReactive;
    public bool WasChanged = false;
    private float timeOut;
    private float max;
    private float amount;

    public void SetMaxAmount(int health)
    {
        max = health;
    }
   
    public void SetAmount(int health)
    {
        if (health < amount)
        {
            this.ApplySquashAndStretch(1.1f, .2f);
        }
        amount = health;
        WasChanged = true;
        timeOut = .4f;
    }

    private void Update()
    {
        if (WasChanged)
        {

            if (slider.fillAmount > amount/max)
            {
                slider.fillAmount = Mathf.Max(slider.fillAmount - Time.deltaTime * 5f, amount / max);
                return;
            }

            if (slider.fillAmount < amount / max)
            {
                slider.fillAmount = Mathf.Min(slider.fillAmount + Time.deltaTime * 5f, amount / max);
                sliderReactive.fillAmount = slider.fillAmount;
                return;
            }
            if (timeOut > 0f)
            {
                timeOut -= Time.deltaTime;
                return;
            }

            if(sliderReactive.fillAmount > slider.fillAmount)
            {
                sliderReactive.fillAmount = Mathf.Max(sliderReactive.fillAmount - Time.deltaTime * 2f, slider.fillAmount);
            }
            else
            {
                sliderReactive.fillAmount = slider.fillAmount;
                WasChanged = false;
            }

        }
    }
}
