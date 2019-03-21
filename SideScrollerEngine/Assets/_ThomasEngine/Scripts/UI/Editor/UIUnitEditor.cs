using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UIUnit))]
public class UIUnitEditor : Editor
{
    protected UIUnit source;
    protected SerializedObject sourceRef;

    protected SerializedProperty avatarImage;
    protected SerializedProperty vitalManagerData;
    protected SerializedProperty vitalsUI;

    public virtual void OnEnable ()
    {
        source = (UIUnit)target;
        sourceRef = serializedObject;
        GetProperties ();
    }

    public override void OnInspectorGUI ()
    {
        SetProperties ();

        sourceRef.ApplyModifiedProperties ();
    }

    public virtual void GetProperties ()
    {
        avatarImage = sourceRef.FindProperty ("avatarImage");
        vitalManagerData = sourceRef.FindProperty ("vitalManagerData");
        vitalsUI = sourceRef.FindProperty ("vitalsUI");
    }

    public virtual void SetProperties ()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField (avatarImage);
        EditorGUILayout.PropertyField (vitalManagerData);
        var man = vitalManagerData.GetRootValue<ValueAmountManagerData>();
        if (man)
        {
            if (vitalsUI.arraySize != man.valueAmountLocals.Length)
                vitalsUI.arraySize = man.valueAmountLocals.Length;
            for (int i = 0; i < vitalsUI.arraySize; i++)
            {
                var element = vitalsUI.GetArrayElementAtIndex(i);
                element.objectReferenceValue = EditorGUILayout.ObjectField(man.valueAmountLocals[i].valueName, element.objectReferenceValue, typeof(UIValueAmount), true);
            }
        }
        else
            EditorExtensions.LabelFieldCustom("Need " + vitalManagerData.displayName + " to load vitals UI", Color.red, FontStyle.Bold);

    }

}