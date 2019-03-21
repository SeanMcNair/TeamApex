using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIValueAmount : MonoBehaviour
{
    [SerializeField] protected UIValue valueAmountUI;
    [SerializeField] protected UIValue reloadOverheatUI;

    private void OnEnable()
    {
        ActivateReloadAmount(false);
    }

    public void SetAmountValue(float _value)
    {
        if (valueAmountUI)
            valueAmountUI.SetCurValue(_value);
    }

    public void SetItemAmountMinMaxValue(float _min, float _max)
    {
        if (valueAmountUI)
            valueAmountUI.SetMinMaxValue(_min, _max);
    }

    public void SetReloadAmountValue(float _value)
    {
        if (reloadOverheatUI)
            reloadOverheatUI.SetCurValue(_value);
    }

    public void ActivateReloadAmount(bool _activate)
    {
        if (reloadOverheatUI)
            reloadOverheatUI.gameObject.SetActive(_activate);
    }
}

