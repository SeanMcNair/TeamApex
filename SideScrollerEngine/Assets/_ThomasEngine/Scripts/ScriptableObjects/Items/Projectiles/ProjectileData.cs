using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileData : LinkableData
{
    public float lifeTime = 10;
    public int hitMaxAmount = 1;
    public GameObject[] spawnOnImpact;
    public InteractFX[] interactFX;
}
