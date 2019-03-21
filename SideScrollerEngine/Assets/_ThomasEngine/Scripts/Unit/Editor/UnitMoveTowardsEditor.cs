using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UnitMoveTowards))]
public class UnitMoveTowardsEditor : DetectZoneTriggerEditor
{

    protected SerializedProperty moveType;
    protected SerializedProperty moveToPoint;

    protected SerializedProperty speed;
    protected SerializedProperty maxArrivalDistance;
    protected SerializedProperty maxChaseDistance;
    protected SerializedProperty maxHeightDifference;

    protected SerializedProperty activateArrivalEvents;
    protected SerializedProperty unityEvents;

    public override void OnInspectorGUI ()
    {
        SetProperties ();

        sourceRef.ApplyModifiedProperties ();
    }

    public override void GetProperties ()
    {
        base.GetProperties();
        moveType = sourceRef.FindProperty ("moveType");
        moveToPoint = sourceRef.FindProperty ("moveToPoint");
        speed = sourceRef.FindProperty ("speed");
        maxArrivalDistance = sourceRef.FindProperty ("maxArrivalDistance");
        maxChaseDistance = sourceRef.FindProperty ("maxChaseDistance");
        maxHeightDifference = sourceRef.FindProperty ("maxHeightDifference");

        //arrival events
        activateArrivalEvents = sourceRef.FindProperty("activateArrivalEvents");
        unityEvents = sourceRef.FindProperty("unityEvents");

    }

    public override void SetProperties ()
    {
        DisplayMovementProperties();
        DisplayArrivalProperties();
    }

    public virtual void DisplayMovementProperties()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(moveType);
        if (moveType.enumValueIndex == 0)
            base.SetProperties();
        EditorGUILayout.Space();
        if (moveType.enumValueIndex == 1)
        {
            if (!moveToPoint.objectReferenceValue)
                EditorGUILayout.LabelField("DRAG TRANSFORM YOU WANT THIS UNIT TO MOVE TOWARD");

            EditorGUILayout.PropertyField(moveToPoint);
        }
        EditorGUILayout.LabelField("Movement", boldStyle);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(maxArrivalDistance);
        EditorGUILayout.PropertyField(maxChaseDistance);
        EditorGUILayout.PropertyField(maxHeightDifference);
    }

    public virtual void DisplayArrivalProperties()
    {
        //arrival
        EditorGUILayout.LabelField("On Arrival", boldStyle);
        EditorGUILayout.PropertyField(activateArrivalEvents);
        if (activateArrivalEvents.boolValue)
        {
            EditorGUILayout.PropertyField(unityEvents);
        }
    }

    public override void OnSceneGUI()
    {
        base.OnSceneGUI();

        //chase disc
        Color handleColorChase = new Color(0, 255, 0, 0.5f);
        Handles.color = handleColorChase;
        Handles.DrawWireDisc(source.transform.position,Vector3.back, maxChaseDistance.floatValue);
        Vector2 labelPos = new Vector2(source.transform.position.x,source.transform.position.y + maxChaseDistance.floatValue);
        Handles.Label(labelPos, "Chase Distance");

        //arrival disc
        Color handleColorArrival = new Color(255, 0, 0, 0.5f);
        Handles.color = handleColorArrival;
        Handles.DrawWireDisc(source.transform.position, Vector3.back, maxArrivalDistance.floatValue);
        Vector2 labelPosArrival = new Vector2(source.transform.position.x, source.transform.position.y - maxArrivalDistance.floatValue);
        Handles.Label(labelPosArrival, "Arrival Distance");
    }

}