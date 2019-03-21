using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(UnitEquip))]
public class EnemyAttack : DetectZoneTrigger
{
    private UnitEquip equip;
    private EnemyController enc;

    private void Start()
    {
        enc = GetComponent<EnemyController>();
        equip = GetComponent<UnitEquip>();
    }

    protected override void OnEnter(Collider2D _col)
    {
        base.OnEnter(_col);
        var unit = _col.GetComponent<Unit>();
        if (unit)
            enc.Target = unit.AttackTarget;
        equip.UseEquippedItem();
    }

    protected override void OnExit(Collider2D _col)
    {
        base.OnExit(_col);
        equip.StopUseEquippedItem();
        enc.Target = null;
    }
}