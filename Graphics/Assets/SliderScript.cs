using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider slider;
    public float maxValue;

    public void setMaxValue(float value)
    {
        maxValue = value;
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }
    public void setValue(float value)
    {
        if (value < 0) value = 0;
        if (value > maxValue) value = 100;

        slider.value = value;
    }
}
