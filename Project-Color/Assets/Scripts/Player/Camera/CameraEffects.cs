using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    ShaderEffect_CorruptedVram[] shaderEffects;
    public float shift = 0;

    public float shiftMin = 0.5f;
    public float shiftMax = 2;
    public float minTime = 0.01f;
    public float maxTime = 0.1f;

    float timer = 0;
    float timer1 = 0;
    float timer2 = 0;

    private void OnValidate()
    {
        shaderEffects = GetComponents<ShaderEffect_CorruptedVram>();
        SetEffect();
    }

    void Start()
    {
        shaderEffects = GetComponents<ShaderEffect_CorruptedVram>();
    }

    void Update()
    {
        if (GameController.paused)
        {
            EffecsEnabled(true);
            timer1 -= Time.unscaledDeltaTime;
            timer2 -= Time.unscaledDeltaTime;
            System.Random rand = new System.Random();
            if (timer1 < 0)
            {
                SetEffect1Random();
                timer1 = (float)rand.NextDouble() / 3;
            }
            if (timer2 < 0)
            {
                SetEffect2Random();
                timer2 = (float)rand.NextDouble() / 3;
            }
        }
        else
        {
            EffecsEnabled(false);
            timer1 = 0;
        }
    }

    void SetEffect()
    {
        if (shift == 0)
        {
            shaderEffects[0].enabled = false;
            shaderEffects[1].enabled = false;
        }
        else
        {
            shaderEffects[0].enabled = true;
            shaderEffects[1].enabled = true;
            shaderEffects[0].shift = shift;
            shaderEffects[1].shift = -shift;
        }
    }
    
    void SetEffect1Random()
    {
        float _shift = Random.Range(shiftMin, shiftMax);
        shaderEffects[0].shift = _shift;
    }
    void SetEffect2Random()
    {
        float _shift = Random.Range(shiftMin, shiftMax);
        shaderEffects[1].shift = -_shift;
    }

    private void EffecsEnabled(bool enabled)
    {
        shaderEffects[0].enabled = enabled;
        shaderEffects[1].enabled = enabled;
    }
}
