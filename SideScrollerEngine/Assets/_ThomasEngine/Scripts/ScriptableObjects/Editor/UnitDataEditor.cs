using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitData))]
public class UnitDataEditor : Editor
{

    protected SerializedObject sourceRef;
    protected UnitData source;
    protected GUIStyle boldStyle;

    //icon
    protected SerializedProperty avatarIcon;

    //ui
    protected SerializedProperty spawnUI;
    protected SerializedProperty UIToSpawn;
    protected SerializedProperty parentUIToUnit;

    //skin
    protected SerializedProperty setSkin;
    protected SerializedProperty skinPrefab;
    protected SerializedProperty skinSize;
    protected SerializedProperty skinRotation;
    protected SerializedProperty itemSpawnLocation;

    //rigidbody
    protected SerializedProperty weight;

    //movement
    protected SerializedProperty speed;
    protected SerializedProperty jumpPower;

    //vitals
    protected SerializedProperty vitalityData;
    protected SerializedProperty hpValue;
    protected SerializedProperty manaValue;
    protected SerializedProperty staminaValue;

    //lives
    protected SerializedProperty infiniteLives;
    protected SerializedProperty maxLives;
    //on hit
    protected SerializedProperty onHitFX;
    protected SerializedProperty invincibleOnHit;
    protected SerializedProperty invincibleTime;
    protected SerializedProperty ignorePhysicsOnHit;
    protected SerializedProperty ignoreLayers;
    protected SerializedProperty ignoreTime;
    protected SerializedProperty stunOnHit;
    protected SerializedProperty stunTime;
    protected SerializedProperty changeMeshMaterialOnHit;
    protected SerializedProperty changeMeshTime;
    protected SerializedProperty materialToUse;

    //buffs
    protected SerializedProperty buffs;

    //death stuff
    protected SerializedProperty deathTime;
    protected SerializedProperty deathFX;
    protected SerializedProperty deathOptions;
    protected SerializedProperty delay;
    protected SerializedProperty respawnTime;
    protected SerializedProperty noLivesOptions;
    protected SerializedProperty endTime;
    protected SerializedProperty deathMovement;
    protected SerializedProperty resetLevelOnDeath;


    public virtual void OnEnable()
    {
        source = (UnitData)target;
        sourceRef = serializedObject;
        SetupGUIStyle();
        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        sourceRef.ApplyModifiedProperties();
    }

    void SetupGUIStyle()
    {
        boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
        };
    }

    public virtual void GetProperties()
    {
        avatarIcon = sourceRef.FindProperty("avatarIcon");
        //ui
        spawnUI = sourceRef.FindProperty("spawnUI");
        UIToSpawn = sourceRef.FindProperty("UIToSpawn");
        parentUIToUnit = sourceRef.FindProperty("parentUIToUnit");
        //skin
        setSkin = sourceRef.FindProperty("setSkin");
        skinPrefab = sourceRef.FindProperty("skinPrefab");
        itemSpawnLocation = sourceRef.FindProperty("itemSpawnLocation");
        skinSize = sourceRef.FindProperty("skinSize");
        skinRotation = sourceRef.FindProperty("skinRotation");
        //rigidbody
        weight = sourceRef.FindProperty("weight");
        //movement
        speed = sourceRef.FindProperty("speed");
        jumpPower = sourceRef.FindProperty("jumpPower");
        //vitals
        vitalityData = sourceRef.FindProperty("vitalityData");
        hpValue = sourceRef.FindProperty("hpValue");
        manaValue = sourceRef.FindProperty("manaValue");
        staminaValue = sourceRef.FindProperty("staminaValue");
        //lives
        infiniteLives = sourceRef.FindProperty("infiniteLives");
        maxLives = sourceRef.FindProperty("maxLives");
        //on hit
        onHitFX = sourceRef.FindProperty("onHitFX");
        invincibleOnHit = sourceRef.FindProperty("invincibleOnHit");
        invincibleTime = sourceRef.FindProperty("invincibleTime");
        ignorePhysicsOnHit = sourceRef.FindProperty("ignorePhysicsOnHit");
        ignoreLayers = sourceRef.FindProperty("ignoreLayers");
        ignoreTime = sourceRef.FindProperty("ignoreTime");
        stunOnHit = sourceRef.FindProperty("stunOnHit");
        stunTime = sourceRef.FindProperty("stunTime");
        changeMeshMaterialOnHit = sourceRef.FindProperty("changeMeshMaterialOnHit");
        materialToUse = sourceRef.FindProperty("materialToUse");
        changeMeshTime = sourceRef.FindProperty("changeMeshTime");
        //buffs
        buffs = sourceRef.FindProperty("buffs");
        //death
        deathTime = sourceRef.FindProperty("deathTime");
        deathFX = sourceRef.FindProperty("deathFX");
        deathOptions = sourceRef.FindProperty("deathOptions");
        noLivesOptions = sourceRef.FindProperty("noLivesOptions");
        delay = sourceRef.FindProperty("delay");
        respawnTime = sourceRef.FindProperty("respawnTime");
        endTime = sourceRef.FindProperty("endTime");
        deathMovement = sourceRef.FindProperty("deathMovement");
        resetLevelOnDeath = sourceRef.FindProperty("resetLevelOnDeath");
    }

    public virtual void SetProperties()
    {
        EditorExtensions.SpritePreviewField(avatarIcon, 80, 80, true);
        DisplayUIProperties();
        DisplaySkinProperties();
        DisplayRigidbodyProperties();
        DisplayMovementProperties();
        DisplayVitalityProperties();
        DisplayOnHitProperties();
        DisplayBuffProperties();
        DisplayDeathProperties();
    }

    protected virtual void DisplayUIProperties()
    {
        EditorGUILayout.LabelField("UI Properties", boldStyle);
        EditorGUILayout.PropertyField(spawnUI);
        if (spawnUI.boolValue)
        {
            UIToSpawn.PrefabFieldWithComponent(typeof(UIPlayer));
            EditorGUILayout.PropertyField(parentUIToUnit);
        }

    }

    protected virtual void DisplaySkinProperties()
    {
        EditorGUILayout.LabelField("Skin Properties", boldStyle);
        EditorGUILayout.PropertyField(setSkin);
        if (setSkin.boolValue)
        {
            EditorGUILayout.PropertyField(skinPrefab);
            itemSpawnLocation.ChildNamePopUpParentOverride(sourceRef, skinPrefab);
            EditorGUILayout.PropertyField(skinSize);
            EditorGUILayout.PropertyField(skinRotation);
        }  
    }

    protected virtual void DisplayRigidbodyProperties()
    {
        EditorGUILayout.LabelField("Rigidbody Properties", boldStyle);
        EditorGUILayout.PropertyField(weight);
    }

    protected virtual void DisplayMovementProperties()
    {
        EditorGUILayout.LabelField("Movement Properties", boldStyle);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(jumpPower);
    }

    protected virtual void DisplayVitalityProperties()
    {
        EditorGUILayout.LabelField("Vitality Properties", boldStyle);
        EditorGUILayout.PropertyField(vitalityData);
        var vitalData = vitalityData.GetRootValue<ValueAmountManagerData>();
        if (vitalData != null)
        {
            var names = vitalData.GetValueNames();
            hpValue.IndexStringPropertyField(names);
            manaValue.IndexStringPropertyField(names);
            staminaValue.IndexStringPropertyField(names);
        }
        
        EditorGUILayout.PropertyField(infiniteLives);
        if (!infiniteLives.boolValue)
            EditorGUILayout.PropertyField(maxLives);
            
    }

    protected virtual void DisplayOnHitProperties()
    {
        EditorGUILayout.LabelField("On Hit Properties", boldStyle);
        EditorGUILayout.PropertyField(onHitFX, true);
        EditorGUILayout.PropertyField(invincibleOnHit);
        if (invincibleOnHit.boolValue)
            EditorGUILayout.PropertyField(invincibleTime);
        EditorGUILayout.PropertyField(ignorePhysicsOnHit);
        if (ignorePhysicsOnHit.boolValue)
        {
            EditorGUILayout.PropertyField(ignoreTime);
            EditorGUILayout.PropertyField(ignoreLayers, true);
        }
        EditorGUILayout.PropertyField(stunOnHit);
        if (stunOnHit.boolValue)
        {
            EditorGUILayout.PropertyField(stunTime);
        }
        EditorGUILayout.PropertyField(changeMeshMaterialOnHit);
        if (changeMeshMaterialOnHit.boolValue)
        {
            EditorGUILayout.PropertyField(changeMeshTime);
            EditorGUILayout.PropertyField(materialToUse);
        }
    }

    protected virtual void DisplayBuffProperties()
    {
        EditorGUILayout.LabelField("Buff Properties", boldStyle);
        EditorGUILayout.PropertyField(buffs, true);
    }

    protected virtual void DisplayDeathProperties()
    {
        EditorGUILayout.LabelField("Death Properties", boldStyle);
        EditorGUILayout.PropertyField(deathTime); 
        EditorGUILayout.PropertyField(deathFX, true);
        EditorGUILayout.PropertyField(deathOptions);
        if (deathOptions.enumValueIndex == 1)
            EditorGUILayout.PropertyField(delay);
        if (deathOptions.enumValueIndex == 2)
            EditorGUILayout.PropertyField(respawnTime);
        EditorGUILayout.PropertyField(noLivesOptions);
        if (noLivesOptions.enumValueIndex != 0)
            EditorGUILayout.PropertyField(endTime);
            EditorGUILayout.PropertyField(deathMovement);
        EditorGUILayout.PropertyField(resetLevelOnDeath);
    }
}
