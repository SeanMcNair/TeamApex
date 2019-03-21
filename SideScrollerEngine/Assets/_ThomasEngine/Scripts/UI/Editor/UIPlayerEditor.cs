using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UIPlayer))]
public class UIPlayerEditor : UIUnitEditor
{
    protected SerializedProperty lives;
    protected SerializedProperty points;
    protected SerializedProperty pauseMenu;
    protected SerializedProperty quickMenu;
    protected SerializedProperty startCursorSettings;
    protected SerializedProperty pauseCursorSettings;

    public override void GetProperties ()
    {
        base.GetProperties();
        lives = sourceRef.FindProperty ("lives");
        points = sourceRef.FindProperty ("points");
        pauseMenu = sourceRef.FindProperty ("pauseMenu");
        quickMenu = sourceRef.FindProperty ("quickMenu");
        startCursorSettings = sourceRef.FindProperty ("startCursorSettings");
        pauseCursorSettings = sourceRef.FindProperty ("pauseCursorSettings");
    }

    public override void SetProperties ()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField (lives);
        EditorGUILayout.PropertyField (points);
        EditorGUILayout.PropertyField (pauseMenu);
        EditorGUILayout.PropertyField (quickMenu);
        EditorGUILayout.PropertyField (startCursorSettings, true);
        EditorGUILayout.PropertyField (pauseCursorSettings, true);

    }

}