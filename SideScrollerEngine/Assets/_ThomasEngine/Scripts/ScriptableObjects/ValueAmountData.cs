using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ValueAmount", menuName = "Data/ValueAmount", order = 1)]
public class ValueAmountData : ScriptableObject
{
    public enum ValueType { Unlimited, Limited, Reload, Recharge }

    public ValueType valueType;
    public int maxSize = 1;
    public float reloadTime;
    public float rechargeSpeed = 1;
    public bool overheat;
    public float overheatTime;
}
