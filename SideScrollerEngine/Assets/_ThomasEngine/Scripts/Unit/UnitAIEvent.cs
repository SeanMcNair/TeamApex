using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnitAIEvent
{
    public enum StartType { Instant, WaitForPreviousToFinish }
    public StartType startType;

    //event types...the names are in the unit ai container property drawer
    public int eventTypeMask;

    //state event
    public enum StateType { MoveToTarget, FleeFromTarget, PauseMovement, DefaultState }
    public StateType stateType;

    //use items...unit equip must be attached
    public enum ItemEventType { UseItem, EquipItem, UnEquipItem, DropItem}
    public ItemEventType itemEventType;
    public int item;

    //anim
    public bool setAnimator;
    public Animator animator;
    public string stateToPlay;
    public float crossfadeTime;
    //method
    public MethodProperty methodToCall;
    //interacts
    public InteractFX[] interacts;

    public int detectZoneInd;
    private List<Collider2D> cols = new List<Collider2D>();
    private List<Collider2D> enteredCols = new List<Collider2D>();

    //common values
    public enum FinishType { OnEventSent, Timed}
    public float delay;
    public bool repeat;
    public float repeatTime;
    public FinishType finishType;
    public float totalTime;
    public bool finished;

    //value Amounts
    public ValueAmountManagerData valueAmountManager;
    public int valueAmountInd;
    public enum ValueOptions { Greater, Less, Equal }
    public ValueOptions valueOption;
    public float comparedValue;
    private Coroutine valueRoutine;

    private Transform senderTrans;
    private bool eventRunning;

    private int index;
    private UnitAI curAI;
    private Unit unit;
    private UnitEquip equip;
    private Collider2D curTarget;

    public void DoEvent(UnitAITrigger _trigger, int _index, Collider2D _col = null)
    {
        curAI = _trigger.CurAi;
        index = _index;
        if (_col)
            curTarget = _col;

        if (startType == StartType.Instant)
            curAI.StartCoroutine(StartTriggerEvent());
        else if (startType == StartType.WaitForPreviousToFinish && index > 0)
            curAI.StartCoroutine(StartWaitForPrevious(_trigger));
    }

    IEnumerator StartWaitForPrevious(UnitAITrigger _trigger)
    {
        var lastEvent = _trigger.events[index - 1];
        while (!lastEvent.finished)
        {
            yield return new WaitForEndOfFrame();
        }
        curAI.StartCoroutine(StartTriggerEvent());
    }

    IEnumerator StartTriggerEvent()
    {
        eventRunning = true;
        bool eventSent = false;
        float delayTimer = 0;
        float repeatTimer = 0;
        float totalTimer = 0;
        while (!finished)
        {
            if (finishType == FinishType.Timed)
            {
                totalTimer += Time.deltaTime;
                if (totalTimer > totalTime)
                {
                    totalTimer = totalTime;
                    finished = true;
                }
                    
            }
            
            delayTimer += Time.deltaTime;
            if (delayTimer > delay && !eventSent)
            {
                delayTimer = delay;
                DoEventSwitch();
                eventSent = true;
                if (finishType == FinishType.OnEventSent)
                    finished = true;
            }
            
            if (repeat && eventSent)
            {
                repeatTimer += Time.deltaTime;
                if (repeatTimer > repeatTime)
                {
                    repeatTimer = 0;
                    DoEventSwitch();
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void DoEventSwitch()
    {
        if (eventTypeMask == (eventTypeMask | (1 << 0)))
            DoStateEvent();
        if (eventTypeMask == (eventTypeMask | (1 << 1)))
            DoItemEvent();
        if (eventTypeMask == (eventTypeMask | (1 << 2)))
            DoPlayAnimation();
        if (eventTypeMask == (eventTypeMask | (1 << 3)))
            DoCallMethod();
        if (eventTypeMask == (eventTypeMask | (1 << 4)))
            DoInteractFX();
    }

    void DoStateEvent()
    {
        if (stateType == StateType.DefaultState)
            curAI.DoDefaultState();
        else if (curTarget)
        {
            if (stateType == StateType.MoveToTarget)
                curAI.MoveToTarget(curTarget.transform);
            else if (stateType == StateType.FleeFromTarget)
                curAI.FleeFromTarget(curTarget.transform);
        }

    }

    void DoItemEvent()
    {
        if (!equip)
            return;

        switch (itemEventType)
        {
            case ItemEventType.UseItem:
                DoUseItem();
                break;
            case ItemEventType.EquipItem:
                DoEquipItem();
                break;
            case ItemEventType.UnEquipItem:
                DoUnequipItem();
                break;
            case ItemEventType.DropItem:
                DoDropItem();
                break;
        }
    }

    void DoUseItem()
    {
        SetItem();
        equip.UseEquippedItem();
    }

    void DoEquipItem()
    {
        SetItem();
        equip.EquipCurItem(true);
    }

    void DoUnequipItem()
    {
        SetItem();
        equip.EquipCurItem(false);
    }

    void DoDropItem()
    {
        SetItem();
        equip.DropCurrentItem();
    }

    void SetItem()
    {
            equip.SetCurItem(item);
    }

    void DoPlayAnimation()
    {
        if (!setAnimator)
            animator = curAI.GetComponentInChildren<Animator>();

        if (animator)
            animator.CrossFade(stateToPlay, crossfadeTime);
    }

    void DoCallMethod()
    {
        methodToCall.InvokeMethod();
    }

    void DoInteractFX()
    {
        foreach (var fx in interacts)
        {
            fx.DoFX(curAI.gameObject, curTarget.gameObject);
        }
    }
}