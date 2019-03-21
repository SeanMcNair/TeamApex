using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ItemShield))]
public class ItemShieldEditor : ItemEditor
{
    private SerializedProperty spawnedShield;

    public override void GetProperties()
    {
        base.GetProperties();
        if (data.objectReferenceValue)
        {
            if (data.objectReferenceValue.GetType() == typeof(ItemShieldData))
            {
                spawnedShield = sourceRef.FindProperty("spawnedShield");
            }
        }
        
    }

    private void OnSceneGUI()
    {
        if (data.objectReferenceValue)
        {
            if (data.objectReferenceValue.GetType() == typeof(ItemShieldData))
            {
                
                if (spawnedShield.objectReferenceValue)
                {
                    var spawnObj = spawnedShield.objectReferenceValue as GameObject;
                    if (spawnObj)
                    {
                        var shieldSource = (ItemShield)source;
                        //need to create a new handles matrix space just for the box so we can rotate the "angle"...this space is the new Zero position.
                        Matrix4x4 boxMatrix = Matrix4x4.TRS((Vector2)spawnObj.transform.position + shieldSource.Data.detectOffset, Quaternion.Euler(0, 0, spawnObj.transform.eulerAngles.z), Handles.matrix.lossyScale);
                        using (new Handles.DrawingScope(boxMatrix))
                        {
                            Handles.DrawWireCube(Vector2.zero, shieldSource.Data.detectSize);
                        }
                    }
                        
                    
                }
                
            }
        }
    }

}
