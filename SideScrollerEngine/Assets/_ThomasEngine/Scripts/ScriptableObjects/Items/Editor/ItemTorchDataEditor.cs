using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemTorchData))]
public class ItemTorchDataEditor : ItemAimableDataEditor
{
    public SerializedProperty lightMaxRange;
    public SerializedProperty lightDrainSpeed;

    public override void OnEnable()
    {
        base.OnEnable();
        overrideUsage.boolValue = true;
        source = (ItemTorchData)target;
        source.linkedType = typeof(ItemTorch);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        lightMaxRange = sourceRef.FindProperty("lightMaxRange");
        lightDrainSpeed = sourceRef.FindProperty("lightDrainSpeed");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorExtensions.LabelFieldCustom("Torch Properties", Color.black, FontStyle.Bold);
        EditorGUILayout.PropertyField(lightMaxRange);
        EditorGUILayout.PropertyField(lightDrainSpeed);
    }

}
