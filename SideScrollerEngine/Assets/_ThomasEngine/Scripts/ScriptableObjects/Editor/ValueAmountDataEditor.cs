using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ValueAmountData))]
public class ValueAmountDataEditor : Editor
{
    protected SerializedObject sourceRef;
    protected ValueAmountData source;

    protected SerializedProperty valueType;
    protected SerializedProperty maxSize;
    protected SerializedProperty reloadTime;
    protected SerializedProperty rechargeSpeed;
    protected SerializedProperty overheat;
    protected SerializedProperty overheatTime;

    public virtual void OnEnable()
    {
        source = (ValueAmountData)target;
        sourceRef = serializedObject;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        valueType = sourceRef.FindProperty("valueType");
        maxSize = sourceRef.FindProperty("maxSize");
        reloadTime = sourceRef.FindProperty("reloadTime");
        rechargeSpeed = sourceRef.FindProperty("rechargeSpeed");
        overheat = sourceRef.FindProperty("overheat");
        overheatTime = sourceRef.FindProperty("overheatTime");
    }

    public virtual void SetProperties()
    {
        EditorGUILayout.PropertyField(valueType);
        if (valueType.enumValueIndex == 0)
            return;

        EditorGUILayout.PropertyField(maxSize);
        if (valueType.enumValueIndex == 2)
            EditorGUILayout.PropertyField(reloadTime);
        else if (valueType.enumValueIndex == 3)
        {
            EditorGUILayout.PropertyField(rechargeSpeed);
            EditorGUILayout.PropertyField(overheat);
            if (overheat.boolValue)
                EditorGUILayout.PropertyField(overheatTime);
        }

    }

}
