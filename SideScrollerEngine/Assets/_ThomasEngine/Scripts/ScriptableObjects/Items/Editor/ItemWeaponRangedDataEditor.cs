using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ItemWeaponRangedData))]
public class ItemWeaponRangedDataEditor : ItemAimableDataEditor
{
    private SerializedProperty spreadType;
    private SerializedProperty fireType;
    private SerializedProperty damage;
    private SerializedProperty instantFirstShot;
    private SerializedProperty fireDelay;
    private SerializedProperty fireAmount;
    private SerializedProperty angle;
    private SerializedProperty randomAmount;
    private SerializedProperty fireDistance;
    private SerializedProperty projectile;
    private SerializedProperty projectileSpeed;
    public SerializedProperty aimTransformAtTarget;
    public SerializedProperty aimTrans;
    private SerializedProperty mask;

    private string[] childNames;
    private GameObject lastPrefab;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemWeaponRangedData)target;
        source.linkedType = typeof(ItemWeaponRanged);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        spreadType = sourceRef.FindProperty("spreadType");
        fireType = sourceRef.FindProperty("fireType");
        fireDelay = sourceRef.FindProperty("fireDelay");
        instantFirstShot = sourceRef.FindProperty("instantFirstShot");
        damage = sourceRef.FindProperty("damage");
        fireAmount = sourceRef.FindProperty("fireAmount");
        angle = sourceRef.FindProperty("angle");
        randomAmount = sourceRef.FindProperty("randomAmount");
        fireDistance = sourceRef.FindProperty("fireDistance");
        projectile = sourceRef.FindProperty("projectile");
        projectileSpeed = sourceRef.FindProperty("projectileSpeed");
        aimTransformAtTarget = sourceRef.FindProperty("aimTransformAtTarget");
        aimTrans = sourceRef.FindProperty("aimTrans");
        mask = sourceRef.FindProperty("mask");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Weapon Properties", boldStyle);
        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(instantFirstShot);
        EditorGUILayout.PropertyField(fireDelay);
        EditorGUILayout.PropertyField(spreadType);
        if (spreadType.enumValueIndex != 0)
        {
            EditorGUILayout.PropertyField(fireAmount);
            EditorGUILayout.PropertyField(angle);
            if (spreadType.enumValueIndex == 2)
                EditorGUILayout.PropertyField(randomAmount);
        }
        EditorGUILayout.PropertyField(fireType);
        if (fireType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(fireDistance);
        }
        else
        {
            EditorExtensions.PrefabFieldWithComponent(projectile, typeof(Projectile));
            EditorGUILayout.PropertyField(projectileSpeed);
        }
        EditorGUILayout.PropertyField(aimTransformAtTarget);
        if (aimTransformAtTarget.boolValue)
            EditorGUILayout.PropertyField(aimTrans);
        EditorGUILayout.PropertyField(mask);
    }

}
