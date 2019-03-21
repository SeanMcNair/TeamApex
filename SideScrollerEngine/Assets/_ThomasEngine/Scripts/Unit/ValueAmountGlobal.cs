using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueAmountGlobal : MonoBehaviour
{
    [SerializeField] private ValueAmountManagerData managerData;
    public ValueAmountManagerData ManagerData { get { return managerData; } set { managerData = value; } }

    private ValueAmountLocal[] valueLocals;

    private void Awake()
    {
        if (managerData)
            LoadValueData();
    }

    public void LoadValueData()
    {
        //initialize array size
        var size = managerData.valueAmountLocals.Length;
        valueLocals = new ValueAmountLocal[size];
        //serialize or "copy" the local values from the data to this script.
        for (int i = 0; i < size; i++)
        {
            var localAmmo = managerData.valueAmountLocals[i];
            valueLocals[i] = new ValueAmountLocal();
            valueLocals[i].CopyValues(localAmmo);
        }
    }

    public ValueAmountLocal GetLocalValue(int _ind)
    {
        return valueLocals[_ind];
    }
}
