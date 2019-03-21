using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyController))]
public class EnemyMoveTowards : UnitMoveTowards
{
    //arrival options
    [SerializeField] private bool useItem;
    [SerializeField] private bool repeatIfNotUsing;

    private EnemyController enc;
    private UnitEquip equip;

    public override void Start()
    {
        base.Start();
        
    }

    public override void GetComponents()
    {
        base.GetComponents();
        enc = (EnemyController)controller;
        equip = GetComponent<UnitEquip>();
    }

    public override void MoveTowardsTarget (GameObject _target)
    {
        enc.PausePatrol(true);
        StopAttacking();
        base.MoveTowardsTarget(_target); 
    }

    public override void DoArrivalEvents()
    {
        base.DoArrivalEvents();
        if (useItem)
        {
            if (equip)
            {
                if (!equip.CurItemInUse)
                {
                    equip.UseEquippedItem();
                    if (repeatIfNotUsing)
                        StartCoroutine(StartUseCheck());
                }
                
            }
            
        }  

    }

    IEnumerator StartUseCheck()
    {
        while (arrived)
        {
            yield return new WaitForEndOfFrame();
            if (!equip.CurItemInUse)
                equip.UseEquippedItem(); 
        }
    }

    public override void StopChasing()
    {
        base.StopChasing();
        enc.PausePatrol(false);
    }

    void StopAttacking()
    {
        if (useItem)
        {
            equip.StopUseEquippedItem();
        }
    }
}