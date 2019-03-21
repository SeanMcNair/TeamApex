using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy))]
public class EnemyController : UnitController 
{
    public enum EnemyMovementType {Static, Moving}

    [SerializeField] private EnemyMovementType movementType = EnemyMovementType.Moving;
    [SerializeField] private float speed = 3;
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private bool resetBoundariesOnEnable = true;
    [SerializeField] private bool resetYPositionOnPatrolResume = true;
    [SerializeField] private float resetSpeed = 10;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    [SerializeField] private bool flipOnCollision = true;

    [SerializeField] private bool bounce;
    [SerializeField] private float bouncePower = 5;
    [SerializeField] private float bounceXAddedForce;
    [SerializeField] private float bounceDelay;
    private float bounceTimer;

    private EnemySoundFX soundFX;

    private Enemy enemy;
    private EnemyAnimations anim;

    private bool pausePatrol;

    private float startYPos;
    private Vector3 lastPos;

    private Bounds bounds;

    public Vector2 leftBoundary;
    public Vector2 rightBoundary;

    private Coroutine heightCoroutine;

    private void OnEnable()
    {
        if (resetBoundariesOnEnable)
        {
            SetupBoundaries();
        }
    }

    public override void Start()
    {
        base.Start();
        startYPos = transform.position.y;
        SetupBoundaries();
    }

    public override void Update()
    {
        base.Update();
        SyncAnimations();
    }

    public override void FixedUpdate()
    { 
        if (enemy.IsDead)
            return;

        if (movementType == EnemyMovementType.Moving)
        {
            CheckCollision();
            MoveEnemy();
        }
        base.FixedUpdate();
    }

    public override void GetComponents()
    {
        base.GetComponents();
        //get components
        soundFX = GetComponent<EnemySoundFX>();
        enemy = GetComponent<Enemy>();
        bounds = GetComponent<Collider2D>().bounds;
        anim = GetComponent<EnemyAnimations>();
    }

    void SetupBoundaries()
    {
        leftBoundary = transform.TransformPoint(leftOffset);
        rightBoundary = transform.TransformPoint(rightOffset);
    }

    void MoveEnemy()
    {
        if (enemy.IsStunned || pausePatrol)
            return;

        if (movementType == EnemyMovementType.Moving)
            //move enemy..forward
            transform.Translate(transform.right * speed * speedMultiplier * Time.deltaTime, Space.World);
        //move enemy sideways in air if xforce and bounce enabled
        else if (movementType == EnemyMovementType.Static && bounce && !grounded && bounceXAddedForce > 0)
            transform.Translate(transform.right * bounceXAddedForce * Time.deltaTime, Space.World);

        if (bounce)
        {
            BounceEnemy();
        }
    }

    public override void CheckDirection()
    {
        base.CheckDirection();
        if (useBoundaries && !pausePatrol && movementType != EnemyMovementType.Static)
        {
            //rotate enemy based in waypoint x position
            if ((transform.position.x - bounds.extents.x) <= leftBoundary.x && !IsFacingRight)
            {
                FlipController();
            }
            else if ((transform.position.x + bounds.extents.x) >= rightBoundary.x && IsFacingRight)
            {
                FlipController();
            }
        } 
    }

    void CheckCollision()
    {
        if (!flipOnCollision || pausePatrol)
            return;

        if (sideHitLeft)
            GetCollisionInformation(leftCols, false);
        if(sideHitRight)
            GetCollisionInformation(rightCols, true);
    }

    void GetCollisionInformation(Collider2D[] _cols, bool _right)
    {
        foreach (var col in _cols)
        {
            if (col)
            {
                if (col.gameObject)
                {
                    //only flip if the collider does not equal this one
                    if (col.gameObject != gameObject)
                    {
                        if (IsFacingRight == _right)
                        {
                            if (col.gameObject.layer == gameObject.layer)//if same layer, wait a frame for other to collide as well
                                FlipController(1);
                            else
                                FlipController();
                        }

                    }
                }
                
            }
            
        }
    }

    void BounceEnemy()
    {
        if (enemy.IsStunned)
            return;

        if (grounded)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer > bounceDelay)
            {
                rb.Sleep();

                rb.AddForce(transform.up * bouncePower, ForceMode2D.Impulse);

                //playsound
                if (soundFX)
                    soundFX.PlayAttackSound();

                //anim switch
                if (anim)
                    anim.PlayJump();

                bounceTimer = 0;
            }

        }    
    }

    public void PausePatrol(bool _pause)
    {
        pausePatrol = _pause;
        if (resetYPositionOnPatrolResume)
        {
            if (!_pause)
                heightCoroutine = StartCoroutine(StartHeightReset());
            else if (heightCoroutine != null)
                StopCoroutine(heightCoroutine);       
        }
            
    }

    IEnumerator StartHeightReset()
    {
        var curY = transform.position.y;
        var diff = startYPos - curY;
        var time = diff / resetSpeed;
        float timer = 0;
        float perc = 0;
        var startRot = transform.rotation;
        while (perc < 1)
        {
            timer += Time.deltaTime;
            perc = timer / time;
            var heightLerp = Mathf.Lerp(curY, startYPos, perc);
            transform.position = new Vector2(transform.position.x, heightLerp);
            if (facingRight)
                transform.rotation = Quaternion.Slerp(startRot, Quaternion.identity, perc);
            else
                transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(0, -90, 0), perc);
            yield return new WaitForFixedUpdate();
        }
    }

    void SyncAnimations()
    {
        if (!anim)
            return;

        anim.Grounded = grounded;
        anim.VelocitySpeed = velocitySpeed;

    }
}
