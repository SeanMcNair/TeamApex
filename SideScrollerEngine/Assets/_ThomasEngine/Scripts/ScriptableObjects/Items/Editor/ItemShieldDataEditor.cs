using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemShieldData))]
public class ItemShieldDataEditor : ItemAimableDataEditor
{
    private SerializedProperty toggleShieldButton;
    private SerializedProperty shieldPrefab;
    private SerializedProperty minDistance;
    private SerializedProperty maxDistance;
    private SerializedProperty lockToModelPrefabHeight;
    private SerializedProperty bounceProjectiles;
    private SerializedProperty useIncomingForce;
    private SerializedProperty bounceForce;
    private SerializedProperty bounceMask;
    private SerializedProperty changeDamageMaskAfterBounce;
    private SerializedProperty damageMask;
    private SerializedProperty detectSize;
    private SerializedProperty detectOffset;
    private SerializedProperty grabObjects;
    private SerializedProperty grabButton;
    private SerializedProperty deactivateShieldOnReload;
    private SerializedProperty disableUnitMovementWhenShieldActive;

    private Vector2 size;
    private Vector2 offset;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemShieldData)target;
        source.linkedType = typeof(ItemShield);
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    public override void GetProperties()
    {
        base.GetProperties();
        toggleShieldButton = sourceRef.FindProperty("toggleShieldButton");
        shieldPrefab = sourceRef.FindProperty("shieldPrefab");
        minDistance = sourceRef.FindProperty("minDistance");
        maxDistance = sourceRef.FindProperty("maxDistance");
        lockToModelPrefabHeight = sourceRef.FindProperty("lockToModelPrefabHeight");
        bounceProjectiles = sourceRef.FindProperty("bounceProjectiles");
        useIncomingForce = sourceRef.FindProperty("useIncomingForce");
        bounceForce = sourceRef.FindProperty("bounceForce");
        bounceMask = sourceRef.FindProperty("bounceMask");
        changeDamageMaskAfterBounce = sourceRef.FindProperty("changeDamageMaskAfterBounce");
        damageMask = sourceRef.FindProperty("damageMask");
        detectSize = sourceRef.FindProperty("detectSize");
        detectOffset = sourceRef.FindProperty("detectOffset");
        grabObjects = sourceRef.FindProperty("grabObjects");
        grabButton = sourceRef.FindProperty("grabButton");
        deactivateShieldOnReload = sourceRef.FindProperty("deactivateShieldOnReload");
        disableUnitMovementWhenShieldActive = sourceRef.FindProperty("disableUnitMovementWhenShieldActive");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Shield Properties", boldStyle);
        EditorGUILayout.PropertyField(toggleShieldButton);
        EditorGUILayout.PropertyField(shieldPrefab);
        EditorGUILayout.PropertyField(minDistance);
        EditorGUILayout.PropertyField(maxDistance);
        EditorGUILayout.PropertyField(lockToModelPrefabHeight);
        EditorGUILayout.PropertyField(bounceProjectiles);
        if (bounceProjectiles.boolValue)
        {
            EditorGUILayout.PropertyField(bounceMask);
            EditorGUILayout.PropertyField(useIncomingForce);
            if (!useIncomingForce.boolValue)
                EditorGUILayout.PropertyField(bounceForce);
            EditorGUILayout.PropertyField(changeDamageMaskAfterBounce);
            if (changeDamageMaskAfterBounce.boolValue)
                EditorGUILayout.PropertyField(damageMask);
            EditorGUILayout.PropertyField(detectSize);
            EditorGUILayout.PropertyField(detectOffset);
            EditorGUILayout.PropertyField(grabObjects);
            if (grabObjects.boolValue)
                EditorGUILayout.PropertyField(grabButton);
            EditorGUILayout.PropertyField(deactivateShieldOnReload);
            EditorGUILayout.PropertyField(disableUnitMovementWhenShieldActive);
        }
        SetGUIValues();
    }

    void SetGUIValues()
    {
        size = detectSize.vector2Value;
        offset = detectOffset.vector2Value;
    }

    public override void OnSceneGUI(SceneView _sceneView)
    {
        if (!Application.isPlaying)
            return;
        ItemShield shieldComp = null;
        var objRefs = (ItemShield[])FindObjectsOfType(typeof(ItemShield));
        if (objRefs.Length > 0)
        {
            foreach (var obj in objRefs)
            {
                if (obj.Data == source)
                    shieldComp = obj;
            }
        }
        if (shieldComp)
        {
            var shield = shieldComp.spawnedShield;
            if (shield)
            {

                    //need to create a new handles matrix space just for the box so we can rotate the "angle"...this space is the new Zero position.
                    Matrix4x4 boxMatrix = Matrix4x4.TRS((Vector2)shield.transform.position + offset, Quaternion.Euler(0, 0, shield.transform.eulerAngles.z), Handles.matrix.lossyScale);
                    using (new Handles.DrawingScope(boxMatrix))
                    {
                        Handles.DrawWireCube(Vector2.zero, size);
                    }

            }
        }
    }
}
