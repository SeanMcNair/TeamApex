using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerStompEnemies))]
public class PlayerStompEnemiesEditor : Editor
{
    private PlayerStompEnemies source;
    private SerializedObject sourceRef;

    private SerializedProperty damage;
    private SerializedProperty instantKill;
    private SerializedProperty instantKillTag;

    private SerializedProperty stompMask;
    private SerializedProperty detectBoxSize;
    private SerializedProperty detectBoxCenter;

    private SerializedProperty bounceForce;


    private void OnEnable()
    {
        source = (PlayerStompEnemies)target;
        sourceRef = serializedObject;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();      
    }

    private void OnSceneGUI()
    {
        //draw grounded gizmo
        Handles.color = Color.blue;
        Handles.DrawWireCube((Vector2)source.transform.position + detectBoxCenter.vector2Value,
            detectBoxSize.vector2Value);
    }

    void GetProperties()
    {
        //damage
        damage = sourceRef.FindProperty("damage");
        instantKill = sourceRef.FindProperty("instantKill");
        instantKillTag = sourceRef.FindProperty("instantKillTag");
        //detect box
        stompMask = sourceRef.FindProperty("stompMask");
        detectBoxSize = sourceRef.FindProperty("detectBoxSize");
        detectBoxCenter = sourceRef.FindProperty("detectBoxCenter");
        //bouncing
        bounceForce = sourceRef.FindProperty("bounceForce");
    }

    void SetProperties()
    {
        EditorGUILayout.PropertyField(damage);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(instantKill);
        if (instantKill.boolValue)
            instantKillTag.stringValue = EditorGUILayout.TagField("Instant Kill Tag", instantKillTag.stringValue);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(stompMask);
        EditorGUILayout.PropertyField(detectBoxSize);
        EditorGUILayout.PropertyField(detectBoxCenter);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(bounceForce);

        sourceRef.ApplyModifiedProperties();
    }
	
}
