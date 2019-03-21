using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(UnitController))]
public class UnitControllerEditor : Editor
{

    protected UnitController source;
    public UnitController Source { get { return source; } }
    protected SerializedObject sourceRef;

    //direction
    protected SerializedProperty faceRightAtStart;

    //aiming
    protected SerializedProperty enableAiming;
    protected SerializedProperty turnTowardsAimPos;
    protected SerializedProperty ignoreZRotation;
    protected SerializedProperty consistentUpDirection;

    //ground
    protected SerializedProperty groundMask;
    protected SerializedProperty platformLayer;
    protected SerializedProperty groundBoxSize;
    protected SerializedProperty groundBoxCenter;

    //side detection
    protected SerializedProperty sideMask;
    protected SerializedProperty leftDetectSize;
    protected SerializedProperty leftDetectCenter;
    protected SerializedProperty rightDetectSize;
    protected SerializedProperty rightDetectCenter;

    protected Bounds col;

    public virtual void OnEnable()
    {
        source = (UnitController)target;
        sourceRef = serializedObject;
        GetProperties();
        SetGroundBox();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();

        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        faceRightAtStart = sourceRef.FindProperty("faceRightAtStart");

        //aiming
        enableAiming = sourceRef.FindProperty("enableAiming");
        turnTowardsAimPos = sourceRef.FindProperty("turnTowardsAimPos");
        ignoreZRotation = sourceRef.FindProperty("ignoreZRotation");
        consistentUpDirection = sourceRef.FindProperty("consistentUpDirection");

        //ground
        groundMask = sourceRef.FindProperty("groundMask");
        platformLayer = sourceRef.FindProperty("platformLayer");
        groundBoxSize = sourceRef.FindProperty("groundBoxSize");
        groundBoxCenter = sourceRef.FindProperty("groundBoxCenter");

        //side
        sideMask = sourceRef.FindProperty("sideMask");
        leftDetectSize = sourceRef.FindProperty("leftDetectSize");
        leftDetectCenter = sourceRef.FindProperty("leftDetectCenter");
        rightDetectSize = sourceRef.FindProperty("rightDetectSize");
        rightDetectCenter = sourceRef.FindProperty("rightDetectCenter");

    }

    void SetGroundBox()
    {
        if (groundBoxSize.vector2Value == Vector2.zero)
        {
            col = source.GetComponent<Collider2D>().bounds;
            Vector3 pos = source.transform.position;
            float sizeX = col.size.x;
            float sizeY = source.transform.InverseTransformPoint(pos - col.extents).y; 
            groundBoxSize.vector2Value = new Vector2(sizeX,0.15f);
            groundBoxCenter.vector2Value = new Vector2(0, sizeY);
        }
    }

    public virtual void SetProperties()
    {
        DisplayMovement();
        DisplayAiming();
        DisplayGroundDetection();
        DisplaySideDetection();
    }

    public virtual void DisplayMovement()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(faceRightAtStart);

    }

    public virtual void DisplayAiming()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(enableAiming);
        if (enableAiming.boolValue)
        {
            EditorGUILayout.PropertyField(turnTowardsAimPos);
            if (turnTowardsAimPos.boolValue)
            {
                EditorGUILayout.PropertyField(ignoreZRotation);
                EditorGUILayout.PropertyField(consistentUpDirection);
            }
        }
    }

    public virtual void DisplayGroundDetection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(groundMask);
        EditorGUILayout.PropertyField(platformLayer);
        EditorGUILayout.PropertyField(groundBoxSize);
        EditorGUILayout.PropertyField(groundBoxCenter);
    }

    public virtual void DisplaySideDetection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(sideMask);
        EditorGUILayout.PropertyField(leftDetectCenter);
        EditorGUILayout.PropertyField(leftDetectSize);
        EditorGUILayout.PropertyField(rightDetectCenter);
        EditorGUILayout.PropertyField(rightDetectSize);
    }

    public virtual void OnSceneGUI()
    {
        DrawGroundedGizmos();
        DrawSideGizmos();

        sourceRef.ApplyModifiedProperties();
    }

    public virtual void DrawGroundedGizmos()
    {
        //draw grounded gizmo
        Handles.color = Color.cyan;
        Handles.DrawWireCube((Vector2)source.transform.position + groundBoxCenter.vector2Value,
            groundBoxSize.vector2Value);

    }

    public virtual void DrawSideGizmos()
    {
        //draw collision gizmos
        Handles.color = Color.magenta;
        Handles.DrawWireCube((Vector2)source.transform.position + leftDetectCenter.vector2Value,
            leftDetectSize.vector2Value);
        Handles.DrawWireCube((Vector2)source.transform.position + rightDetectCenter.vector2Value,
            rightDetectSize.vector2Value);
    }

}
