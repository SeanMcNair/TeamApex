using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UnitDetectInteractFX))]
public class UnitDetectInteractFXEditor : DetectZoneTriggerEditor
{
    protected SerializedProperty interacts;

    public override void GetProperties()
    {
        base.GetProperties();
        interacts = sourceRef.FindProperty("interacts");
    }

    public override void SetProperties ()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(interacts, true);

    }

}