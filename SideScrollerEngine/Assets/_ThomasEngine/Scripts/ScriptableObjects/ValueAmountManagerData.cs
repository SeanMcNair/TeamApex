using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ValueAmountManager", menuName = "Data/Managers/ValueAmountManager", order = 1)]
public class ValueAmountManagerData : ScriptableObject
{
    public ValueAmountLocal[] valueAmountLocals;

    public string[] GetValueNames()
    {
        var names = new string[valueAmountLocals.Length];
        for (int i = 0; i < valueAmountLocals.Length; i++)
        {
            names[i] = valueAmountLocals[i].valueName;
        }
        return names;
    }
}
