using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemUseable : Item
{
    public new ItemUseableData Data { get { return (ItemUseableData)data; } }

    protected ValueAmountLocal ammoAmountLocal = new ValueAmountLocal();
    public ValueAmountLocal AmmoAmountLocal { get { return ammoAmountLocal; } }

    protected bool recoiling;
    public bool IsRecoiling { get { return recoiling; } }
    private float recoilTimer;
    private Coroutine recoilCoroutine;

    public bool IsFireReady { get { return !recoiling && ammoAmountLocal.IsFireReady; } }

    protected bool inUse;
    public bool IsInUse { get { return inUse; } }
    private Coroutine inUseCoroutine;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inUse = false;
        if (curUI)
            Destroy(curUI.gameObject);
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
        LoadLocalValueAmount();
    }

    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        LoadUnitValueAmount();
        if (recoiling)
            Recoil(false);
        ammoAmountLocal.CheckReadyStatus();
    }

    protected override void SetupUI()
    {
        base.SetupUI();
        if (!curUI)
            return;
        ammoAmountLocal.CurUI = curUI;
        ammoAmountLocal.RefreshUI();
        ammoAmountLocal.CheckReadyStatus();
    }

    void LoadLocalValueAmount()
    {
        if (Data.ammoType != ItemUseableData.AmmoType.Local)
            return;

        if (ammoAmountLocal.IsInitialized)
            return;

        ammoAmountLocal = null;

        //need to serialize and copy local data to the item
        ammoAmountLocal = new ValueAmountLocal();
        if (Data.localAmmo.data)
        {
            ammoAmountLocal.CopyValues(Data.localAmmo);
            ammoAmountLocal.Initialize(this);
        }
        else
        {
            Debug.Log("No data found on " + gameObject + " in " + Data);
        }

    }

    void LoadUnitValueAmount()
    {
        if (Data.ammoType != ItemUseableData.AmmoType.Unit)
            return;
        if (ammoAmountLocal.IsInitialized)
            return;

        ammoAmountLocal = null;

        var ind = Data.globalAmmo.indexValue;
        if (dropped)
            ammoAmountLocal = lastUnitOwner.GetLocalValue(ind);
        else
            //get the ammo from the unit
            ammoAmountLocal = curUnitOwner.GetLocalValue(ind);
        if (ammoAmountLocal != null)
            Debug.Log("Loaded " + ammoAmountLocal.valueName + " onto " + gameObject);
        else
            Debug.Log("Could not find local ammo value!");

    }

    public virtual void UseItem()
    {
        if (!Data.overrideUsage)
            DoUseType();
    }

    public virtual void StopUseItem()
    {
        if (!Data.overrideUsage)
            OnStopItem();
    }

    protected virtual void DoUseType()
    {
        inUse = true;
        if (Data.useType == ItemUseableData.UseType.Single)
            UseSingle();
        else
        {
            if (inUseCoroutine != null)
                StopCoroutine(inUseCoroutine);
            if (Data.useType == ItemUseableData.UseType.HoldRepeated)
                inUseCoroutine = StartCoroutine(StartHoldRepeated());
            else if (Data.useType == ItemUseableData.UseType.Constant)
                inUseCoroutine = StartCoroutine(StartConstantUse());
        }
    }

    protected virtual void OnStopItem()
    {
        inUse = false;
        if (ammoAmountLocal != null)
        {
            if (ammoAmountLocal.data)
            {
                if (ammoAmountLocal.data.valueType == ValueAmountData.ValueType.Recharge)
                    ammoAmountLocal.Recharge();
            }

        }

    }

    void UseSingle()
    {
        if (!IsFireReady)
            return;

        OnTriggerItem();
        OnRunItem();
        RemoveAmount(Data.removeAmount);
        Recoil(true);
    }

    IEnumerator StartHoldRepeated()
    {
        if (Data.instantFirstUse)
        {
            yield return new WaitForEndOfFrame();
            UseSingle();
        }
        while (inUse)
        {
            UseSingle();
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartConstantUse()
    {
        if (IsFireReady)
            OnTriggerItem();
        while (inUse)
        {
            if (IsFireReady)
            {
                RemoveAmount(Time.deltaTime * Data.constantRemoveSpeed);
                if (ammoAmountLocal.CurAmount > 0)
                    OnRunItem();
            }
            else if (inUse)
                StopUseItem();
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual void OnTriggerItem()
    {
    }

    protected virtual void OnRunItem()
    {
    }
    public virtual void AddToClipSize(float _amount)
    {
        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;

        ammoAmountLocal.AddToClipSize(_amount);
    }

    public virtual void RemoveMaxValue(float _amount)
    {
        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;

        ammoAmountLocal.RemoveMaxValue(_amount);
    }

    public virtual void AddAmount(float _amount)
    {
        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;

        ammoAmountLocal.AddAmount(_amount);
    }

    public virtual void RemoveAmount(float _amount)
    {
        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;

        ammoAmountLocal.RemoveAmount(_amount);
        if (ammoAmountLocal.IsEmpty)
            OnEmpty();
    }

    void Recoil(bool _reset)
    {
        if (_reset)
            recoilTimer = 0;

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(StartRecoil());
    }

    IEnumerator StartRecoil()
    {
        recoiling = true;
        while (recoilTimer < Data.recoilTime)
        {
            recoilTimer += Time.deltaTime;
            if (recoilTimer > Data.recoilTime)
                recoilTimer = Data.recoilTime;
            yield return new WaitForEndOfFrame();
        }
        recoiling = false;
    }

    public virtual void OnEmpty()
    {
        if (Data.onEmpty == ItemUseableData.OnEmptyOption.Destroy)
        {  
            KillItem();
        }
        else if (Data.onEmpty == ItemUseableData.OnEmptyOption.Drop)
        {
            var equip = curUnitOwner.GetComponent<UnitEquip>();
            equip.DropCurrentItem();
        }
    }

    public virtual void PickUpAmmo(ValueAmountData _ammoData)
    {
        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;

        if (_ammoData != ammoAmountLocal.data)
            return;
        AddAmount(_ammoData.maxSize);
    }

    public override void CopyValues(Item _item)
    {
        base.CopyValues(_item);

        if (Data.ammoType == ItemUseableData.AmmoType.None)
            return;
        var useable = (ItemUseable)_item;
        var localAmmo = useable.ammoAmountLocal;
        if (useable && Data.ammoType == ItemUseableData.AmmoType.Local)
        {
            ammoAmountLocal.CopyValues(localAmmo);
        }

    }

}
