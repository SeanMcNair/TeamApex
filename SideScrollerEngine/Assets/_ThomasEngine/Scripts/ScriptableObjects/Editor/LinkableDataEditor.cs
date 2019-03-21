using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LinkableData))]
public class LinkableDataEditor : Editor
{
    protected SerializedObject sourceRef;
    protected LinkableData source;

    protected SerializedProperty connectedPrefab;

    public virtual void OnEnable()
    {
        source = (LinkableData)target;
        source.linkedType = typeof(Linkable);
        sourceRef = serializedObject;
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        sourceRef.ApplyModifiedProperties();
    }

    public virtual void GetProperties()
    {
        connectedPrefab = sourceRef.FindProperty("connectedPrefab");
    }

    public virtual void SetProperties()
    {
        EditorGUILayout.PropertyField(connectedPrefab);
        CheckModelPrefabLink();
    }

    public virtual void CheckModelPrefabLink()
    {
        if (!connectedPrefab.objectReferenceValue)
            return;
        var go = connectedPrefab.objectReferenceValue as GameObject;
        if (go)
        {
            var comp = go.GetComponent(source.linkedType);
            if (!comp)
            {
                if (GUILayout.Button("Add " + source.linkedType.Name + " script to prefab"))
                {
                    go.AddComponent(source.linkedType);
                    var linkableComp = go.GetComponent<Linkable>();
                    linkableComp.Data = source;
                    Undo.RecordObject(go, "Linked data file");
                }
            }
            if (comp)
            { 
                var linkableComp = go.GetComponent<Linkable>();
                if (linkableComp.Data != source)
                {
                    if (GUILayout.Button("Add " + source.name + " data to " + linkableComp.name))
                    {
                        linkableComp.Data = source;
                        Undo.RecordObject(go, "Linked data file");
                    }
                }
            }
                
        }
    }

    public virtual void OnSceneGUI(SceneView sceneView)
    {
        
    }

}
