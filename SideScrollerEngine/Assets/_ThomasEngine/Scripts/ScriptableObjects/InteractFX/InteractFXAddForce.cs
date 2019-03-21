using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddForce", menuName = "Data/Interacts/AddForce", order = 1)]
public class InteractFXAddForce : InteractFX
{
    public enum DirectionType { ClosestPointAngle, XOnly, Override }
    public enum ForceDirection { Away, Towards }
    public enum AffectedForceType { Sender, Receiver }

    [SerializeField] private AffectedForceType affectedForceType;
    [SerializeField] private DirectionType directionType;
    [SerializeField] private ForceDirection forceDirection;
    [SerializeField] private ForceMode2D forceMode;
    [SerializeField] private Vector2 direction;
    [SerializeField] private bool disableUnitSpeed;
    [SerializeField] private float disableSpeedTime = 1;

    [SerializeField] private float force = 1;
    [SerializeField] private bool consistent;

    UnitController unit;

    public override void DoFX(GameObject _sender, GameObject _receiver)
    {
        var affected = _receiver;
        var sender = _sender;
        if (affectedForceType == AffectedForceType.Sender)
        {
            affected = _sender;
            sender = _receiver;
        }
        unit = affected.GetComponent<UnitController>();   
        Rigidbody2D rb = affected.GetComponent<Rigidbody2D>();
        Collider2D col = affected.GetComponent<Collider2D>();
        Vector2 center = sender.transform.position;

        //gather direction information
        Vector2 dir = direction;
        if (directionType == DirectionType.ClosestPointAngle)
        {
            dir = ((Vector2)col.bounds.ClosestPoint(center) - center).normalized;
            if (forceDirection == ForceDirection.Towards)
                dir = (center - (Vector2)col.bounds.ClosestPoint(center)).normalized; ;
        }
        else if (directionType == DirectionType.XOnly)
        {
            dir = (Vector2)col.transform.position - center;
            if (dir.x > 0 )
            {
                dir = new Vector2(1, direction.y);
                if (forceDirection == ForceDirection.Towards)
                    dir = new Vector2(-1, direction.y);

            }
               
            else if (dir.x < 0)
            {
                dir = new Vector2(-1, direction.y);
                if (forceDirection == ForceDirection.Towards)
                    dir = new Vector2(1, direction.y);
            }
                
        }
        
        if (unit && disableUnitSpeed)
            unit.DisableSpeedSmooth(disableSpeedTime);
        if (rb)
        {
            if (consistent)
                rb.Sleep();
            rb.AddForce(dir * force, forceMode);
        }
        else if (affected)
            Debug.LogError("need a Rigidbody2D on: " + affected.name + " for " + this + " to function!");
        
    }
}
