using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;
using System.Linq;

[CustomEditor(typeof(SpawnPoolManager))]
public class SpawnPoolManagerEditor : Editor
{
    private SerializedObject sourceRef;
    private SpawnPoolManager source;

    private SerializedProperty pools;
    private ReorderableList poolList;

    private void OnEnable()
    {
        source = (SpawnPoolManager)target;
        sourceRef = serializedObject;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        sourceRef.ApplyModifiedProperties();
    }

    void GetProperties()
    { 
        pools = sourceRef.FindProperty("pools");
        CreatePoolList();
    }

    void CreatePoolList()
    {
        poolList = pools.ReorderableListCustom(sourceRef, typeof(Pool), "Pools", 1);
    }

    void SetProperties()
    {
        poolList.DoLayoutList();   
    }

}
