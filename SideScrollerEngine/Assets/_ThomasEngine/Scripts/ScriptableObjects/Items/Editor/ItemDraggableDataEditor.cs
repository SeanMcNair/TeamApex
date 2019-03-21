using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDraggableData))]
public class ItemDraggableDataEditor : ItemAimableDataEditor
{
    public SerializedProperty dragButton;
    public SerializedProperty removeAmmoSpeed;
    public SerializedProperty dragZone;
    public SerializedProperty dragSensitivity;
    public SerializedProperty sensitivityByWeight;
    public SerializedProperty minWeight;
    public SerializedProperty maxWeight;
    public SerializedProperty throwForce;
    public SerializedProperty consistentThrowPower;
    public SerializedProperty allowDragThroughColliders;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemDraggableData)target;
        source.linkedType = typeof(ItemDraggable);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        dragButton = sourceRef.FindProperty("dragButton");
        removeAmmoSpeed = sourceRef.FindProperty("removeAmmoSpeed");
        dragZone = sourceRef.FindProperty("dragZone");
        dragSensitivity = sourceRef.FindProperty("dragSensitivity");
        sensitivityByWeight = sourceRef.FindProperty("sensitivityByWeight");
        minWeight = sourceRef.FindProperty("minWeight");
        maxWeight = sourceRef.FindProperty("maxWeight");
        throwForce = sourceRef.FindProperty("throwForce");
        consistentThrowPower = sourceRef.FindProperty("consistentThrowPower");
        allowDragThroughColliders = sourceRef.FindProperty("allowDragThroughColliders");



    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Draggable Properties", boldStyle);
        EditorGUILayout.PropertyField(dragButton);
        EditorGUILayout.PropertyField(removeAmmoSpeed);
        EditorGUILayout.PropertyField(dragZone, true);
        EditorGUILayout.PropertyField(dragSensitivity);
        EditorGUILayout.PropertyField(sensitivityByWeight);
        if (sensitivityByWeight.boolValue)
        {
            EditorGUILayout.PropertyField(minWeight);
            EditorGUILayout.PropertyField(maxWeight);
        }
        EditorGUILayout.PropertyField(throwForce);
        EditorGUILayout.PropertyField(consistentThrowPower);
        EditorGUILayout.PropertyField(allowDragThroughColliders);



    }

}
