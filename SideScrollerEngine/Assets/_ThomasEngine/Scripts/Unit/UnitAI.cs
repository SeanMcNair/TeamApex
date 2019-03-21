using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Unit))]
public class UnitAI : MonoBehaviour
{

    public enum DefaultState { Idle, Patrol}
    public enum MovementState { MoveToTarget, FleeFromTarget, PauseMovement, Default}

    [SerializeField] private DefaultState defaultState;
    [SerializeField] private Vector2 leftBoundary;
    [SerializeField] private Vector2 rightBoundary;
    [SerializeField] private bool ignoreZRotation;
    [SerializeField] private bool consistentUpDirection;

    public DetectZone[] detectZones;
    [SerializeField] private UnitAITriggerContainer unitAIEvents;

    private MovementState curState;
    private UnitController controller;
    private Bounds bounds;

    private void Start()
    {
        GetComponents();
    }

    void GetComponents()
    {
        controller = GetComponent<UnitController>();
        bounds = GetComponent<Collider2D>().bounds;
    }
    
    void StartEvents()
    {
        unitAIEvents.ActivateAllEvents(this, true);
    }

    protected void FixedUpdate()
    {
        CheckMovementState();
    }

    void CheckMovementState()
    {
        switch (curState)
        {
            case MovementState.MoveToTarget:
                DoMoveTowardsTarget();
                break;
            case MovementState.FleeFromTarget:
                DoFleeFromTarget();
                break;
            case MovementState.PauseMovement:
                DoPauseMovement();
                break;
            case MovementState.Default:
                DoDefault();
                break;
        }
    }

    void DoMoveTowardsTarget()
    {
        RotateTowardsTarget();
        Move();
    }

    void DoFleeFromTarget()
    {
        RotateAwayFromTarget();
        Move();
    }

    void DoPauseMovement()
    {

    }

    void DoDefault()
    {
        switch (defaultState)
        {
            case DefaultState.Idle:
                break;
            case DefaultState.Patrol:
                PatrolUnit();
                break;
        }
    }

    void RotateTowardsTarget()
    {
        if (!controller.Target)
            return;

        transform.LookAt2D(controller.Target.position, ignoreZRotation, consistentUpDirection);
    }

    void RotateAwayFromTarget()
    {
        if (!controller.Target)
            return;

        transform.LookAt2D(controller.Target.position, ignoreZRotation, consistentUpDirection);
    }

    void Move()
    {
        transform.Translate(Vector3.right * controller.BaseSpeed * Time.deltaTime, Space.Self);
    }


    void PatrolUnit()
    {

        transform.Translate(Vector3.right * controller.BaseSpeed * Time.deltaTime, Space.Self);
        //rotate enemy based in waypoint x position
        if ((transform.position.x - bounds.extents.x) <= leftBoundary.x && !controller.IsFacingRight)
        {
            controller.FlipController();
        }
        else if ((transform.position.x + bounds.extents.x) >= rightBoundary.x && controller.IsFacingRight)
        {
            controller.FlipController();
        }
    }

    public void DoDefaultState()
    {
        curState = MovementState.Default;
    }

    public void MoveToTarget(Transform _target)
    {
        controller.Target = _target;
        curState = MovementState.MoveToTarget;
    }

    public void FleeFromTarget(Transform _target)
    {
        controller.Target = _target;
        curState = MovementState.FleeFromTarget;
    }
}