using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Trigger2DEvent))]
public class Trigger2DEventEditor : Editor
{
    private Trigger2DEvent source;
    private SerializedObject sourceRef;
    //masks
    private SerializedProperty mask;
    private SerializedProperty triggerTag;
    //button
    private SerializedProperty buttonDownToTrigger;
    private SerializedProperty button;
    //delays
    private SerializedProperty delayTriggerTime;
    private SerializedProperty useRepeatDelay;
    private SerializedProperty repeatDelay;
    //events
    private SerializedProperty trigger2DEnterEvents;
    private SerializedProperty trigger2DExitEvents;
    private SerializedProperty trigger2DStayEvents;

    private void OnEnable()
    {
        source = (Trigger2DEvent)target;
        sourceRef = serializedObject;

        buttonDownToTrigger = sourceRef.FindProperty("buttonDownToTrigger");
        button = sourceRef.FindProperty("button");

        mask = sourceRef.FindProperty("mask");
        triggerTag = sourceRef.FindProperty("triggerTag");
        delayTriggerTime = sourceRef.FindProperty("delayTriggerTime");
        useRepeatDelay = sourceRef.FindProperty("useRepeatDelay");
        repeatDelay = sourceRef.FindProperty("repeatDelay");

        //events
        trigger2DEnterEvents = sourceRef.FindProperty("trigger2DEnterEvents");
        trigger2DExitEvents = sourceRef.FindProperty("trigger2DExitEvents");
        trigger2DStayEvents = sourceRef.FindProperty("trigger2DStayEvents");
    }

    public override void OnInspectorGUI()
    {
        triggerTag.stringValue = EditorGUILayout.TagField("Trigger2D Tag", triggerTag.stringValue);
        mask.intValue = EditorGUILayout.MaskField("Trigger2D Types", mask.intValue, System.Enum.GetNames(typeof(Trigger2DEvent.TriggerType)));

        EditorGUILayout.PropertyField(delayTriggerTime);

        if (mask.intValue == (mask.intValue | (1 << 0)))
        {
            EditorGUILayout.Space();
            EditorExtensions.LabelFieldCustom("Trigger Enter", Color.black, FontStyle.Bold);
            EditorGUILayout.LabelField("----------------");
            DisplayTriggerEvent(trigger2DEnterEvents);
        }

        if (mask.intValue == (mask.intValue | (1 << 1)))
        {
            EditorGUILayout.Space();
            EditorExtensions.LabelFieldCustom("Trigger Exit", Color.black, FontStyle.Bold);
            EditorGUILayout.LabelField("----------------");
            DisplayTriggerEvent(trigger2DExitEvents);
        }

        if (mask.intValue == (mask.intValue | (1 << 2)))
        {
            EditorGUILayout.Space();
            EditorExtensions.LabelFieldCustom("Trigger Stay", Color.black, FontStyle.Bold);
            EditorGUILayout.LabelField("----------------");

            EditorGUILayout.PropertyField(buttonDownToTrigger);
            if (buttonDownToTrigger.boolValue)
            {
                EditorGUILayout.PropertyField(button);
            }
            EditorGUILayout.PropertyField(useRepeatDelay);
            if (useRepeatDelay.boolValue)
            {
                EditorGUILayout.PropertyField(repeatDelay);
            }
            DisplayTriggerEvent(trigger2DStayEvents);

        }


        sourceRef.ApplyModifiedProperties();
    }

    void DisplayTriggerEvent(SerializedProperty _prop)
    {
        var useUnityEvent = _prop.FindPropertyRelative("useUnityEvent");
        var triggerEvent = _prop.FindPropertyRelative("triggerEvent");
        var triggerInteracts = _prop.FindPropertyRelative("triggerInteracts");

        EditorGUILayout.PropertyField(useUnityEvent);
        if (useUnityEvent.boolValue)
            EditorGUILayout.PropertyField(triggerEvent);

        EditorGUILayout.PropertyField(triggerInteracts, true);
    }

}
