using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ProjectileTargetSeekingData))]
public class ProjectileTargetSeekingDataEditor : ProjectileDataEditor
{
    public SerializedProperty keepYFacingUp;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ProjectileTargetSeekingData)target;
        source.linkedType = typeof(ProjectileTargetSeeking);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        keepYFacingUp = sourceRef.FindProperty("keepYFacingUp");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(keepYFacingUp);

    }

}
