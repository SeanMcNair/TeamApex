using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBuffEnableBehaviour", menuName = "Data/UnitBuffs/UnitBuffEnableBehaviour", order = 1)]
public class UnitBuffEnableBehaviour : UnitBuff
{
    public string behaviourToEnable = "ScriptName";
    public bool enable;

    public override void ActivateBuff(Unit _unit, bool _activate)
    {
        Behaviour behaviour = (Behaviour)_unit.GetComponent(behaviourToEnable);
        if (behaviour)
        {
            behaviour.enabled = enable == _activate;
        }
        else
        {
            Debug.Log("Behaviour: " + behaviourToEnable + " not be found on " + _unit.gameObject.name);
        }
            
    }

}
