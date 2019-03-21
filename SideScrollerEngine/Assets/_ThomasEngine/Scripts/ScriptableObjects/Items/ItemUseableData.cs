using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseableData : ItemData
{
    public enum AmmoType { None, Local, Unit }
    public enum UseType { Single, HoldRepeated, Constant }
    public enum OnEmptyOption { Nothing, Destroy, Drop }

    public bool overrideUsage;
    public UseType useType;
    public float removeAmount = 1;
    public float recoilTime = 0.3f;
    public bool instantFirstUse;
    public float constantRemoveSpeed = 1;

    public AmmoType ammoType;
    public ValueAmountLocal localAmmo;
    public ValueAmountManagerData globalAmmoManager;
    public IndexStringProperty globalAmmo;

    public OnEmptyOption onEmpty;


}
