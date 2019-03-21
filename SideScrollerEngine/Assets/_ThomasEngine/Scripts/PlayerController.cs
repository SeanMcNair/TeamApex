using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : UnitController
{

    public enum JumpStyle { Consistent, Additive }

    public enum MovementState { Idle, Walking, Backwards, Running, Crouching, InAir, Climbing, Grappling }
    private MovementState movementState;
    public MovementState CurMovementState { get { return movementState; } }


    //Movement
    [SerializeField] private bool constantSpeed;
    [SerializeField] private bool enableJoystickUse = true;
    [SerializeField] private bool enableRun = true;
    public bool RunEnabled { get { return enableRun; } set { enableRun = value; } }
    [SerializeField] private float runSpeed = 15;
    private BoolWrapper run = new BoolWrapper(false);
    [SerializeField] private float backwardSpeed = 3;
    private bool backwards;

    //crouching
    [SerializeField] private bool enableCrouch = true;
    public bool CrouchEnabled { get { return enableCrouch; } set { enableCrouch = value; } }
    [SerializeField] private float crouchSpeed = 2;
    [SerializeField] private float crouchSpeedTime = 1;
    private BoolWrapper crouch = new BoolWrapper(false);
    private bool crouching;

    //climbing
    [SerializeField] private bool enableClimbing = true;
    public bool ClimbingEnabled { get { return enableClimbing; } set { enableClimbing = value; } }
    [SerializeField] private float climbSpeed = 6;
    public bool climbing;
    private bool onLadder;
    public bool IsOnLadder { get { return onLadder; } set { onLadder = value; } }
    [SerializeField] private float inAirControlTime = 1;
    private bool controlFading;

    //Jumping
    [SerializeField] protected InputProperty jumpButton;
    [SerializeField] private JumpStyle jumpStyle = JumpStyle.Additive;
    private bool jump;
    private bool doubleJumping;
    [SerializeField] private bool enableDoubleJump;
    public bool DoubleJumpEnabled { get { return enableDoubleJump; } set { enableDoubleJump = value; } }
    private bool doubleJumpActive;
    [SerializeField] private bool enableJumpClimbing = true;
    private BoolWrapper jumpClimbing = new BoolWrapper(false);
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float lowJumpMultiplier;

    //Dashing
    [SerializeField] private bool enableDash;
    public bool DashEnabled { get { return enableDash; } set { enableDash = value; } }
    [SerializeField] private InputProperty dashButton;
    [SerializeField] private float dashPower = 10;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float dashCooldown = 2;
    [SerializeField] private int dashButtonInd;
    private bool dashing;
    private bool dashCool = true;

    //Grappling
    [SerializeField] private bool enableGrappleMods;
    [SerializeField] private float grappleSwingSpeed = 20;
    [SerializeField] private float grappleJumpPower = 5;
    private bool grappling;
    public bool IsGrappling { get { return grappling; } set { grappling = value; } }

    //one way platforms
    [SerializeField] private bool enableOneWayPlatforms;
    [SerializeField] private DetectZone oneWayDetectZone;
    [SerializeField] private float oneWayDisablePhysicsTime = 0.3f;
    [SerializeField] private LayerProperty disablePhysicsLayer;
    private bool ignoringLayers;

    //Ceiling Detection
    [SerializeField] private LayerMask ceilingMask;
    [SerializeField] public Vector2 ceilingBoxSize = Vector2.zero;
    [SerializeField] public Vector2 ceilingBoxCenter = Vector2.zero;
    [SerializeField] private Vector2 curCeilingBoxCenter;
    private Vector2 crouchCeilingBoxCenter;
    private Collider2D[] ceilingHits;
    private BoolWrapper ceilingHit = new BoolWrapper(false);
    private Collider2D curCeilingHit;
    private float lastCeilingYPos;

    //wallJump stuff
    [SerializeField] private bool enableWallJump = true;
    public bool WallJumpEnabled { get { return enableWallJump; } set { enableWallJump = value; } }
    [SerializeField] private bool autoFlip = true;
    [SerializeField] private bool faceWallToJump = true;
    [SerializeField] private float wallBounceForceX = 5;
    [SerializeField] private float wallBounceForceY = 5;

    [SerializeField] private bool disableMovementOnWallJump = true;
    [SerializeField] private float disableTime = 1;

    //movement inputs
    protected float inputHor;
    protected float inputVer;
    protected Vector2 inputDirection;
    protected Vector2 move;

    //aiming
    private Vector2 mousePos;
    private Vector2 lastJoyStickPos;
    private float conHor;
    private float conVer;

    //comps
    private Player pl;
    protected CapsuleCollider2D conCollider;
    protected PlayerAnimations anim;

    public override void Update()
    {
        base.Update();
        GetInputs();
        SetMovementStates();
        CheckMovementStates();
        CheckBouncing();
        SyncAnimations();
    }

    public override void FixedUpdate()
    {
        if (enableAiming && aimingItemEquipped)
        {
            ControllerAim();
            MouseAim();
        }
        base.FixedUpdate();
        CheckCeiling();
        CheckOneWay();
        CheckHeightVelocity();
        Move();
    }

    public override void InitializeController()
    {
        base.InitializeController();

        curSpeed = baseSpeed;

        curCeilingBoxCenter = ceilingBoxCenter;
        crouchCeilingBoxCenter = new Vector2(ceilingBoxCenter.x, ceilingBoxCenter.y / 2);

        //get collider
        conCollider = GetComponent<CapsuleCollider2D>();
        startColSize = conCollider.size;
        startColOffset = conCollider.offset;

        //get comps
        anim = GetComponent<PlayerAnimations>();
        pl = GetComponent<Player>();
        equip = GetComponent<PlayerEquip>();
    }

    public override void CheckTarget()
    {
        if (!enableAiming && muzzle)
            aimPos = muzzle.TransformPoint(Vector3.right * aimable.Data.aimDistance);
    }

    protected override void TurnTowardsAimPos()
    {
        if (enableAiming && turnTowardsAimPos && muzzle)
        {
           transform.LookAt2D(lookPos, ignoreZRotation, consistentUpDirection);
        }
            
    }

    public virtual void GetInputs()
    {
        if (pl.IsDead)
            return;

        //movement axis input
        inputHor = Input.GetAxisRaw("Horizontal");
        inputVer = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector2(inputHor, inputVer).normalized;

        //jump input
        jump = Input.GetButtonDown(jumpButton.stringValue) && enableJump;
        if (jump)
            Jump();

        //run input
        run.Value = (Input.GetButton("Run") || Input.GetAxisRaw("XboxTriggerLeft") > 0);

        //crouch input
        crouch.Value = (inputDirection.y < -0.69f && !climbing);

        if (enableAiming && aimingItemEquipped)
            backwards = (IsFacingRight && inputHor < 0 || !IsFacingRight && inputHor > 0);

        if (enableDash)
        {
            if (Input.GetButtonDown(dashButton.stringValue))
                Dash();
        }
    }

    void MouseAim()
    {
        //return if controller is connected
        if (Input.GetJoystickNames().Length > 0 && enableJoystickUse)
            return;

        //get mouse position on screen
        mousePos = Input.mousePosition;
        lookPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
    }

    void ControllerAim()
    {
        //create position based on axis input and offset
        conHor = Input.GetAxis("XboxRightStickHor");
        conVer = -Input.GetAxis("XboxRightStickVer");

        var joystickPos = new Vector2(conHor, conVer);
        if (muzzle)
        {
            if (joystickPos != Vector2.zero)
            {
                lastJoyStickPos = joystickPos * aimable.Data.aimDistance;
                lookPos = (Vector2)muzzle.position + lastJoyStickPos;
            }   
            else
                lookPos = (Vector2)muzzle.position + ((lastJoyStickPos / aimable.Data.aimDistance) * aimable.Data.minAimDistance);
                
            
        }
            
    }

    void Dash()
    {
        if (!dashCool || (sideHitLeft && !facingRight) || (sideHitRight && facingRight))
            return;
        StartCoroutine(StartDash());
    }

    IEnumerator StartDash()
    {
        dashing = true;
        float timer = 0;
        rb.constraints = (RigidbodyConstraints2D)6;
        Vector2 force = Vector2.zero;
        if (IsFacingRight)
            force = transform.right;
        else
            force = -transform.right;
        while (dashing)
        {
            transform.Translate(force * dashPower * Time.deltaTime);

            timer += Time.deltaTime;
            if (timer > dashTime || sideHitLeft && !facingRight || sideHitRight && facingRight)
            {
                dashing = false;
            }
            yield return new WaitForFixedUpdate();
        }
        rb.constraints = (RigidbodyConstraints2D)4;
        StartCoroutine(StartDashCoolDown());
    }

    IEnumerator StartDashCoolDown()
    {
        dashCool = false;
        float timer = 0;
        while (timer < dashCooldown)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dashCool = true;
    }

    void CheckMovementStates()
    {
        if (enableGrappleMods && grappling)
            movementState = MovementState.Grappling;
        else if (onLadder && enableClimbing && inputDirection.y > 0.69f && !jumpClimbing.Value)
        {
            movementState = MovementState.Climbing;
        }
        else if (climbing)
            movementState = MovementState.Climbing;
        else if (crouching && ceilingHit.Value || crouch.Value && enableCrouch)
            movementState = MovementState.Crouching;
        else if (grounded)
        {
            if (Mathf.Abs(inputHor) > 0)
            {  
                if (backwards)
                    movementState = MovementState.Backwards;
                else if (run.Value && enableRun)
                    movementState = MovementState.Running;
                else
                    movementState = MovementState.Walking;
            }
            else
                movementState = MovementState.Idle;
        }
        else
        {
                movementState = MovementState.InAir;
        }
            
    }

    void SetMovementStates()
    {
        switch (movementState)
        {
            case MovementState.Idle:
                Idle();
                break;
            case MovementState.Walking:
                Walking();
                break;
            case MovementState.Backwards:
                Backwards();
                break;
            case MovementState.Running:
                Running();
                break;
            case MovementState.Crouching:
                Crouching();
                break;
            case MovementState.InAir:
                InAir();
                break;
            case MovementState.Climbing:
                Climbing();
                break;
            case MovementState.Grappling:
                Grappling();
                break;
        }
    }

    void Backwards()
    {
        curSpeed = backwardSpeed;
    }

    void Grappling()
    {
        curSpeed = grappleSwingSpeed;
        if (!grounded && (sideHitLeft || sideHitRight))
        {
            if (inputHor > 0 && sideHitLeft)
                rb.AddForce(transform.right * grappleJumpPower, ForceMode2D.Impulse);
            if (inputHor < 0 && sideHitRight)
                rb.AddForce(-transform.right * grappleJumpPower, ForceMode2D.Impulse);

        }
    }

    void Idle()
    {
        curSpeed = baseSpeed;
        backwards = false;
    }

    void Walking()
    {
        curSpeed = baseSpeed;
    }

    void Running()
    {
        curSpeed = runSpeed;
    }

    void Crouching()
    {
        if (!crouch.Value && !ceilingHit.Value)
        {
            ResetCrouch();
            return;
        }

        if (crouching)
            return;

        Crouch(new Vector2(conCollider.offset.x, startColOffset.y / 2), new Vector2(conCollider.size.x, startColSize.y / 2));

        if (curSpeed > crouchSpeed && crouchSpeedTime > 0)
            StartCoroutine(StartSpeedFade(curSpeed, crouchSpeed, crouchSpeedTime, crouch));
        else
            curSpeed = crouchSpeed;

        crouching = true;

    }

    void Crouch(Vector2 _offset, Vector2 _size)
    {
        conCollider.offset = _offset;
        conCollider.size = _size;
    }

    void ResetCrouch()
    {
        Crouch(startColOffset, startColSize);
        crouching = false;
    }

    void InAir()
    {
        if (Mathf.Abs(inputHor) > 0)
        {
            if (controlFading)
                return;

            StartCoroutine(StartControlFade(inputHor, inAirControlTime));
        }
    }

    void Climbing()
    {
        if (!onLadder)
        {
            StopClimbing();
            return;
        }

        if (jump && enableJumpClimbing && !jumpClimbing.Value)
        {
            StartCoroutine(StartClimbJump());
            StopClimbing();
            return;
        }

        if (jumpClimbing.Value)
            return;

        if (inputVer == 0)//freeze y position so there is no slow fall
            rb.constraints = (RigidbodyConstraints2D)6;
        else//unfreeze y position but reset velocity
        {
            rb.velocity = Vector2.zero;
            rb.constraints = (RigidbodyConstraints2D)4;
        }

        if (climbing)
            return;

        curSpeed = climbSpeed;

        if (!IsFacingRight)
            FlipController();

        climbing = true;
    }

    void StopClimbing()
    {
        rb.constraints = (RigidbodyConstraints2D)4;
        climbing = false;
    }

    public virtual void Move()
    {
        if (pl.IsDead || disableMovement.Value || dashing)
            return;

        if (!controlFading)
        {
            if (constantSpeed)
                move = transform.right;
            else
                move = transform.right * inputHor;
            if (climbing)
                move = new Vector2(inputHor, inputVer).normalized;
        }

        transform.Translate(move * curSpeed * speedMultiplier * Time.deltaTime);

    }

    public virtual void Jump()
    { 

        if (!grounded && !doubleJumping && enableDoubleJump)
        {
            //enable double jumping until grounded/climbing
            StartCoroutine(StartJumpSwitch());
            doubleJumpActive = true;
        }

        if (grounded || doubleJumpActive || sideHitLeft || sideHitRight || climbing)
        {
            Vector2 force = Vector2.up * jumpPower;
            if (!grounded)
            {
                if (sideHitLeft || sideHitRight)
                {
                    if (!enableWallJump)
                        return;

                    if (sideHitLeft)
                    {
                        if (!IsFacingRight || !faceWallToJump)
                        {
                            force = new Vector2(wallBounceForceX, jumpPower + wallBounceForceY);
                            if (autoFlip)
                                FlipController();
                        }
                        else
                            force = Vector2.zero;
                    }
                    else if (sideHitRight)
                    {
                        if (IsFacingRight || !faceWallToJump)
                        {
                            force = new Vector2(-wallBounceForceX, jumpPower + wallBounceForceY);
                            if (autoFlip)
                                FlipController();
                        }
                        else
                            force = Vector2.zero;
                    }

                    if (disableMovementOnWallJump)
                        StartCoroutine(StartSpeedFade(0, curSpeed, disableTime, new BoolWrapper(!grounded)));
                }
            }

            //jump player
            rb.velocity = force;

            //set double jump
            if (enableDoubleJump)
            {
                if (doubleJumpActive)
                    doubleJumpActive = false;
            }

            //animations
            if (anim)
                anim.PlayJump();

        }

    }

    void CheckCeiling()
    {
        if (crouching)
        {
            if (curCeilingBoxCenter != crouchCeilingBoxCenter)
                curCeilingBoxCenter = crouchCeilingBoxCenter;
        }
        else if (curCeilingBoxCenter != ceilingBoxCenter)
            curCeilingBoxCenter = ceilingBoxCenter;

        ceilingHits = Physics2D.OverlapBoxAll((Vector2)transform.position + curCeilingBoxCenter, ceilingBoxSize, 0, ceilingMask);
        if (ceilingHits.Length > 0)
        {
            ceilingHit.Value = true;

            if (ceilingHits[0] != curCeilingHit)
            {
                curCeilingHit = ceilingHits[0];
                lastCeilingYPos = curCeilingHit.transform.position.y;
                return;
            }

            if (curCeilingHit.transform.position.y < lastCeilingYPos)//is ceiling moving down?
                pl.Die("being crushed by " + ceilingHits[0].ToString());

            lastCeilingYPos = curCeilingHit.transform.position.y;

        }
        else if (ceilingHit.Value)
            ceilingHit.Value = false;
    }

    void CheckOneWay()
    {
        if (!enableOneWayPlatforms || velocityDirection.y < 0)
            return;
        Collider2D[] cols;
        cols = oneWayDetectZone.DetectColliders(transform);
        if (cols.Length > 0 && !ignoringLayers)
        {
            StartCoroutine(StartIgnoreLayers());
        }

    }

    IEnumerator StartIgnoreLayers()
    {
        ignoringLayers = true;
        var layer = gameObject.layer;
        Physics2D.IgnoreLayerCollision(layer, disablePhysicsLayer.indexValue, true);
        yield return new WaitForSeconds(oneWayDisablePhysicsTime);
        Physics2D.IgnoreLayerCollision(layer, disablePhysicsLayer.indexValue, false);
        ignoringLayers = false;
    }

    void CheckBouncing()
    {
        if (grounded && bouncing || jump)
            bouncing = false;
    }

    void CheckHeightVelocity()
    {
        if (climbing || jumpStyle == JumpStyle.Consistent || bouncing || disableMovement.Value || grappling)
            return;

        if (rb.velocity.y < 0)
            AddGravity(gravityMultiplier);
        if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            AddGravity(lowJumpMultiplier);
    }

    void AddGravity(float _multiplier)
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * _multiplier * Time.deltaTime;
    }

    public override void CheckDirection()
    {
        base.CheckDirection();

        if (climbing || disableMovement.Value)
            return;

        if (!enableAiming || !turnTowardsAimPos || !aimingItemEquipped)
        {
            //flip controller based on input left or right
            if (inputHor < 0 && IsFacingRight || inputHor > 0 && !IsFacingRight)
            {
                FlipController();
            }
        }
    }

    void RunTimer(float _time, FloatWrapper _timer, FloatWrapper _perc)
    {
        _timer.Value += Time.deltaTime;
        if (_timer.Value > _time)
            _timer.Value = _time;
        _perc.Value = _timer.Value / _time;
    }

    IEnumerator StartControlFade(float _startHor, float _time)
    {
        FloatWrapper timer = new FloatWrapper(0);
        FloatWrapper perc = new FloatWrapper(0);
        controlFading = true;
        while (perc.Value < 1 && !grounded && !climbing)
        {
            RunTimer(_time, timer, perc);
            float hor = Mathf.Lerp(_startHor, inputHor, perc.Value);
            move = transform.right * hor;
            yield return new WaitForEndOfFrame();
        }
        controlFading = false;
    }

    IEnumerator StartSpeedFade(float _startSpeed, float _endSpeed, float _time, BoolWrapper _condition)
    {
        float timer = 0;
        float perc = 0;
        while (perc < 1 && _condition.Value)
        {
            timer += Time.deltaTime;
            if (timer > _time)
                timer = _time;
            perc = timer / _time;
            curSpeed = Mathf.Lerp(_startSpeed, _endSpeed, perc);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartJumpSwitch()
    {
        //anim switch
        if (anim)
            anim.PlayDoubleJump();

        //possible to do a jump out of the air if an input jump was not the cause for being not grounded.
        doubleJumping = true;
        while (!grounded && !climbing)
            yield return new WaitForEndOfFrame();
        doubleJumping = false;
    }

    IEnumerator StartClimbJump()
    {
        jumpClimbing.Value = true;
        while (rb.velocity.y > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        jumpClimbing.Value = false;
    }

    public override void FlipController(int _frameDelay = 0)
    {
        if (grappling)
            return;

        base.FlipController(_frameDelay);
    }

    public void SetSpeed(float _newSpeed, float _time)
    {
        StartCoroutine(StartSpeedFade(curSpeed, _newSpeed, _time, new BoolWrapper(true)));
    }

    void SyncAnimations()
    {
        if (!anim)
            return;

        anim.FacingRight = IsFacingRight;
        anim.Grounded = grounded;
        anim.Running = run.Value;
        anim.Backwards = backwards;
        anim.Crouched = crouching;
        anim.Climbing = climbing;
        anim.WallHitLeft = sideHitLeft;
        anim.WallHitRight = sideHitRight;
        anim.InputHorizontal = inputHor;
        anim.VelocitySpeed = velocitySpeed;
        anim.InputVertical = inputVer;
    }

}
