using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnitAITrigger
{

    public enum TriggerType { OnEnter, OnExit, OnStay, ValueAmount }
    public TriggerType triggerType;

    public enum UseType { Continuous, SetAmount }
    public UseType useType;
    public int amount;
    private int amountInd;

    public int detectZoneInd;

    //value Amounts
    public ValueAmountManagerData valueAmountManager;
    public int valueAmountInd;
    public enum ValueOptions { Greater, Less, Equal }
    public ValueOptions valueOption;
    public float comparedValue;
    private Coroutine valueRoutine;

    //events
    public UnitAIEvent[] events;

    private Transform senderTrans;
    private bool triggerActive;

    public UnitAI CurAi { get; set; }
    private Unit unit;
    private UnitEquip equip;
    private Collider2D curTarget;

    //gui stuff
    public int currentFieldAmount;

    public void ActivateTriggerDetection(UnitAI _sender, bool _activate)
    {
        CurAi = _sender;
        equip = CurAi.GetComponent<UnitEquip>();
        unit = CurAi.GetComponent<Unit>();
        senderTrans = _sender.transform;

        switch (triggerType)
        {
            case TriggerType.OnEnter:
                DoEnterDetection(_activate);
                break;
            case TriggerType.OnExit:
                DoExitDetection(_activate);
                break;
            case TriggerType.OnStay:
                DoStayDetection(_activate);
                break;
            case TriggerType.ValueAmount:
                DoValueAmountDetection(_activate);
                break;
        }
    }

    void DoEnterDetection(bool _activate)
    {
        if (_activate)
            CurAi.detectZones[detectZoneInd].AddEnterTrigger(CurAi, TriggerEvents);
        else
            CurAi.detectZones[detectZoneInd].RemoveEnterTrigger(CurAi, TriggerEvents);
    }

    void DoExitDetection(bool _activate)
    {
        if (_activate)
            CurAi.detectZones[detectZoneInd].AddExitTrigger(CurAi, TriggerEvents);
        else
            CurAi.detectZones[detectZoneInd].RemoveExitTrigger(CurAi, TriggerEvents);
    }

    void DoStayDetection(bool _activate)
    {
        if (_activate)
            CurAi.detectZones[detectZoneInd].AddStayTrigger(CurAi, TriggerEvents);
        else
            CurAi.detectZones[detectZoneInd].RemoveStayTrigger(CurAi, TriggerEvents);
    }

    void DoValueAmountDetection(bool _activate)
    {
        if (_activate)
        {
            if (valueRoutine != null)
                CurAi.StopCoroutine(valueRoutine);
            valueRoutine = CurAi.StartCoroutine(StartValueAmountDetection());
        }

    }

    IEnumerator StartValueAmountDetection()
    {
        bool valueTriggered = false;
        var valueLocal = unit.GetLocalValue(valueAmountInd);
        while (!valueTriggered)
        {
            var value = valueLocal.TotalAmount;

            valueTriggered = valueOption == ValueOptions.Greater && value > comparedValue ||
                             valueOption == ValueOptions.Equal && value == comparedValue ||
                             valueOption == ValueOptions.Less && value < comparedValue;

            yield return new WaitForEndOfFrame();
        }
        TriggerEvents();
    }

    void TriggerEvents(Collider2D _col = null)
    {
        if (_col)
            curTarget = _col;

        for (int i = 0; i < events.Length; i++)
            events[i].DoEvent(this, i, _col);

        if (useType == UseType.SetAmount)
        {
            amountInd++;
            if (amountInd >= amount - 1)
                ActivateTriggerDetection(CurAi, false);
        }
    }

}