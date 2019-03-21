using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBuildableData))]
public class ItemBuildableDataEditor : ItemAimableDataEditor
{

    public SerializedProperty previewPrefab;
    public SerializedProperty placedPrefab;
    public SerializedProperty validMaterial;
    public SerializedProperty invalidMaterial;
    public SerializedProperty overlapMask;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemBuildableData)target;
        source.linkedType = typeof(ItemBuildable);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        previewPrefab = sourceRef.FindProperty("previewPrefab");
        placedPrefab = sourceRef.FindProperty("placedPrefab");
        validMaterial = sourceRef.FindProperty("validMaterial");
        invalidMaterial = sourceRef.FindProperty("invalidMaterial");
        overlapMask = sourceRef.FindProperty("overlapMask");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(previewPrefab);
        EditorGUILayout.PropertyField(placedPrefab);
        EditorGUILayout.PropertyField(validMaterial);
        EditorGUILayout.PropertyField(invalidMaterial);
        EditorGUILayout.PropertyField(overlapMask);
    }

}
