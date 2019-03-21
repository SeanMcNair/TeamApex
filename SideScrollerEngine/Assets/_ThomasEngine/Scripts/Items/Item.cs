using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Linkable
{
    public new ItemData Data { get { return (ItemData)data; } }
    protected bool dropped;
    public bool IsDropped { get { return dropped; } }
    protected Unit curUnitOwner;
    public Unit UnitOwner { get { return curUnitOwner; } }
    protected Unit lastUnitOwner;
    public Unit LastUnitOwner { get { return lastUnitOwner; } }
    protected UIValueAmount curUI;
    public GameObject CurUIObject { get { return curUI.gameObject; } }
    public GameObject GetCurUIObject() { if (curUI) return curUI.gameObject; return null; }

    protected List<ItemBuff> curBuffs = new List<ItemBuff>();
    public List<ItemBuff> CurBuffs { get { return curBuffs; } }

    protected bool spawnedOnce;
    public bool SpawnedOnce { get { return spawnedOnce; } }

    protected virtual void Awake()
    {
        //
    }

    protected virtual void Start()
    {
        //
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(StartWaitForOwner());
    }

    protected virtual void OnDisable()
    {
        if (curUI)
            Destroy(curUI.gameObject);
        if (curUnitOwner)
            ActivateAllBuffs(false);
    }

    IEnumerator StartWaitForOwner()
    {
        while (!curUnitOwner && !dropped)
        {
            yield return new WaitForEndOfFrame();
        }
        OnOwnerFound();
        if (!spawnedOnce)
            LoadDefaultData();
        if (!dropped)
            ActivateAllBuffs(true);
        SetupUI();
    }

    protected virtual void OnOwnerFound()
    {
    }

    protected virtual void SetupUI()
    {
        if (dropped && !Data.keepUIActiveIfDropped)
            return;

        if (!curUI && Data.spawnUI)
        {
            curUI = Instantiate(Data.itemUI).GetComponent<UIValueAmount>();
            if (Data.setOwnerAsParent)
            {
                var trans = transform;
                if (curUnitOwner)
                    trans = curUnitOwner.transform;
                curUI.transform.position = trans.position;
                curUI.transform.rotation = Quaternion.identity;
                curUI.transform.SetParent(trans);
            }

        }
    }

    public virtual void LoadDefaultData()
    {
        if (dropped)
            return;
        spawnedOnce = true;
        curBuffs = new List<ItemBuff>();
        curBuffs.AddRange(Data.buffs);
    }

    public virtual void SetOwner(Unit _unit)
    {
        curUnitOwner = _unit;

        if (curUnitOwner)
            lastUnitOwner = curUnitOwner;
    }

    public virtual void CopyValues(Item _item)
    {
        spawnedOnce = _item.SpawnedOnce;
        curBuffs.Clear();
        curBuffs.AddRange(_item.CurBuffs);
        if (_item.LastUnitOwner)
            lastUnitOwner = _item.LastUnitOwner;
    }

    public virtual void PickUp()
    {
        dropped = false;
    }

    public virtual void Drop()
    {
        dropped = true;
    }

    public virtual void AddBuff(ItemBuff _buffToAdd)
    {
        _buffToAdd.ActivateBuff(this, true);
        curBuffs.Add(_buffToAdd);
    }

    public virtual void RemoveBuff(ItemBuff _buffToRemove)
    {
        if (!curBuffs.Contains(_buffToRemove))
            return;

        _buffToRemove.ActivateBuff(this, false);
        curBuffs.Remove(_buffToRemove);
    }

    public virtual void ActivateAllBuffs(bool _activate)
    {
        if (!(curBuffs.Count > 0))
            return;

        foreach (var buff in curBuffs)
        {
            buff.ActivateBuff(this, _activate);
        }
    }

    public virtual void KillItem()
    {
        if (curUnitOwner)
        {
            var equip = curUnitOwner.GetComponent<UnitEquip>();
            equip.RemoveItem(Data);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
