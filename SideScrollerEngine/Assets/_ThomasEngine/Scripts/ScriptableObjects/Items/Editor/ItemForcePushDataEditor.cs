using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemForcePushData))]
public class ItemForcePushDataEditor : ItemAimableDataEditor
{
    public SerializedProperty pushProperty;
    public SerializedProperty pullProperty;
    public SerializedProperty mask;
    public SerializedProperty maxObjects;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemForcePushData)target;
        source.linkedType = typeof(ItemForcePush);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        pushProperty = sourceRef.FindProperty("pushProperty");
        pullProperty = sourceRef.FindProperty("pullProperty");
        mask = sourceRef.FindProperty("mask");
        maxObjects = sourceRef.FindProperty("maxObjects");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Force Push Properties", boldStyle);
        EditorGUILayout.PropertyField(pushProperty, true);
        var push = pushProperty.GetRootValue<ItemForcePushData.PhysicsProperty>();
        push.button.stringValues = customButtonNames;
        EditorGUILayout.PropertyField(pullProperty, true);
        var pull = pullProperty.GetRootValue<ItemForcePushData.PhysicsProperty>();
        pull.button.stringValues = customButtonNames;
        EditorGUILayout.PropertyField(mask);
        EditorGUILayout.PropertyField(maxObjects);
    }
}
