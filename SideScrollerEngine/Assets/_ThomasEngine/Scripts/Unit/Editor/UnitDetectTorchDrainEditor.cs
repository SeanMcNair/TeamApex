using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UnitDetectTorchDrain))]
public class UnitDetectTorchDrainEditor : DetectZoneTriggerEditor
{
    private SerializedProperty lightDrain;
    private SerializedProperty hpDrain;

    public override void GetProperties()
    {
        base.GetProperties();
        lightDrain = sourceRef.FindProperty("lightDrain");
        hpDrain = sourceRef.FindProperty("hpDrain");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(lightDrain);
        EditorGUILayout.PropertyField(hpDrain);
    }
}