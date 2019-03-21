using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Data/Units/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    public enum DeathType { None, Destroy, Respawn }
    public enum NoLivesType { None, GameOverLose, GameOverWin }
    public enum DeathMovement { None, StopIfGrounded, FallThrough }

    public Sprite avatarIcon;
    //UI
    public bool spawnUI;
    public UIUnit UIToSpawn;
    public bool parentUIToUnit;
    //Skin
    public bool setSkin;
    public GameObject skinPrefab;
    public Vector2 skinSize;
    public Vector3 skinRotation;
    public ChildName itemSpawnLocation;

    //rigidbody
    public float weight = 1;

    //movement
    public float speed;
    public float jumpPower;

    //hp
    public ValueAmountManagerData vitalityData;
    public IndexStringProperty hpValue;
    public IndexStringProperty manaValue;
    public IndexStringProperty staminaValue;

    //lives
    public bool infiniteLives;
    public int maxLives = 1;

    //On hit
    public InteractFX[] onHitFX;
    public bool invincibleOnHit;
    public float invincibleTime = 1;
    public bool ignorePhysicsOnHit = true;
    public LayerProperty[] ignoreLayers;
    public float ignoreTime = 1;
    public bool stunOnHit;
    public float stunTime = 1;
    public bool changeMeshMaterialOnHit;
    public float changeMeshTime = 1;
    public Material materialToUse;

    //buffs
    public UnitBuff[] buffs;

    //death stuff
    public float deathTime = 1;
    public InteractFX[] deathFX;
    public DeathType deathOptions;
    public float delay;
    public float respawnTime = 1;
    public NoLivesType noLivesOptions;
    public float endTime = 1;
    public DeathMovement deathMovement;
    public bool resetLevelOnDeath;
}
