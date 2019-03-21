using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ItemWeaponMeleeData))]
public class ItemWeaponMeleeDataEditor : ItemUseableDataEditor
{
    private SerializedProperty damage;
    private SerializedProperty damageDelay;
    private SerializedProperty activeTime;
    private SerializedProperty bounceType;
    private SerializedProperty direction;
    private SerializedProperty bounceForce;
    private SerializedProperty unitAmount;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemWeaponMeleeData)target;
        source.linkedType = typeof(ItemWeaponMelee);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        damage = sourceRef.FindProperty("damage");
        damageDelay = sourceRef.FindProperty("damageDelay");
        activeTime = sourceRef.FindProperty("activeTime");
        bounceType = sourceRef.FindProperty("bounceType");
        direction = sourceRef.FindProperty("direction");
        bounceForce = sourceRef.FindProperty("bounceForce");
        unitAmount = sourceRef.FindProperty("unitAmount");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Weapon Properties", boldStyle);
        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(damageDelay);
        EditorGUILayout.PropertyField(activeTime);
        EditorGUILayout.PropertyField(bounceType);
        if (bounceType.enumValueIndex == 2)
            EditorGUILayout.PropertyField(direction);
        EditorGUILayout.PropertyField(bounceForce);
        EditorGUILayout.PropertyField(unitAmount);
    }

}
