using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Projectile), true)]
public class ProjectileEditor : Editor
{
    protected SerializedObject sourceRef;
    protected Projectile source;
    protected SerializedProperty data;
    protected SerializedProperty detectZone;
    protected DetectZone zone;

    public virtual void OnEnable()
    {
        sourceRef = serializedObject;
        source = (Projectile)target;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {

        SetProperties();

        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        data = sourceRef.FindProperty("data");
        detectZone = sourceRef.FindProperty("detectZone");
    }

    public virtual void SetProperties()
    {
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField(data, typeof(ProjectileData));
        SetZoneProperties();
        EditorGUILayout.PropertyField(detectZone);
        
    }

    void SetZoneProperties()
    {
        zone = detectZone.GetRootValue<DetectZone>();
        zone.overrideDetectMask = true;
        zone.overridePositionType = true;
        zone.positionType = DetectZone.PositionType.Offset;
    }

    void OnSceneGUI()
    {
        if (zone == null)
            return;

        zone.DrawDetectZone(source, sourceRef, source.transform);
    }

}
