using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemInteractFXData))]
public class ItemInteractFXDataEditor : ItemAimableDataEditor
{
    public SerializedProperty interacts;
    public SerializedProperty runInteractsOnOwner;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemInteractFXData)target;
        source.linkedType = typeof(ItemInteractFX);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        interacts = sourceRef.FindProperty("interacts");
        runInteractsOnOwner = sourceRef.FindProperty("runInteractsOnOwner");


    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorExtensions.LabelFieldCustom("InteractFX Properties", Color.black, FontStyle.Bold);
        EditorGUILayout.PropertyField(interacts, true);
        EditorGUILayout.PropertyField(runInteractsOnOwner);
    }

}
