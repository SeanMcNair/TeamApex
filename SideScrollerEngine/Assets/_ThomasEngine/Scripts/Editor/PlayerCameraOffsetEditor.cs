using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PlayerCameraOffset))]
public class PlayerCameraOffsetEditor : Editor
{

    private SerializedObject sourceRef;
    private PlayerCameraOffset source;

    private SerializedProperty triggerTag;
    private SerializedProperty playerCamera;
    private SerializedProperty offsetType;
    private SerializedProperty disableDelay;
    private SerializedProperty newCamPos;
    private SerializedProperty offset;
    private SerializedProperty newRot;
    private SerializedProperty overrideRotLock;
    private SerializedProperty offsetTime;
    private SerializedProperty disableTime;
    private SerializedProperty resetOffsetOnExit;
    private SerializedProperty resetTime;
    private SerializedProperty smooth;
    private SerializedProperty playerPreview;

    private bool preview;
    private Vector2 previewPos;
    private Camera previewCamera;
    private GameObject previewPlayer;
    private GameObject lastPreviewPlayer;

    private void OnDisable()
    {
        DestroyCamera();
        DestroyPlayer();
    }

    private void OnEnable()
    {
        sourceRef = serializedObject;
        source = target as PlayerCameraOffset;

        GetProperties();
    }

    public override void OnInspectorGUI()
    {

        SetProperties();

        sourceRef.ApplyModifiedProperties();
    }

    void GetProperties()
    {
        triggerTag = sourceRef.FindProperty("triggerTag");
        playerCamera = sourceRef.FindProperty("playerCamera");
        offsetType = sourceRef.FindProperty("offsetType");
        disableDelay = sourceRef.FindProperty("lockDelay");
        newCamPos = sourceRef.FindProperty("newCamPos");
        offset = sourceRef.FindProperty("newOffset");
        newRot = sourceRef.FindProperty("newRot");
        overrideRotLock = sourceRef.FindProperty("overrideRotLock");
        offsetTime = sourceRef.FindProperty("offsetTime");
        disableTime = sourceRef.FindProperty("lockTime");
        resetOffsetOnExit = sourceRef.FindProperty("resetOffsetOnExit");
        resetTime = sourceRef.FindProperty("resetTime");
        smooth = sourceRef.FindProperty("smooth");
        playerPreview = sourceRef.FindProperty("playerPreview");
    }

    void SetupCamera()
    {
        if (!previewCamera)
        {
            previewCamera = new GameObject().AddComponent<Camera>();
            var main = Camera.main;
            previewCamera.orthographicSize = main.orthographicSize;
            previewCamera.projectionMatrix = main.projectionMatrix;
            previewCamera.fieldOfView = main.fieldOfView;
            previewCamera.gameObject.name = "[Preview Camera]";
        }
           
    }

    void SetCameraProperties()
    {
        var sourcePos = (Vector2)source.transform.position;
        if (offsetType.enumValueIndex == 0)
            previewCamera.transform.position = (Vector3)sourcePos + (Vector3)previewPos + offset.vector3Value;
        else if (offsetType.enumValueIndex == 1)
        {
            if (newCamPos.objectReferenceValue)
            {
                var pos = newCamPos.objectReferenceValue as Transform;
                previewCamera.transform.position = pos.transform.position;
            }
        }
            
        if (playerPreview.objectReferenceValue)
        {
            var player = playerPreview.objectReferenceValue as GameObject;
            if (player != lastPreviewPlayer || !previewPlayer)
            {
                DestroyPlayer();
                previewPlayer = Instantiate(player);
                previewPlayer.name = "[Preview Player]";
                lastPreviewPlayer = player;
            }
            if (previewPlayer)
                previewPlayer.transform.position = sourcePos + previewPos;
        }
        previewCamera.transform.rotation = Quaternion.Euler(newRot.vector3Value);
    }

    void DestroyCamera()
    {
        if (previewCamera)
            DestroyImmediate(previewCamera.gameObject);
    }

    void DestroyPlayer()
    {
        if (previewPlayer)
            DestroyImmediate(previewPlayer);
    }

    void SetProperties()
    {
        EditorGUILayout.Space();
        triggerTag.stringValue = EditorGUILayout.TagField("Trigger Tag", triggerTag.stringValue);
        EditorGUILayout.PropertyField(playerCamera);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(offsetType);
        if (offsetType.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(newCamPos);
            EditorGUILayout.PropertyField(newRot);
            EditorGUILayout.PropertyField(overrideRotLock);
            EditorGUILayout.PropertyField(disableDelay);
            EditorGUILayout.PropertyField(disableTime);
            EditorGUILayout.PropertyField(resetTime);
            resetOffsetOnExit.boolValue = false;
        }
        else
        {
            EditorGUILayout.PropertyField(offset);
            EditorGUILayout.PropertyField(newRot);
            EditorGUILayout.PropertyField(overrideRotLock);
            EditorGUILayout.PropertyField(resetOffsetOnExit);
            if (resetOffsetOnExit.boolValue)
                EditorGUILayout.PropertyField(resetTime);
        }
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(offsetTime);
        EditorGUILayout.PropertyField(smooth);
        preview = EditorGUILayout.Toggle("Preview Camera Position", preview);
        if (preview)
        {
            EditorGUILayout.PropertyField(playerPreview);
            previewPos = EditorGUILayout.Vector2Field("Player Position", previewPos);
            SetupCamera();
            SetCameraProperties();
        }
        else if (previewCamera || Application.isPlaying)
        {
            DestroyCamera();
            DestroyPlayer();
        }
    }

}
