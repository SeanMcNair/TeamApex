using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : LinkableData
{
    public Sprite icon;
    public bool spawnUI;
    public GameObject itemUI;
    public bool keepUIActiveIfDropped;
    public bool setOwnerAsParent;
    public int value;
    public float weight;
    public bool droppable;
    public GameObject droppedPrefab;
    public bool quickMenuCompatible;
    public GameObject particleFX;
    public string animState;
    public ItemBuff[] buffs;
}
