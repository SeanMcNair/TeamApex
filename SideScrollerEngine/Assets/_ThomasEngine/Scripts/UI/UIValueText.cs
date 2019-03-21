using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueText : UIValue
{
    [SerializeField] private Text text;

    public override void SetCurValue(float _value)
    {
        base.SetCurValue(_value);
        text.text = curValue.ToString();
    }
}
