using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemHookShotData))]
public class ItemHookShotDataEditor : ItemAimableDataEditor
{
    private SerializedProperty shootType;
    private SerializedProperty shootButton;
    private SerializedProperty cancelButton;
    private SerializedProperty hookPrefab;
    private SerializedProperty lineRenderer;
    private SerializedProperty damageMask;
    private SerializedProperty damage;
    private SerializedProperty damageRadius;
    private SerializedProperty retractOnDamage;
    private SerializedProperty fireSpeed;
    private SerializedProperty lifeTime;
    private SerializedProperty dragType;
    private SerializedProperty dragSpeed;
    private SerializedProperty controlSpeed;
    private SerializedProperty minDistance;
    private SerializedProperty hookableSurfaceMask;
    private SerializedProperty cancelShotMask;
    private SerializedProperty obstacleCollisionMask;
    private SerializedProperty collisionRadius;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemHookShotData)target;
        source.linkedType = typeof(ItemHookShot);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        shootType = sourceRef.FindProperty("shootType");
        shootButton = sourceRef.FindProperty("shootButton");
        cancelButton = sourceRef.FindProperty("cancelButton");
        hookPrefab = sourceRef.FindProperty("hookPrefab");
        lineRenderer = sourceRef.FindProperty("lineRenderer");
        damageMask = sourceRef.FindProperty("damageMask");
        damage = sourceRef.FindProperty("damage");
        damageRadius = sourceRef.FindProperty("damageRadius");
        retractOnDamage = sourceRef.FindProperty("retractOnDamage");
        fireSpeed = sourceRef.FindProperty("fireSpeed");
        lifeTime = sourceRef.FindProperty("lifeTime");
        dragType = sourceRef.FindProperty("dragType");
        dragSpeed = sourceRef.FindProperty("dragSpeed");
        controlSpeed = sourceRef.FindProperty("controlSpeed");
        minDistance = sourceRef.FindProperty("minDistance");
        hookableSurfaceMask = sourceRef.FindProperty("hookableSurfaceMask");
        cancelShotMask = sourceRef.FindProperty("cancelShotMask");
        obstacleCollisionMask = sourceRef.FindProperty("obstacleCollisionMask");
        collisionRadius = sourceRef.FindProperty("collisionRadius");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorExtensions.LabelFieldCustom("HookShot Properties", Color.black, FontStyle.Bold);
        EditorGUILayout.PropertyField(shootType);
        EditorGUILayout.PropertyField(shootButton);
        EditorGUILayout.PropertyField(cancelButton);
        EditorGUILayout.PropertyField(hookPrefab);
        EditorGUILayout.PropertyField(lineRenderer);
        EditorGUILayout.PropertyField(damageMask);
        if (damageMask.intValue != 0)
        {
            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(damageRadius);
            EditorGUILayout.PropertyField(retractOnDamage);
        }
        EditorGUILayout.PropertyField(fireSpeed);
        EditorGUILayout.PropertyField(lifeTime);
        EditorGUILayout.PropertyField(dragType);
        if (dragType.enumValueIndex == 0)
            EditorGUILayout.PropertyField(dragSpeed);
        else
            EditorGUILayout.PropertyField(controlSpeed);
        EditorGUILayout.PropertyField(minDistance);
        EditorGUILayout.PropertyField(hookableSurfaceMask);
        EditorGUILayout.PropertyField(cancelShotMask);
        EditorGUILayout.PropertyField(obstacleCollisionMask);
        EditorGUILayout.PropertyField(collisionRadius);
    }

}
