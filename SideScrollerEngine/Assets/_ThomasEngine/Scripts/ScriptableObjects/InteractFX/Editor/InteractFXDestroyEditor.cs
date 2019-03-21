using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractFXDestroy))]
public class InteractFXDestroyEditor : Editor
{
    protected SerializedObject sourceRef;

    private SerializedProperty destroyType;
    private SerializedProperty destroyRoot;
    private SerializedProperty delay;
    private SerializedProperty closestTag;
    private SerializedProperty nameToFind;

    void OnEnable()
    {
        sourceRef = serializedObject;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        sourceRef.ApplyModifiedProperties();
    }

    void GetProperties()
    {
        destroyType = sourceRef.FindProperty("destroyType");
        destroyRoot = sourceRef.FindProperty("destroyRoot");
        delay = sourceRef.FindProperty("delay");
        closestTag = sourceRef.FindProperty("closestTag");
        nameToFind = sourceRef.FindProperty("nameToFind");
    }

    void SetProperties()
    {
        EditorGUILayout.PropertyField(destroyType);
        if (destroyType.enumValueIndex == 2)
            EditorGUILayout.PropertyField(closestTag);
        if (destroyType.enumValueIndex == 3)
            EditorGUILayout.PropertyField(nameToFind);
        EditorGUILayout.PropertyField(destroyRoot);
        EditorGUILayout.PropertyField(delay);
    }

}
