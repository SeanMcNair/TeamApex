using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    //direction
    [SerializeField] protected bool faceRightAtStart;
    protected bool facingRight = true;
    public bool IsFacingRight { get { return facingRight; } }

    //aiming
    [SerializeField] protected bool enableAiming;
    [SerializeField] protected bool turnTowardsAimPos = true;
    [SerializeField] protected bool ignoreZRotation = true;
    [SerializeField] protected bool consistentUpDirection = true;
    protected bool aimingItemEquipped;
    protected Vector2 lookPos;
    protected Vector2 aimPos;
    public Vector2 AimPos { get { return aimPos; } }
    protected Vector2 aimDirection;
    public Vector2 AimDirection { get { return aimDirection; } }

    //Ground Detection
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected LayerProperty platformLayer;
    [SerializeField] protected Vector2 groundBoxSize = Vector2.one;
    [SerializeField] protected Vector2 groundBoxCenter = Vector2.zero;
    protected Collider2D[] groundHits;
    protected bool onPlatform;
    protected GameObject currentGroundGO;
    protected bool grounded;
    public bool IsGrounded { get { return grounded; } }
    protected float lastYPos;
    protected float curYPos;

    //Side Detection
    [SerializeField] protected LayerMask sideMask;
    [SerializeField] protected Vector2 leftDetectSize = Vector2.one;
    [SerializeField] protected Vector2 leftDetectCenter = Vector2.left;
    [SerializeField] protected Vector2 rightDetectSize = Vector2.one;
    [SerializeField] protected Vector2 rightDetectCenter = Vector2.right;
    protected Collider2D[] leftCols;
    protected Collider2D[] rightCols;

    [SerializeField] protected float baseSpeed;
    public float BaseSpeed { get { return baseSpeed; } set { baseSpeed = value; } }
    [SerializeField] protected bool enableJump = true;
    public bool JumpEnabled { get { return enableJump; } set { enableJump = value; } }
    [SerializeField] protected float jumpPower;
    public float JumpPower { get { return jumpPower; } set { jumpPower = value; } }
    protected float curSpeed;
    public float CurSpeed { get { return curSpeed; } }
    protected float speedMultiplier = 1;
    public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }
    protected bool speedEffected;
    public bool IsSpeedEffected { get { return speedEffected; } set { speedEffected = value; } }
    protected float velocitySpeed;
    public float VelocitySpeed { get { return velocitySpeed; } }
    protected Vector2 velocityDirection;
    public Vector2 VelocityDirection { get { return velocityDirection; } }

    protected bool sideHitLeft;
    public bool IsSideHitLeft { get { return sideHitLeft; } }
    protected bool sideHitRight;
    public bool IsSideHitRight { get { return sideHitRight; } }

    protected BoolWrapper disableMovement = new BoolWrapper(false);
    public bool IsMovementDisabled { get { return disableMovement.Value; } }

    protected BoolWrapper disableAiming = new BoolWrapper(false);
    public bool IsAimingDisabled { get { return disableAiming.Value; } }

    protected UnitEquip equip;
    protected ItemAimable aimable;
    protected Transform muzzle;
    protected Rigidbody2D rb;
    protected bool bouncing;

    protected Vector2 startColOffset;
    public Vector2 StartColOffset { get { return startColOffset; } set { startColOffset = value; } }
    protected Vector2 startColSize;
    public Vector2 StartColSize { get { return startColSize; } set { startColSize = value; } }

    protected Transform target;
    public Transform Target { get { return target; } set { target = value; } }

    protected Coroutine stopSpeedCoroutine;

    public virtual void Start()
    {
        GetComponents();
        InitializeController();
    }

    public virtual void Update()
    {
        CheckTarget();
        CheckFacingRight();
        CheckDirection();
        TurnTowardsAimPos();
    }

    public virtual void FixedUpdate()
    {
        CheckGrounded();
        CheckPlatform();
        CheckSideHits();
        ClampAiming();
    }

    public virtual void InitializeController()
    {
        curSpeed = baseSpeed;

        //set Direction
        transform.rotation = Quaternion.Euler(Vector3.zero);
        CheckFacingRight();
        if (!faceRightAtStart && facingRight || faceRightAtStart && !facingRight)
            FlipController(1);
        StartCoroutine(StartCalculateSpeed());
    }

    public virtual void GetComponents()
    {
        //get comps
        rb = GetComponent<Rigidbody2D>();
        equip = GetComponent<UnitEquip>();
    }

    public virtual void CheckTarget()
    {
        if (target)
        {
            lookPos = target.position;
            if (muzzle)
                aimDirection = (target.position - muzzle.position).normalized;
        }
        else
        {   

            if (muzzle)
            {
                if (muzzle.localRotation != Quaternion.identity)
                    muzzle.localRotation = Quaternion.identity;
                lookPos = muzzle.TransformPoint(Vector3.right * aimable.Data.aimDistance);
            }
            else
                aimDirection = transform.TransformDirection(Vector2.right);
        }
        
    }

    public virtual void ClampAiming()
    {
        if (!muzzle)
            return;
        //get distance
        var dist = Vector2.Distance(muzzle.position, lookPos);
        //set clamp distance
        var curclamp = Mathf.Clamp(dist, aimable.Data.minAimDistance, aimable.Data.aimDistance);
        //get normalized direction controller is pointing at
        var dir = (lookPos - (Vector2)muzzle.position).normalized;
        //multiply the direction by the clamped distance
        var distClamped = dir * curclamp;
        //apply the multiplied direction as an offset to the origin
        aimPos = (Vector2)muzzle.position + distClamped;
        //clamp x position so aimer cant go behind muzzle
        if (aimable.Data.disableAimBehindMuzzle)
        {
            if ((aimPos.x < muzzle.position.x && facingRight) || (aimPos.x > muzzle.position.x && !facingRight))
                aimPos = new Vector2(muzzle.position.x, aimPos.y);
        }
    }

    public virtual void Bounce(Vector2 _direction, float _force, bool _consistent = false)
    {
        bouncing = true;
        if (_consistent)
        {
            rb.Sleep();
            rb.velocity = Vector2.zero;
        }
            
        rb.velocity = _direction * _force;
    }

    public virtual void SetCurSpeed(float _speed)
    {
        curSpeed = _speed;
    }

    public virtual void CheckDirection()
    {

        if (!enableAiming || disableAiming.Value)
            return;

        if (equip)
        {
            if (equip.CurItem)
            {
                if (equip.CurItem.gameObject.activeSelf)
                    aimingItemEquipped = Utils.IsClassOrSubClass(equip.CurItem.GetType(), typeof(ItemAimable));
                else
                    aimingItemEquipped = false;
            }
            else
                aimingItemEquipped = false;
        }
        if (aimingItemEquipped)
        {
            aimable = (ItemAimable)equip.CurItem;
            muzzle = aimable.Muzzle;
            if (muzzle)
            {
                aimDirection = (aimPos - (Vector2)muzzle.position).normalized;
            }
            
        }          

    }

    protected virtual void TurnTowardsAimPos()
    {
        if (!turnTowardsAimPos || lookPos == Vector2.zero)
            return;

        if (target)
        {
            //turn unit based on aimer position
            transform.LookAt2D(lookPos, ignoreZRotation, consistentUpDirection);
        }
    }

    public virtual void CheckGrounded()
    {

        curYPos = Mathf.Round(transform.position.y * 100) / 100;

        if (curYPos != lastYPos || onPlatform)
        {
            //ground detect
            groundHits = Physics2D.OverlapBoxAll((Vector2)transform.position + groundBoxCenter, groundBoxSize, 0, groundMask);
            if (groundHits.Length > 0)
            {
                currentGroundGO = groundHits[0].gameObject;
                grounded = true;
            }
            else
                grounded = false;
        }

        lastYPos = Mathf.Round(transform.position.y * 100) / 100;

    }

    public virtual void CheckPlatform()
    {
        var col = Physics2D.OverlapBox((Vector2)transform.position + groundBoxCenter, groundBoxSize, 0, platformLayer.maskValue);
        if (col)
        {
            if (!onPlatform)
            {
                onPlatform = true;
                var colTrans = col.transform;
                transform.SetParent(colTrans, true);
            }
        }
        else if (onPlatform)
        {
            onPlatform = false;
            transform.SetParent(null, true);
        }

    }

    public virtual void CheckSideHits()
    {
        //left detect        
        leftCols = Physics2D.OverlapBoxAll((Vector2)transform.position + leftDetectCenter, leftDetectSize, 0, sideMask);
        sideHitLeft = leftCols.Length > 0;
        //right detect
        rightCols = Physics2D.OverlapBoxAll((Vector2)transform.position + rightDetectCenter, rightDetectSize, 0, sideMask);
        sideHitRight = rightCols.Length > 0;
    }

    void CheckFacingRight()
    {
        facingRight = transform.TransformPoint(Vector3.right).x > transform.position.x;
    }

    public virtual void DisableAiming(bool _disable)
    {
        disableAiming.Value = _disable;
    }

    public virtual void DisableMovement(bool _disable)
    {
        disableMovement.Value = _disable;
    }

    public virtual void DisableMovement(float _time)
    {
        StartCoroutine(Utils.StartBoolTimer(disableMovement, _time, 0));
    }

    public virtual void DisableMovement(int _frameCount)
    {
        StartCoroutine(Utils.StartBoolTimer(disableMovement, 0, _frameCount));
    }

    public virtual void DisableSpeedSmooth(float _time, bool _fadeIn = false)
    {
        if (stopSpeedCoroutine != null)
            StopCoroutine(stopSpeedCoroutine);
        stopSpeedCoroutine = StartCoroutine(StartDisableSpeedSmooth(_time, _fadeIn));
    }

    public virtual void FlipController(int _frameDelay = default(int))
    {
        if (_frameDelay < 1)
            DoFlipController();
        else
            StartCoroutine(StartFlipController(_frameDelay));
    }

    IEnumerator StartDisableSpeedSmooth(float _time, bool _fadeIn)
    {
        var spd = speedMultiplier;
        float timer = 0;
        if (_fadeIn)
        {
            while (timer < _time)
            {
                timer += Time.deltaTime;
                if (timer > _time)
                    timer = _time;
                float perc = timer / _time;
                speedMultiplier = Mathf.Lerp(spd, 0, perc);
                yield return new WaitForEndOfFrame();
            }
            timer = 0;
        }
        while (timer < _time)
        {
            timer += Time.deltaTime;
            if (timer > _time)
                timer = _time;
            float perc = timer / _time;
            speedMultiplier = Mathf.Lerp(0, 1, perc);
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator StartFlipController(int _frameDelay)
    {
        int i = 0;
        while (i < _frameDelay)
        {
            i++;
            yield return new WaitForFixedUpdate();
        }
        DoFlipController();
    }

    void DoFlipController()
    {
        transform.right = transform.TransformDirection(Vector3.left);
        if (!(transform.eulerAngles.y == 0 || transform.eulerAngles.y == 180))
        {
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    IEnumerator StartCalculateSpeed()
    {
        var pos = transform.position;
        while (Application.isPlaying)
        {
            // Position at frame start
            pos = transform.position;
            // Wait till it the end of the frame
            yield return new WaitForFixedUpdate();
            // Calculate velocity: Velocity = DeltaPosition / DeltaTime
            velocityDirection = transform.position - pos;
            velocitySpeed = (velocityDirection / Time.fixedDeltaTime).magnitude;
        }
    }
}
