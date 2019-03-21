using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(UnitEquip))]
public class UnitEquipEditor : Editor
{

    protected SerializedObject sourceRef;
    protected SerializedProperty autoEquipItems;
    protected SerializedProperty maxItems;
    protected SerializedProperty disablePickupIfFull;
    protected SerializedProperty setAllItemsActive;
    protected SerializedProperty itemManager;
    protected SerializedProperty itemsToAdd;
    protected SerializedProperty itemDatas;
    protected SerializedProperty dropThrowPower;

    public virtual void OnEnable()
    {
        sourceRef = serializedObject;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {

        SetProperties();

        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        autoEquipItems = sourceRef.FindProperty("autoEquipItems");
        maxItems = sourceRef.FindProperty("maxItems");
        disablePickupIfFull = sourceRef.FindProperty("disablePickupIfFull");
        setAllItemsActive = sourceRef.FindProperty("setAllItemsActive");
        itemManager = sourceRef.FindProperty("itemManager");
        itemsToAdd = sourceRef.FindProperty("itemsToAdd");
        itemDatas = sourceRef.FindProperty("itemDatas");
        dropThrowPower = sourceRef.FindProperty("dropThrowPower");

    }

    public virtual void SetProperties()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(autoEquipItems);
        EditorGUILayout.PropertyField(setAllItemsActive);
        DisplayItemDatasProperties();
        EditorGUILayout.PropertyField(disablePickupIfFull);
        EditorGUILayout.PropertyField(dropThrowPower);


    }

    protected virtual void DisplayItemDatasProperties()
    {
        EditorGUILayout.PropertyField(itemManager);
        EditorGUILayout.DelayedIntField(maxItems);
        var manager = itemManager.GetRootValue<ItemDataManager>();
        if (manager)
        {
            for (int i = 0; i < itemsToAdd.arraySize; i++)
            {
                var element = itemsToAdd.GetArrayElementAtIndex(i);
                var item = element.GetRootValue<ItemProperty>();
                item.itemNames = manager.GetItemNames();
            } 
            itemsToAdd.ArrayFieldCustom(false, true, "Item");
            itemsToAdd.arraySize = maxItems.intValue;
        }
        else
            EditorExtensions.LabelFieldCustom("You need to a inventory manager to add items!", Color.red, FontStyle.Bold);
    }

}
