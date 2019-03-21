using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDraggable : ItemAimable
{
    public new ItemDraggableData Data { get { return (ItemDraggableData)data; } }

    private Collider2D col;
    public GameObject DraggedObject { get { if (col) return col.gameObject; else return null; } }
    private Rigidbody2D rb;
    private Vector2 dir;
    private Vector3 startPos;
    private Vector3 lastPos;
    private Vector3 nextPos;
    private Vector3 rbDir;

    private Coroutine dragCoroutine;

    protected override void Awake()
    {
        base.Awake();
        Data.useType = ItemUseableData.UseType.Constant;
        Data.constantRemoveSpeed = Data.removeAmmoSpeed;
    }

    private void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        if (Input.GetButtonDown(Data.dragButton.stringValue))
        {
            GrabDraggable();
            if (dragCoroutine != null)
                StopCoroutine(dragCoroutine);
            dragCoroutine = StartCoroutine(StartDrag());
            DoUseType();
        }
    }

    void GrabDraggable ()
    {
        col = Data.dragZone.DetectCollider(controller.AimPos);
        if (col)
        {
            rb = col.GetComponent<Rigidbody2D>();
            startPos = col.transform.position;
            dir = controller.AimDirection;
            lastPos = Vector2.zero;
            nextPos = startPos;
            rbDir = dir;
        }
    }

    IEnumerator StartDrag()
    {
        while (Input.GetButton(Data.dragButton.stringValue))
        {
            Drag();
            yield return new WaitForFixedUpdate();
        }
        OnStopItem();
        ThrowItem();
    }

    void Drag()
    {
        if (col)
        {
            var aim = controller.AimPos;
            dir = aim - (Vector2)lastPos;
            if (Data.consistentThrowPower)
                dir.Normalize();
            nextPos = new Vector3(aim.x, aim.y, startPos.z);
            var sens = Data.dragSensitivity;
            //calculate sensitivity based on weight
            if (Data.sensitivityByWeight)
            {
                if (rb)
                {
                    //get collider mass
                    var weight = rb.mass;
                    //get percentage or cur weight based on min and max weight
                    var perc = Mathf.InverseLerp(Data.maxWeight, Data.minWeight, weight);
                    //multiply drag sensitivity by this percentage
                    sens = Data.dragSensitivity * perc;
                }
            }
            if (Data.allowDragThroughColliders)
            {
                col.transform.position = Vector3.Lerp(col.transform.position, nextPos, sens * Time.deltaTime);
            }
            else
            {
                rbDir = (controller.AimPos - rb.position);
                rb.velocity = Vector2.Lerp(Vector2.zero, rbDir, sens * Time.deltaTime) * (sens * 5);
            }

            //get last pos
            lastPos = aim;

        }

    }

    void ThrowItem()
    {
        if (!rb)
            return;

       rb.AddForce(dir * Data.throwForce, ForceMode2D.Impulse);
    }

}
