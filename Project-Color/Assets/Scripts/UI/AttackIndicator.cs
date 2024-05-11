using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackIndicator : MonoBehaviour
{
    Slider slider;
    Image bar;

    void Start()
    {
        slider = GetComponent<Slider>();
        bar = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (slider.value == 1) bar.color = new Color(0, 0, 255);
        else bar.color = new Color(0, 255, 215);
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
