using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDrawableData))]
public class ItemDrawableDataEditor : ItemAimableDataEditor
{
    public SerializedProperty drawButton;
    public SerializedProperty topPrefab;
    public SerializedProperty growLinePrefab;
    public SerializedProperty trailRenderer;
    public SerializedProperty sensitivityDistance;
    public SerializedProperty growSpeed;
    public SerializedProperty useMask;
    public SerializedProperty drawableMask;
    public SerializedProperty blockableMask;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemDrawableData)target;
        source.linkedType = typeof(ItemDrawable);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        drawButton = sourceRef.FindProperty("drawButton");
        topPrefab = sourceRef.FindProperty("topPrefab");
        growLinePrefab = sourceRef.FindProperty("growLinePrefab");
        trailRenderer = sourceRef.FindProperty("trailRenderer");
        sensitivityDistance = sourceRef.FindProperty("sensitivityDistance");
        growSpeed = sourceRef.FindProperty("growSpeed");
        useMask = sourceRef.FindProperty("useMask");
        drawableMask = sourceRef.FindProperty("drawableMask");
        blockableMask = sourceRef.FindProperty("blockableMask");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Drawing Properties", boldStyle);
        EditorGUILayout.PropertyField(drawButton);
        EditorGUILayout.PropertyField(topPrefab);
        EditorGUILayout.PropertyField(growLinePrefab);
        EditorGUILayout.PropertyField(trailRenderer);
        EditorGUILayout.PropertyField(sensitivityDistance);
        EditorGUILayout.PropertyField(growSpeed);
        EditorGUILayout.PropertyField(useMask);
        if (useMask.boolValue)
        {
            EditorGUILayout.PropertyField(drawableMask);
            EditorGUILayout.PropertyField(blockableMask);
        }
            

    }

}
