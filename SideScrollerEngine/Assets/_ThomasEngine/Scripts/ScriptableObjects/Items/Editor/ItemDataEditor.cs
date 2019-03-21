using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Web;
using System;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : LinkableDataEditor
{
    private SerializedProperty icon;
    private SerializedProperty spawnUI;
    private SerializedProperty itemUI;
    private SerializedProperty setOwnerAsParent;
    private SerializedProperty keepUIActiveIfDropped;
    private SerializedProperty value;
    private SerializedProperty weight;
    private SerializedProperty droppable;
    private SerializedProperty droppedPrefab;
    private SerializedProperty quickMenuCompatible;
    private SerializedProperty buffs;

    protected GUIStyle boldStyle;

    public override void OnEnable()
    {
        base.OnEnable();
        SetGUIStyle();
        source = (ItemData)target;
        source.linkedType = typeof(Item);
    }

    void SetGUIStyle()
    {
        boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
        };
    }

    public override void GetProperties()
    {
        base.GetProperties();
        icon = sourceRef.FindProperty("icon");
        spawnUI = sourceRef.FindProperty("spawnUI");
        itemUI = sourceRef.FindProperty("itemUI");
        setOwnerAsParent = sourceRef.FindProperty("setOwnerAsParent");
        keepUIActiveIfDropped = sourceRef.FindProperty("keepUIActiveIfDropped");
        value = sourceRef.FindProperty("value");
        weight = sourceRef.FindProperty("weight");
        droppable = sourceRef.FindProperty("droppable");
        droppedPrefab = sourceRef.FindProperty("droppedPrefab");
        quickMenuCompatible = sourceRef.FindProperty("quickMenuCompatible");
        buffs = sourceRef.FindProperty("buffs");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(spawnUI);
        if (spawnUI.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorExtensions.PrefabFieldWithComponent(itemUI, typeof(UIValueAmount));
            EditorGUILayout.PropertyField(setOwnerAsParent);
            EditorGUILayout.PropertyField(keepUIActiveIfDropped);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(value);
        EditorGUILayout.PropertyField(weight);
        EditorGUILayout.PropertyField(droppable);
        if (droppable.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(droppedPrefab);
            EditorGUI.indentLevel--;
        }  
        EditorGUILayout.PropertyField(quickMenuCompatible);
        EditorGUILayout.PropertyField(buffs, true);
    }

}
