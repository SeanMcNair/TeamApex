using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : UnitControllerEditor
{

    public new EnemyController Source { get { return (EnemyController)source; } }
    //movement
    private SerializedProperty movementType;
    private SerializedProperty speed;
    //boundaries
    private SerializedProperty useBoundaries;
    private SerializedProperty resetBoundariesOnEnable;
    private SerializedProperty resetYPositionOnPatrolResume;
    private SerializedProperty resetSpeed;
    private SerializedProperty leftOffset;
    private SerializedProperty leftBoundary;
    private SerializedProperty rightOffset;
    private SerializedProperty rightBoundary;
    //collision hits
    private SerializedProperty flipOnCollision;
    //bounce
    private SerializedProperty bounce;
    private SerializedProperty bouncePower;
    private SerializedProperty bounceXAddedForce;
    private SerializedProperty bounceDelay;

    private Vector2 handleLeft;
    private Vector2 handleRight;

    private bool animFoldout;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void GetProperties()
    {
        base.GetProperties();

        faceRightAtStart = sourceRef.FindProperty("faceRightAtStart");
        //movement
        movementType = sourceRef.FindProperty("movementType");
        useBoundaries = sourceRef.FindProperty("useBoundaries");
        resetBoundariesOnEnable = sourceRef.FindProperty("resetBoundariesOnEnable");
        resetYPositionOnPatrolResume = sourceRef.FindProperty("resetYPositionOnPatrolResume");
        resetSpeed = sourceRef.FindProperty("resetSpeed");
        leftOffset = sourceRef.FindProperty("leftOffset");
        leftBoundary = sourceRef.FindProperty("leftBoundary");
        rightOffset = sourceRef.FindProperty("rightOffset");
        rightBoundary = sourceRef.FindProperty("rightBoundary");
        //collision hits
        flipOnCollision = sourceRef.FindProperty("flipOnCollision");
        //bounce
        bounce = sourceRef.FindProperty("bounce");
        bouncePower = sourceRef.FindProperty("bouncePower");
        bounceXAddedForce = sourceRef.FindProperty("bounceXAddedForce");
        bounceDelay = sourceRef.FindProperty("bounceDelay");

    }

    public override void SetProperties()
    {
        DisplayMovement();
        DisplayAiming();
        DisplayBounce();
        if (movementType.enumValueIndex == 1)
        {
            DisplayBoundaries();
            DisplayGroundDetection();
            DisplaySideDetection();
        }

    }

    void DisplayBoundaries()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(useBoundaries);
        if (useBoundaries.boolValue)
        {
            EditorGUILayout.PropertyField(resetBoundariesOnEnable);
            EditorGUILayout.PropertyField(resetYPositionOnPatrolResume);
            if (resetYPositionOnPatrolResume.boolValue)
                EditorGUILayout.PropertyField(resetSpeed);
            EditorGUILayout.PropertyField(leftOffset);
            EditorGUILayout.PropertyField(rightOffset);
        }
    }

    void DisplayBounce()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(bounce);
        if (bounce.boolValue)
        {
            EditorGUILayout.PropertyField(bouncePower);
            EditorGUILayout.PropertyField(bounceDelay);
            if (movementType.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(bounceXAddedForce);
            }

        }
    }

    public override void DisplayMovement()
    {
        base.DisplayMovement();
        EditorGUILayout.PropertyField(movementType);
    }

    public override void DisplaySideDetection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(flipOnCollision);
        if (!flipOnCollision.boolValue)
            return;

        base.DisplaySideDetection();
    }

    public override void OnSceneGUI()
    {
        base.OnSceneGUI();
        if (movementType.enumValueIndex == 0)
            return;

        if (useBoundaries.boolValue)
            DrawPositionHandles();

    }

    public override void DrawSideGizmos()
    {
        if (!flipOnCollision.boolValue)
            return;
        base.DrawSideGizmos();
    }

    void DrawPositionHandles()
    {

        InitializeHandlePositions();


        if (!Application.isPlaying)
        {
            leftOffset.DrawHandleTransformPoint(source, source.transform, true, leftBoundary);
            rightOffset.DrawHandleTransformPoint(source, source.transform, true, rightBoundary);
            //draw icons
            DrawSideLines(source.transform.TransformPoint(leftOffset.GetRootValue<Vector2>()), "Left Pos");
            DrawSideLines(source.transform.TransformPoint(rightOffset.GetRootValue<Vector2>()), "Right Pos");
        }
        else
        {
            //draw icons
            DrawSideLines(leftBoundary.vector2Value, "Left Pos");
            DrawSideLines(rightBoundary.vector2Value, "Right Pos");
        }
           


    }

    void InitializeHandlePositions()
    {
        //set Initial positions for other points so they don't all start at zero zero
        if (leftOffset.vector2Value == Vector2.zero)
        {
            Source.leftOffset = Vector2.left * 3;
        }
        if (rightOffset.vector2Value == Vector2.zero)
        {
            Source.rightOffset = Vector2.right * 3;
        }
    }

    void DrawSideLines(Vector2 _pos, string _labelName)
    {
        Handles.color = Color.white;
        Handles.Label(_pos, _labelName);
        Handles.DrawSolidDisc(_pos, Vector3.back, 0.3f);
        Handles.DrawDottedLine(new Vector2(_pos.x, _pos.y - 5), new Vector2(_pos.x, _pos.y + 5), 5);
    }

}
