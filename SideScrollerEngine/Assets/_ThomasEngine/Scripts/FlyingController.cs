using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingController : UnitController
{

    [SerializeField] private float ceiling = 5;
    [SerializeField] private float divePower = 10;
    [SerializeField] private float airDensity = 1;
    [SerializeField] private string diveButton = "Fire1";

    float moveX;
    float moveY;
    Vector2 move;
    private bool hitCeiling;

    public override void Start()
    {
        base.Start();
        SetCurSpeed(10);
    }

    public override void Update()
    {
        base.Update();
        GetInputs();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveController();
        CheckCeling();
        FloatUp();
    }

    void GetInputs()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown(diveButton))
            StartCoroutine(StartDive());
        
        move = new Vector2(moveX, moveY);
    }

    IEnumerator StartDive()
    {
        while (Input.GetButton(diveButton))
        {
            moveY -= divePower * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    void CheckCeling()
    {
        hitCeiling = transform.position.y >= ceiling;
        if (hitCeiling)
        {
            transform.position = new Vector2(transform.position.x, ceiling);
            moveY = 0;
        }
    }

    void FloatUp()
    {
        if (hitCeiling)
            return;

        moveY += airDensity * Time.deltaTime;
    }

    void MoveController()
    {
        transform.Translate(move * curSpeed * speedMultiplier * Time.deltaTime);
    }
}
