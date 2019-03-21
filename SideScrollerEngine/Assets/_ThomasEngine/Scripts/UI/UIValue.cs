using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIValue : MonoBehaviour
{
    protected float curValue;
    public float CurValue { get { return curValue; } }
    protected float minValue;
    protected float maxValue;

    public virtual void SetCurValue(float _value)
    {
        curValue = _value;
    }

    public virtual void SetMinMaxValue(float _min, float _max)
    {
        minValue = _min;
        maxValue = _max;
    }
}
