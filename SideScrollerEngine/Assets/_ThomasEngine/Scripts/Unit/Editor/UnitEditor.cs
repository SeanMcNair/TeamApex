using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor
{
    protected Unit source;
    protected SerializedObject sourceRef;
    //skins
    protected SerializedProperty data;
    protected SerializedProperty skinOptions;
    protected SerializedProperty childSkinOverride;
    protected SerializedProperty spawnSkinOverride;
    protected SerializedProperty itemSpawnLocation;
    protected SerializedProperty spawnUI;
    protected SerializedProperty UIToSpawn;
    protected SerializedProperty parentUIToUnit;
    protected SerializedProperty disableOnDeath;
    //enemy configs
    protected SerializedProperty attackTarget;


    public virtual void OnEnable()
    {
        sourceRef = serializedObject;
        source = (Unit)target;
        GetProperties();
    }

    public override void OnInspectorGUI()
    {

        SetProperties();

        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        //enemy
        attackTarget = sourceRef.FindProperty("attackTarget");
        //skins
        data = sourceRef.FindProperty("data");
        childSkinOverride = sourceRef.FindProperty("childSkinOverride");
        skinOptions = sourceRef.FindProperty("skinOptions");
        spawnSkinOverride = sourceRef.FindProperty("spawnSkinOverride");
        itemSpawnLocation = sourceRef.FindProperty("itemSpawnLocation");
        spawnUI = sourceRef.FindProperty("spawnUI");
        UIToSpawn = sourceRef.FindProperty("UIToSpawn");
        parentUIToUnit = sourceRef.FindProperty("parentUIToUnit");
        disableOnDeath = sourceRef.FindProperty("disableOnDeath");
    }

    protected virtual void SetProperties()
    {
        EditorGUILayout.Space();
        DisplayDataProperties();
        DisplayUIProperties();
        //enemy
        EditorGUILayout.PropertyField(disableOnDeath, true);
        EditorGUILayout.PropertyField(attackTarget);

    }

    protected virtual void DisplayDataProperties()
    {
        //skins
        EditorGUILayout.PropertyField(data);
        if (data.objectReferenceValue)
        {
            
            EditorGUILayout.PropertyField(skinOptions);
            if (skinOptions.enumValueIndex == 1)
                spawnSkinOverride.objectReferenceValue = EditorGUILayout.ObjectField("Prefab Override", spawnSkinOverride.objectReferenceValue, typeof(GameObject), false);
            else if (skinOptions.enumValueIndex == 2)
            {
                EditorGUILayout.PropertyField(childSkinOverride);
                var child = childSkinOverride.GetRootValue<GameObject>();
                if (child)
                {
                    if (!source.transform.FindDeepChild(child.name))
                    {
                        Debug.Log("You must assign a child of " + source + " to " + childSkinOverride.displayName);
                        childSkinOverride.objectReferenceValue = null;
                    }
                }
            }
            var sourceData = data.GetRootValue<UnitData>();
            if (sourceData)
            {
                if ((sourceData.skinPrefab && skinOptions.enumValueIndex != 2) || spawnSkinOverride.objectReferenceValue || childSkinOverride.objectReferenceValue)
                {
                    if (spawnSkinOverride.objectReferenceValue)
                        itemSpawnLocation.ChildNamePopUpParentOverride(sourceRef, spawnSkinOverride);
                    else if (childSkinOverride.objectReferenceValue)
                        itemSpawnLocation.ChildNamePopUpParentOverride(sourceRef, childSkinOverride);
                }
                else
                    EditorExtensions.LabelFieldCustom("You must assign a skin to equip items!", Color.red, FontStyle.Bold);
            }
            
        }
        else
            EditorExtensions.LabelFieldCustom("You must assign a unit data file to " + data.displayName, Color.red, FontStyle.Bold);
    }

    protected virtual void DisplayUIProperties()
    {
        EditorGUILayout.PropertyField(spawnUI);
        if (spawnUI.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(UIToSpawn);
            EditorGUILayout.PropertyField(parentUIToUnit);
        }
            
    }

}
