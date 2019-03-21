using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFishInflator : ItemUseable
{
    public new ItemFishInflatorData Data { get { return (ItemFishInflatorData)data; } }
    [SerializeField] private DetectZone detectZone;

    private FishController fishController;
    private Coroutine itemCoroutine;

    private float lastHP;
    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        fishController = curUnitOwner.GetComponent<FishController>();
        lastHP = curUnitOwner.CurHP;
    }

    protected override void OnTriggerItem()
    {
        base.OnTriggerItem();

        lastHP = curUnitOwner.CurHP;
        fishController.Inflate(true);
    }

    protected override void OnRunItem()
    {
        base.OnRunItem();

        if (curUnitOwner.CurHP < lastHP && Data.drainOnHPHit)
        {
            RemoveAmount(ammoAmountLocal.TotalAmount);
            lastHP = curUnitOwner.CurHP;
        }
        else
            RemoveAmount(Data.constantRemoveSpeed * Time.deltaTime);

    }

    public override void StopUseItem()
    {
        base.StopUseItem();
        fishController.Inflate(false);
    }

}
