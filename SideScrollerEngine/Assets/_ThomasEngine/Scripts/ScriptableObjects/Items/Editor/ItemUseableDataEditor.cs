using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Web;
using System;

[CustomEditor(typeof(ItemData))]
public class ItemUseableDataEditor : ItemDataEditor
{
    protected SerializedProperty overrideUsage;
    protected SerializedProperty useType;
    protected SerializedProperty removeAmount;
    protected SerializedProperty recoilTime;
    protected SerializedProperty instantFirstUse;
    protected SerializedProperty constantRemoveSpeed;

    protected SerializedProperty ammoType;
    protected SerializedProperty localAmmo;
    protected SerializedProperty globalAmmoManager;
    protected SerializedProperty globalAmmo;

    protected SerializedProperty onEmpty;

    protected string[] customButtonNames;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemUseableData)target;
        source.linkedType = typeof(ItemUseable);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        overrideUsage = sourceRef.FindProperty("overrideUsage");
        useType = sourceRef.FindProperty("useType");
        removeAmount = sourceRef.FindProperty("removeAmount");
        recoilTime = sourceRef.FindProperty("recoilTime");
        instantFirstUse = sourceRef.FindProperty("instantFirstUse");
        constantRemoveSpeed = sourceRef.FindProperty("constantRemoveSpeed");

        ammoType = sourceRef.FindProperty("ammoType");
        localAmmo = sourceRef.FindProperty("localAmmo");
        globalAmmoManager = sourceRef.FindProperty("globalAmmoManager");
        globalAmmo = sourceRef.FindProperty("globalAmmo");

        onEmpty = sourceRef.FindProperty("onEmpty");
        
    }

    public override void SetProperties()
    {
        base.SetProperties();
        DisplayUsageProperties();
        DisplayAmmoProperties();   
    }

    protected virtual void DisplayUsageProperties()
    {
        if (overrideUsage.boolValue)
            return;

        EditorGUILayout.LabelField("Usage Properties", boldStyle);

        EditorGUILayout.PropertyField(useType);
        if (useType.enumValueIndex != 2)
        {
            if (ammoType.enumValueIndex != 0)
                EditorGUILayout.PropertyField(removeAmount);
            EditorGUILayout.PropertyField(recoilTime);
            if (useType.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(instantFirstUse);
            }

        }
        else if (useType.enumValueIndex == 2)
        {
            if (ammoType.enumValueIndex != 0)
            {
                if (ammoType.enumValueIndex == 2)
                    constantRemoveSpeed.FloatFieldClamp(1, Mathf.Infinity);
                else
                    EditorGUILayout.PropertyField(constantRemoveSpeed);
            }

        }

    }

    protected virtual void DisplayAmmoProperties()
    {
        EditorGUILayout.LabelField("Ammo Properties", boldStyle);
        EditorGUILayout.PropertyField(ammoType);
        if (ammoType.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(localAmmo, true);
        }
        else if (ammoType.enumValueIndex == 2)
        {
            EditorGUILayout.PropertyField(globalAmmoManager);
            var manager = globalAmmoManager.GetRootValue<ValueAmountManagerData>();
            if (manager)
            {
                string[] names = new string[manager.valueAmountLocals.Length];
                for (int i = 0; i < manager.valueAmountLocals.Length; i++)
                {
                    names[i] = manager.valueAmountLocals[i].valueName;
                }
                var global = globalAmmo.GetRootValue<IndexStringProperty>();
                global.stringValues = names;
                EditorGUILayout.PropertyField(globalAmmo);
            }

        }

        EditorGUILayout.PropertyField(onEmpty);

    }
}
