using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemButtonPressData))]
public class ItemButtonPressDataEditor : ItemUseableDataEditor
{
    public SerializedProperty setButton;
    public SerializedProperty button;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemButtonPressData)target;
        source.linkedType = typeof(ItemButtonPress);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        setButton = sourceRef.FindProperty("setButton");
        button = sourceRef.FindProperty("button");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Button Press Properties",boldStyle);
        EditorGUILayout.PropertyField(setButton);
        if (setButton.boolValue)
            EditorGUILayout.PropertyField(button);

    }

}
