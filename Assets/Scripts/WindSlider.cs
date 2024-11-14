using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WindSlider : MonoBehaviour
{
    WindZone windZone;
    [SerializeField] Slider slider;
    void Start()
    {
        windZone = GetComponent<WindZone>();
        if(null == slider)
        {
            return;
        }
        slider.onValueChanged.AddListener(UpdateWindStrength);
    }

    private void UpdateWindStrength(float value)
    {
        if (null == windZone)
        {
            return;
        }
        windZone.windMain = value;
    }
}
