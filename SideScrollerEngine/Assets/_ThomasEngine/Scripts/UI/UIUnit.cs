using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour
{
    [SerializeField] protected Image avatarImage;
    [SerializeField] protected ValueAmountManagerData vitalManagerData;
    [SerializeField] protected UIValueAmount[] vitalsUI;
    [SerializeField] protected UIValueAmount hp;
    public UIValueAmount HpUI { get { return hp; } }
    [SerializeField] protected UIValueAmount mana;
    public UIValueAmount ManaUI { get { return mana; } }
    [SerializeField] protected UIValueAmount stamina;
    public UIValueAmount StaminaUI { get { return stamina; } }

    public virtual void SetAvatarIcon(Sprite _icon)
    {
        avatarImage.sprite = _icon;
    }

    public virtual void SetHPValue(float _value)
    {
        hp.SetAmountValue(_value);
    }

    public virtual void SetManaValue(float _value)
    {
        mana.SetAmountValue(_value);
    }

    public virtual void SetStaminaValue(float _value)
    {
        stamina.SetAmountValue(_value);
    }

    public UIValueAmount GetValueAmountUI(int _ind)
    {
        return vitalsUI[_ind];
    }
}
