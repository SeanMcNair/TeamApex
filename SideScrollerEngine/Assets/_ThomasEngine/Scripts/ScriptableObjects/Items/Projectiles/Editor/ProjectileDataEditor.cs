using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[CustomEditor(typeof(ProjectileData))]
public class ProjectileDataEditor : LinkableDataEditor
{
    protected SerializedProperty lifeTime;
    protected SerializedProperty hitMaxAmount;
    protected SerializedProperty spawnOnImpact;
    protected SerializedProperty interactFX;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ProjectileData)target;
        source.linkedType = typeof(Projectile);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        lifeTime = sourceRef.FindProperty("lifeTime");
        hitMaxAmount = sourceRef.FindProperty("hitMaxAmount");
        spawnOnImpact = sourceRef.FindProperty("spawnOnImpact");
        interactFX = sourceRef.FindProperty("interactFX");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(hitMaxAmount);
        EditorGUILayout.PropertyField(lifeTime);
        EditorGUILayout.PropertyField(spawnOnImpact, true);
        EditorGUILayout.PropertyField(interactFX, true);

    }

}
