using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : UnitControllerEditor
{
    private GUIStyle headerStyle = new GUIStyle();
    private PlayerController pSource;
    //Movement Properties
    private SerializedProperty constantSpeed;
    private SerializedProperty enableJoystickUse;
    private SerializedProperty enableRun;
    private SerializedProperty runSpeed;
    private SerializedProperty backwardSpeed;
    private SerializedProperty enableCrouch;
    private SerializedProperty crouchSpeed;
    private SerializedProperty crouchSpeedTime;
    private SerializedProperty inAirControlTime;
    private SerializedProperty enableClimbing;
    private SerializedProperty climbSpeed;

    //Jumping Properties
    protected SerializedProperty enableJump;
    protected SerializedProperty jumpButton;
    private SerializedProperty jumpStyle;
    private SerializedProperty jumpPower;
    private SerializedProperty enableDoubleJump;
    private SerializedProperty enableJumpClimbing;
    private SerializedProperty gravityMultiplier;
    private SerializedProperty lowJumpMultiplier;

    //dashing
    private SerializedProperty enableDash;
    private SerializedProperty dashButton;
    private SerializedProperty dashPower;
    private SerializedProperty dashTime;
    private SerializedProperty dashCooldown;

    //grapple
    private SerializedProperty enableGrappleMods;
    private SerializedProperty grappleSwingSpeed;
    private SerializedProperty grappleJumpPower;

    //one way platforms
    private SerializedProperty enableOneWayPlatforms;
    private SerializedProperty oneWayDetectZone;
    private SerializedProperty oneWayDisablePhysicsTime;
    private SerializedProperty disablePhysicsLayer;

    //ceiling Detection
    private SerializedProperty ceilingMask;
    private SerializedProperty ceilingBoxSize;
    private SerializedProperty ceilingBoxCenter;
    private SerializedProperty curCeilingBoxCenter;

    //Wall Detection
    private SerializedProperty enableWallJump;
    private SerializedProperty autoFlip;
    private SerializedProperty faceWallToJump;
    private SerializedProperty wallBounceForceX;
    private SerializedProperty wallBounceForceY;
    private SerializedProperty disableMovementOnWallJump;
    private SerializedProperty disableTime;

    public override void OnEnable()
    {
        base.OnEnable();
        pSource = (PlayerController)source;
        SetHeaderStyle();   
    }

    public override void GetProperties()
    {
        base.GetProperties();
        GetMovementProperties();
        GetJumpingProperties();
        GetDashProperties();
        GetGrappleProperties();
        GetOneWayProperties();
        GetCeilingDetectionProperties();
        GetWallDetectionProperties();
    }

    public override void SetProperties()
    {
        DisplayMovement();
        DisplayAiming();
        DisplayJumping();
        DisplayDash();
        DisplayGrapple();
        DisplayOneWay();
        DisplayGroundDetection();
        DisplaySideDetection();
        DisplayCeilingDetection();

    }

    public override void DisplayMovement()
    {
        base.DisplayMovement();
        EditorGUILayout.PropertyField(constantSpeed);
        EditorGUILayout.PropertyField(enableJoystickUse);
        EditorGUILayout.PropertyField(enableRun);
        if (enableRun.boolValue)
            EditorGUILayout.PropertyField(runSpeed);
        EditorGUILayout.PropertyField(enableCrouch);
        if (enableCrouch.boolValue)
        {
            EditorGUILayout.PropertyField(crouchSpeed);
            EditorGUILayout.PropertyField(crouchSpeedTime);
        }
        EditorGUILayout.PropertyField(backwardSpeed);
        EditorGUILayout.PropertyField(enableClimbing);
        if (enableClimbing.boolValue)
            EditorGUILayout.PropertyField(climbSpeed);
        EditorGUILayout.PropertyField(inAirControlTime);

    }

    void GetMovementProperties()
    {
        constantSpeed = sourceRef.FindProperty("constantSpeed");
        enableJoystickUse = sourceRef.FindProperty("enableJoystickUse");
        enableRun = sourceRef.FindProperty("enableRun");
        runSpeed = sourceRef.FindProperty("runSpeed");
        backwardSpeed = sourceRef.FindProperty("backwardSpeed");
        enableCrouch = sourceRef.FindProperty("enableCrouch");
        crouchSpeed = sourceRef.FindProperty("crouchSpeed");
        crouchSpeedTime = sourceRef.FindProperty("crouchSpeedTime");
        inAirControlTime = sourceRef.FindProperty("inAirControlTime");
        enableClimbing = sourceRef.FindProperty("enableClimbing");
        climbSpeed = sourceRef.FindProperty("climbSpeed");
    }

    void GetJumpingProperties()
    {
        enableJump = sourceRef.FindProperty("enableJump");
        jumpButton = sourceRef.FindProperty("jumpButton");
        jumpStyle = sourceRef.FindProperty("jumpStyle");
        jumpPower = sourceRef.FindProperty("jumpPower");
        enableDoubleJump = sourceRef.FindProperty("enableDoubleJump");
        gravityMultiplier = sourceRef.FindProperty("gravityMultiplier");
        lowJumpMultiplier = sourceRef.FindProperty("lowJumpMultiplier");
        lowJumpMultiplier = sourceRef.FindProperty("lowJumpMultiplier");
        enableJumpClimbing = sourceRef.FindProperty("enableJumpClimbing");
    }

    void GetDashProperties()
    {
        enableDash = sourceRef.FindProperty("enableDash");
        dashButton = sourceRef.FindProperty("dashButton");
        dashPower = sourceRef.FindProperty("dashPower");
        dashTime = sourceRef.FindProperty("dashTime");
        dashCooldown = sourceRef.FindProperty("dashCooldown");

    }

    void GetGrappleProperties()
    {
        enableGrappleMods = sourceRef.FindProperty("enableGrappleMods");
        grappleSwingSpeed = sourceRef.FindProperty("grappleSwingSpeed");
        grappleJumpPower = sourceRef.FindProperty("grappleJumpPower");
    }

    void GetOneWayProperties()
    {
        enableOneWayPlatforms = sourceRef.FindProperty("enableOneWayPlatforms");
        oneWayDetectZone = sourceRef.FindProperty("oneWayDetectZone");
        oneWayDisablePhysicsTime = sourceRef.FindProperty("oneWayDisablePhysicsTime");
        disablePhysicsLayer = sourceRef.FindProperty("disablePhysicsLayer");
    }

    void GetCeilingDetectionProperties()
    {
        ceilingMask = sourceRef.FindProperty("ceilingMask");
        ceilingBoxSize = sourceRef.FindProperty("ceilingBoxSize");
        ceilingBoxCenter = sourceRef.FindProperty("ceilingBoxCenter");
        curCeilingBoxCenter = sourceRef.FindProperty("curCeilingBoxCenter");
    }

    void GetWallDetectionProperties()
    {
        enableWallJump = sourceRef.FindProperty("enableWallJump");
        autoFlip = sourceRef.FindProperty("autoFlip");
        faceWallToJump = sourceRef.FindProperty("faceWallToJump");
        wallBounceForceX = sourceRef.FindProperty("wallBounceForceX");
        wallBounceForceY = sourceRef.FindProperty("wallBounceForceY");
        disableMovementOnWallJump = sourceRef.FindProperty("disableMovementOnWallJump");
        disableTime = sourceRef.FindProperty("disableTime");
    }


    void DisplayJumping()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(enableJump);

        if (enableJump.boolValue)
        {
            EditorGUILayout.PropertyField(jumpButton);
            EditorGUILayout.PropertyField(jumpStyle);
            EditorGUILayout.PropertyField(jumpPower);
            EditorGUILayout.PropertyField(enableDoubleJump);
            if (enableClimbing.boolValue)
            {
                EditorGUILayout.PropertyField(enableJumpClimbing);
            }
            EditorGUILayout.PropertyField(enableWallJump);
            if (enableWallJump.boolValue)
            {
                EditorGUILayout.PropertyField(wallBounceForceX);
                EditorGUILayout.PropertyField(wallBounceForceY);
                EditorGUILayout.PropertyField(disableMovementOnWallJump);
                if (disableMovementOnWallJump.boolValue)
                    EditorGUILayout.PropertyField(disableTime);
            }
            if (jumpStyle.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(gravityMultiplier);
                EditorGUILayout.PropertyField(lowJumpMultiplier);
            }
        }
            
    }

    void DisplayDash()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(enableDash);
        if (enableDash.boolValue)
        {
            EditorGUILayout.PropertyField(dashButton);
            EditorGUILayout.PropertyField(dashPower);
            EditorGUILayout.PropertyField(dashTime);
            EditorGUILayout.PropertyField(dashCooldown);
        }
        
    }

    void DisplayGrapple()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(enableGrappleMods);
        if (enableGrappleMods.boolValue)
        {
            EditorGUILayout.PropertyField(grappleSwingSpeed);
            EditorGUILayout.PropertyField(grappleJumpPower);
        }

    }

    void DisplayOneWay()
    {
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(enableOneWayPlatforms);
        if (enableOneWayPlatforms.boolValue)
        {
            EditorGUILayout.PropertyField(oneWayDetectZone);
            EditorGUILayout.PropertyField(oneWayDisablePhysicsTime);
            EditorGUILayout.PropertyField(disablePhysicsLayer);
        }

    }

    void DisplayCeilingDetection()
    {
        EditorGUILayout.Space();

            EditorGUILayout.PropertyField(ceilingMask);
            EditorGUILayout.PropertyField(ceilingBoxSize);
            EditorGUILayout.PropertyField(ceilingBoxCenter);
    }

    public override void DisplaySideDetection()
    {
        base.DisplaySideDetection();
        EditorGUILayout.PropertyField(autoFlip);
        EditorGUILayout.PropertyField(faceWallToJump);
    }

    public override void OnSceneGUI()
    {
        base.OnSceneGUI();

        SetGroundBox();
        SetCeilingBox();
        DrawOneWayDetectZone();

        //draw ceiling gizmo
        Handles.color = Color.green;
        Vector2 center = ceilingBoxCenter.vector2Value;
        if (Application.isPlaying)
            center = curCeilingBoxCenter.vector2Value;
        Handles.DrawWireCube((Vector2)source.transform.position + center,
            ceilingBoxSize.vector2Value);
    }

    void SetHeaderStyle()
    {
        headerStyle.fontStyle = FontStyle.Bold;
        RectOffset paddingOffset = new RectOffset(0, 0, 3, 3);
        headerStyle.padding = paddingOffset;
    }

    void SetGroundBox()
    {
        if (groundBoxSize.vector2Value == Vector2.zero)
        {
            col = source.GetComponent<Collider2D>().bounds;
            Vector3 pos = source.transform.position;
            float sizeX = col.size.x;
            groundBoxSize.vector2Value = new Vector2(sizeX, 0.15f);
        }
    }

    void SetCeilingBox()
    {
        if (ceilingBoxSize.vector2Value == Vector2.zero)
        {
            col = source.GetComponent<Collider2D>().bounds;
            Vector3 pos = source.transform.position;
            float sizeX = col.size.x;
            float sizeY = source.transform.InverseTransformPoint(pos + col.extents).y;
            pSource.ceilingBoxSize = new Vector2(sizeX, 0.15f);
            groundBoxCenter.vector2Value = new Vector2(0, sizeY);
        }
    }

    void DrawOneWayDetectZone()
    {
        if (enableOneWayPlatforms.boolValue)
            EditorExtensions.DrawDetectZone(source.transform, oneWayDetectZone);
    }
}
