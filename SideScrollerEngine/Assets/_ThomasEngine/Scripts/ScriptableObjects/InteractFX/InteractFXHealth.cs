using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Data/Interacts/Health", order = 1)]
public class InteractFXHealth : InteractFX
{

    [SerializeField] private int hpDelta;

    public override void DoFX(GameObject _sender, GameObject _receiver)
    {
        Unit unit = _receiver.GetComponent<Unit>();
        if (hpDelta < 0)
            unit.DamageHp(Mathf.Abs(hpDelta));
        else
            unit.AddHp(hpDelta);
    }
}
