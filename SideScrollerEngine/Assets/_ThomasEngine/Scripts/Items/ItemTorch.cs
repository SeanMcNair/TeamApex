using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ItemTorch : ItemAimable
{
    public new ItemTorchData Data { get { return (ItemTorchData)data; } }
    private Light lightSource;
    public float CurLightRange { get { return lightSource.range; } }
    public float CurLightIntensity { get { return lightSource.intensity; } }

    private Coroutine lightDrainRoutine;
    private Coroutine hpDrainRoutine;

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
        SetLightProperties();
    }

    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        if (spawnedOnce)
            SetLightProperties();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!lightSource)
            lightSource = GetComponent<Light>();
    }

    public override void RemoveAmount(float _amount)
    {
        base.RemoveAmount(_amount);
        SetLightProperties();
    }

    public override void AddAmount(float _amount)
    {
        base.AddAmount(_amount);
        SetLightProperties();
    }

    void SetLightProperties()
    {
        if (!lightSource)
            return;
        var perc = ammoAmountLocal.CurAmount / ammoAmountLocal.Size;
        lightSource.range = Mathf.Lerp(0, Data.lightMaxRange, perc);
        lightSource.intensity = Mathf.Lerp(0, ammoAmountLocal.Size, perc);
    }

    public void EnableLightDrain(bool _enable)
    {
        if (_enable)
            lightDrainRoutine = StartCoroutine(StartDrain());
        else if (lightDrainRoutine != null)
            StopCoroutine(lightDrainRoutine);
    }

    public void EnableHealthDrain(bool _enable)
    {
        if (_enable)
            hpDrainRoutine = StartCoroutine(StartHpDrain());
        else if (hpDrainRoutine != null)
            StopCoroutine(hpDrainRoutine);
    }

    IEnumerator StartDrain()
    {
        while (ammoAmountLocal.CurAmount > 0)
        {
            RemoveAmount(Data.lightDrainSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartHpDrain()
    {
        while (lastUnitOwner.CurHP > 0)
        {
            lastUnitOwner.DamageHp(Data.hpDrainSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void AddBuff(ItemBuff _buffToAdd)
    {
        base.AddBuff(_buffToAdd);
        SetLightProperties();
    }

    public override void RemoveBuff(ItemBuff _buffToRemove)
    {
        base.RemoveBuff(_buffToRemove);
        SetLightProperties();
    }

}
