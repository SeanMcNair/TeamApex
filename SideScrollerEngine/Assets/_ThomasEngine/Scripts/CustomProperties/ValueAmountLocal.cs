using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueAmountLocal
{
    public string valueName = "ValueName";
    public ValueAmountData data;
    public int maxCount = 1;
    public bool unlimitedTotalAmount;

    private float curSize;
    public float Size { get { return curSize; } }
    private float curAmount;
    public float CurAmount { get { return curAmount; } set { curAmount = value; } }
    private int curInd;
    public int CurIndex { get { return curInd; } }
    private float totalAmount;
    public float TotalAmount { get { return totalAmount; } }

    public bool IsFireReady { get { return !reloading && !overheated && !empty; } }

    private bool reloading;
    public bool IsReloading { get { return reloading; } }
    private float reloadTimer;
    private Coroutine reloadingCoroutine;

    private bool overheated;
    public bool IsOverheated { get { return overheated; } }
    private Coroutine overheatedCoroutine;
    private float overheatTimer;

    private bool recharging;
    public bool IsRecharging { get { return recharging; } }
    private Coroutine rechargeCoroutine;

    private bool empty;
    public bool IsEmpty { get { return empty; } }

    public MonoBehaviour Owner { get; set; }

    private bool initialized;
    public bool IsInitialized { get { return initialized; } }

    public UIValueAmount CurUI { get; set; }

    private UIPlayer playerUI;

    public void CopyValues(ValueAmountLocal _local)
    {
        //public values
        valueName = _local.valueName;
        data = _local.data;
        maxCount = _local.maxCount;
        unlimitedTotalAmount = _local.unlimitedTotalAmount;

        //private
        curSize = _local.Size;
        curAmount = _local.CurAmount;
        curInd = _local.CurIndex;
        totalAmount = _local.totalAmount;
        reloading = _local.IsReloading;
        recharging = _local.IsRecharging;
        empty = _local.IsEmpty;
        if (_local.CurUI)
        {
            CurUI = _local.CurUI;
            RefreshUI();
        }
           
    }

    public void Initialize(MonoBehaviour _owner)
    {
        if (initialized)
            return;

        Owner = _owner;
        Reset();
        initialized = true;
    }

    public void RefreshUI()
    {
        //ui stuff
        if (CurUI)
        {
            CurUI.SetItemAmountMinMaxValue(0, curSize);
            CurUI.SetAmountValue(curAmount);
        }
    }

    public void Reset()
    {
        if (!data)
        {
            Debug.Log("No value amount data found on " + this);
            return;
        }
        curAmount = 0;
        curInd = 0;
        curSize = data.maxSize;
        AddAmount(curSize);
        RefreshUI();
    }



    public void CheckReadyStatus()
    {
        if (reloading)
            Reload();
        if (overheated)
            OverHeat(false);
    }

    public virtual void AddToClipSize(float _amount)
    {
        curSize += _amount;

        if (CurUI)
            CurUI.SetItemAmountMinMaxValue(0, curSize);
    }

    public virtual void RemoveMaxValue(float _amount)
    {
        curSize -= _amount;

        if (CurUI)
            CurUI.SetItemAmountMinMaxValue(0, curSize);
    }

    public virtual void AddAmount(float _amount)
    {
        totalAmount += _amount;
        if (totalAmount < 0)
            totalAmount = 0;
        RefreshClipAmount();
        if (curAmount > curSize)
        {
            if (data.valueType == ValueAmountData.ValueType.Reload && curInd < maxCount - 1)
            {
                curInd++;
            }
            else
                totalAmount = curSize;
        }

        RefreshClipAmount();

        if (CurUI)
            CurUI.SetAmountValue(curAmount);
    }

    public virtual void RemoveAmount(float _amount)
    {
        if (data.valueType == ValueAmountData.ValueType.Unlimited)
            return;

        totalAmount -= _amount;
        if (totalAmount < 0)
            totalAmount = 0;
        RefreshClipAmount();
        if (curAmount <= 0)
        {
            if (data.valueType == ValueAmountData.ValueType.Reload)
            {
                if (totalAmount > 0 || unlimitedTotalAmount)
                    Reload();
            }
            else if (data.valueType == ValueAmountData.ValueType.Recharge && data.rechargeSpeed > 0)
            {
                if (data.overheat)
                    OverHeat(true);
                else
                    Recharge();
            }

        }
        else if (data.valueType == ValueAmountData.ValueType.Recharge && !recharging)
        {
            Recharge();
        }

        //ui
        if (CurUI)
            CurUI.SetAmountValue(curAmount);
    }

    void RefreshClipAmount()
    {
        curAmount = totalAmount - (curSize * curInd);
        empty = totalAmount <= 0;
    }

    public void Recharge()
    {
        if (rechargeCoroutine != null)
            Owner.StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = Owner.StartCoroutine(StartRecharge());
    }

    IEnumerator StartRecharge()
    {
        recharging = true;
        while (curAmount < curSize)
        {
            yield return new WaitForEndOfFrame();
            if (!overheated)
                AddAmount(data.rechargeSpeed * Time.deltaTime);
        }
        recharging = false;
    }

    void OverHeat(bool _reset)
    {
        if (!data.overheat)
            return;
        Debug.Log("overheat");
        if (_reset)
            overheatTimer = 0;
        if (overheatedCoroutine != null)
            Owner.StopCoroutine(overheatedCoroutine);
        overheatedCoroutine = Owner.StartCoroutine(StartOverheat());
    }

    IEnumerator StartOverheat()
    {
        //do UI stuff
        if (CurUI)
            CurUI.ActivateReloadAmount(true);

        overheated = true;
        float perc = 0;
        while (perc < 1)
        {
            overheatTimer += Time.deltaTime;
            if (overheatTimer > data.overheatTime)
                overheatTimer = data.overheatTime;
            perc = overheatTimer / data.overheatTime;

            //do UI stuff
            if (CurUI)
                CurUI.SetReloadAmountValue(perc);

            yield return new WaitForEndOfFrame();
        }
        overheated = false;
        Recharge();

        if (CurUI)
        {
            CurUI.SetAmountValue(curAmount);
            CurUI.ActivateReloadAmount(false);
        }
    }

    void Reload()
    {
        if (unlimitedTotalAmount)
            totalAmount = curSize;
        else if (curInd > 0)
            curInd--;

        if (reloadingCoroutine != null)
            Owner.StopCoroutine(reloadingCoroutine);
        reloadingCoroutine = Owner.StartCoroutine(StartReload());
    }

    IEnumerator StartReload()
    {
        reloading = true;
        float timer = 0;
        float perc = 0;
        //do UI stuff
        if (CurUI)
            CurUI.ActivateReloadAmount(true);
        while (perc < 1)
        {
            timer += Time.deltaTime;
            if (timer > data.reloadTime)
                timer = data.reloadTime;
            perc = timer / data.reloadTime;

            //do UI stuff
            if (CurUI)
                CurUI.SetReloadAmountValue(perc);

            yield return new WaitForEndOfFrame();
        }
        RefreshClipAmount();
        reloading = false;
        //do UI stuff
        if (CurUI)
        {
            CurUI.SetAmountValue(curAmount);
            CurUI.ActivateReloadAmount(false);
        }

    }

}
