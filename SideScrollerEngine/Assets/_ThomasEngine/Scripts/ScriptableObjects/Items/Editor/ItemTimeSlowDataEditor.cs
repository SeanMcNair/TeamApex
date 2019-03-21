using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemTimeSlowData))]
public class ItemTimeSlowDataEditor : ItemAimableDataEditor
{
    public SerializedProperty slowTimeScale;
    public SerializedProperty physicsTimeScale;
    public SerializedProperty crossfadeTime;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemTimeSlowData)target;
        source.linkedType = typeof(ItemTimeSlow);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        slowTimeScale = sourceRef.FindProperty("slowTimeScale");
        physicsTimeScale = sourceRef.FindProperty("physicsTimeScale");
        crossfadeTime = sourceRef.FindProperty("crossfadeTime");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Time Slow Properties", boldStyle);
        EditorGUILayout.PropertyField(slowTimeScale);
        EditorGUILayout.PropertyField(physicsTimeScale);
        EditorGUILayout.PropertyField(crossfadeTime);

    }

}
