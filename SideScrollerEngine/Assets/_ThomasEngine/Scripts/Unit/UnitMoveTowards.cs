using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Unit))]
public class UnitMoveTowards : DetectZoneTrigger
{

    public enum MoveTowardType { OnDetection, Transform, ExternalSource }

    //detection
    [SerializeField] protected MoveTowardType moveType;
    [SerializeField] protected GameObject moveToPoint;

    //movement
    [SerializeField] protected bool setSpeed;
    [SerializeField] protected float speed = 3;
    [SerializeField] protected float maxArrivalDistance = 2;
    [SerializeField] protected float maxChaseDistance = 10;
    [SerializeField] protected float maxHeightDifference = 1;

    //arrival options
    [SerializeField] protected bool activateArrivalEvents;
    [SerializeField] protected UnityEvent unityEvents;

    protected bool chasing;
    public bool IsChasing() { return chasing; }
    protected bool arrived;
    public bool HasArrived() { return arrived; }
    protected bool targetInRange;
    public bool IsTargetInRange() { return targetInRange; }
    protected bool targetTooHigh;
    protected float distance;
    protected Unit unit;
    protected UnitController controller;

    private Coroutine moveCoroutine;

    public virtual void Start()
    {
        GetComponents();
        if (!setSpeed)
            speed = unit.CurUnitData.speed;

        CheckDetectType();
    }

    protected override void OnEnter(Collider2D _col)
    {
        var tar = _col.gameObject;
        var unit = tar.GetComponent<Unit>();
        if (unit)
        {
            var at = unit.AttackTarget;
            if (at)
                tar = at.gameObject;
        }
        MoveTowardsTarget(tar);
    }

    protected override void FixedUpdate()
    {
        if (moveType != MoveTowardType.OnDetection)
            return;
        base.FixedUpdate();
    }

    public virtual void GetComponents()
    {
        unit = GetComponent<Unit>();
        controller = GetComponent<UnitController>();
    }

    void CheckDetectType()
    {
        if (moveType == MoveTowardType.Transform)
            MoveTowardsTarget(moveToPoint);
    }

    public virtual void MoveTowardsTarget(GameObject _target)
    {
        if (chasing)
            return;

        chasing = true;
        controller.Target = _target.transform;
        moveCoroutine = StartCoroutine(StartMoveTowardsTarget());
    }

    IEnumerator StartMoveTowardsTarget()
    {
        CheckTargetDistance();
        while (targetInRange && !arrived && !unit.IsDead)
        {
            CheckHeightDifference();
            if (!targetTooHigh)
                MoveToTarget();
            CheckTargetDistance();
            //wait one frame and return
            yield return new WaitForFixedUpdate();
        }
        if (!targetInRange)
            StopChasing();
        else if (arrived)
        {
            DoArrivalEvents();
            StartCoroutine(StartCheckArrivalDistance());
        }


    }

    void MoveToTarget()
    {
        transform.Translate(transform.right * controller.CurSpeed * controller.SpeedMultiplier * Time.deltaTime, Space.World);
    }

    public virtual void CheckTargetDistance()
    {
        distance = Vector2.Distance(controller.Target.position, transform.position);
        targetInRange = distance < maxChaseDistance;
        arrived = distance <= maxArrivalDistance;
    }

    void CheckHeightDifference()
    {
        //stop enemy from turning/moving if the target is directly above or below
        var yPos = transform.position.y;
        if (unit.Collider)
            yPos = unit.Collider.bounds.center.y;
        float vDiff = Mathf.Abs(controller.Target.position.y - yPos);
        float hDiff = Mathf.Abs(controller.Target.position.x - transform.position.x);
        targetTooHigh = vDiff > maxHeightDifference && hDiff < maxArrivalDistance;
    }

    public virtual void StopChasing()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        controller.Target = null;
        chasing = false;
    }

    public virtual void DoArrivalEvents()
    {
        chasing = false;
        if (!activateArrivalEvents)
            return;

        unityEvents.Invoke();
    }

    IEnumerator StartCheckArrivalDistance()
    {
        while (arrived)
        {
            CheckTargetDistance();
            yield return new WaitForEndOfFrame();
        }
        MoveTowardsTarget(controller.Target.gameObject);
    }

}