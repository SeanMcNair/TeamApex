using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Item), true)]
public class ItemEditor : Editor
{

    protected SerializedObject sourceRef;
    protected Item source;
    protected SerializedProperty data;

    public virtual void OnEnable()
    {
        sourceRef = serializedObject;
        source = (Item)target;

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
    }

    public virtual void SetProperties()
    {
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField(data, typeof(ItemData));
    }

}
