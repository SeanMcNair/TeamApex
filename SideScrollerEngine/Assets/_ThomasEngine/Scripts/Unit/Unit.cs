using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum SpawnUIOptions { FromData, Override, None }
    public enum SpawnSkinOptions { FromData, Override, Child, None }

    [SerializeField] protected UnitData data;
    [SerializeField] protected ChildName itemSpawnLocation;
    [SerializeField] protected SpawnSkinOptions skinOptions = SpawnSkinOptions.None;
    [SerializeField] protected GameObject spawnSkinOverride;
    [SerializeField] protected GameObject childSkinOverride;
    protected Transform itemSpawnTrans;
    public Transform ItemSpawnLocation { get { return itemSpawnTrans; } }
    [SerializeField] protected SpawnUIOptions spawnUI;
    [SerializeField] protected GameObject UIToSpawn;
    [SerializeField] protected bool parentUIToUnit;
    [SerializeField] private Behaviour[] disableOnDeath;

    protected UnitData curData;
    public UnitData CurUnitData { get { return curData; } }
    public string CurUnitDataName { get { return CurUnitData.name; } }
    [SerializeField] protected Transform attackTarget;
    public Transform AttackTarget { get { return attackTarget; } }

    protected int curMaxHP = 5;
    public int MaxHp { get { return curMaxHP; } set { curMaxHP = value; } }
    protected int curLives;
    public int CurLives { get { return curLives; } }

    protected ValueAmountLocal[] valueAmountLocals;
    public ValueAmountLocal[] ValueAmountLocals { get { return valueAmountLocals; } }

    public float CurHP { get { return valueAmountLocals[curData.hpValue.indexValue].CurAmount; } }
    public float CurMana { get { return valueAmountLocals[curData.manaValue.indexValue].CurAmount; } }
    public float CurStamina { get { return valueAmountLocals[curData.staminaValue.indexValue].CurAmount; } }

    protected bool stunned;
    public bool IsStunned { get { return stunned; } }
    private Coroutine stunnedCoroutine;

    protected bool physicsIgnored;
    public bool IsPhysicsIgnored { get { return physicsIgnored; } }
    private Coroutine physicsIgnoredCoroutine;

    protected bool meshesChanged;
    public bool IsMeshesChanged { get { return meshesChanged; } }
    private Renderer[] meshesToChange;
    private Material[] startMeshesMaterials;
    private Coroutine meshChangeCoroutine;

    protected bool invincible;
    public bool IsInvincible { get { return invincible; } set { invincible = value; } }
    private Coroutine invincibleCoroutine;

    protected float curWeight;
    public float CurWeight { get { return curWeight; } }
    protected bool dead;
    public bool IsDead { get { return dead; } }
    protected UnitController controller;
    protected UIUnit ui;
    public UIUnit UI { get { return ui; } }

    protected List<UnitBuff> curBuffs = new List<UnitBuff>();

    protected UnitEquip equip;
    protected UnitAnimations anim;
    protected Rigidbody2D rb;
    protected Collider2D col;
    public Collider2D Collider { get { return col; } }

    protected Vector2 spawnPos;
    protected GameObject curSkin;

    protected virtual void Awake()
    {
        GetComponents();
        GetSpawnPos();
        SetDefaultData();
        SetDefaultVitals();
        SpawnSkin();
        SpawnUI();
        SetupUnit(true, true);
    }

    protected virtual void Update()
    {
        CheckKillHeight();
    }

    protected virtual void GetComponents()
    {
        controller = GetComponent<UnitController>();
        equip = GetComponent<UnitEquip>();
        anim = GetComponent<UnitAnimations>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void GetSpawnPos()
    {
        spawnPos = transform.position;
        if (skinOptions == SpawnSkinOptions.Child || skinOptions == SpawnSkinOptions.Override)
                itemSpawnTrans = itemSpawnLocation.transformValue;
    }

    protected virtual void SetDefaultData()
    { 
        //set data
        SetData(data);
    }

    protected virtual void SetDefaultVitals()
    {
        //load default vitals
        LoadVitalities(curData.vitalityData.valueAmountLocals);
    }

    protected virtual void LoadVitalities(ValueAmountLocal[] _valueAmounts)
    {
        valueAmountLocals = new ValueAmountLocal[_valueAmounts.Length];
        for (int i = 0; i < valueAmountLocals.Length; i++)
        {
            valueAmountLocals[i] = new ValueAmountLocal();
            valueAmountLocals[i].CopyValues(_valueAmounts[i]);
            valueAmountLocals[i].Initialize(this);
            valueAmountLocals[i].RefreshUI();
        }
    }

    public ValueAmountLocal GetLocalValue(int _ind)
    {
        if (valueAmountLocals.Length > 0 && _ind < valueAmountLocals.Length - 1)
            return valueAmountLocals[_ind];

        Debug.Log("could not find local value in " + data.vitalityData.name);
        return null;
    }

    public virtual void ChangeSkin(Unit _unit)
    {
        LoadVitalities(_unit.ValueAmountLocals);
        ChangeSkin(_unit.CurUnitData);
    }

    public virtual void ChangeSkin(UnitData _unitData, bool _defaultVitals = false)
    {
        SetData(_unitData);
        if (_defaultVitals)
            SetDefaultVitals();
        SpawnSkin();
        SpawnUI();
        if (equip)
        {
            equip.GetComponents();
            equip.GetSpawnLocation();
            equip.SpawnItems();
        }
        SetupUnit(false, false);
    }

    public virtual void SetData(UnitData _data)
    {
        curData = _data;
        //update unit stats

        //weight
        curWeight = curData.weight;
        if (rb)
            rb.mass = curWeight;

        //update controller stats
        if (controller)
        {
            controller.BaseSpeed = curData.speed;
            controller.JumpPower = curData.jumpPower;
            controller.StartColSize = curData.skinSize;
            controller.StartColOffset = curData.skinSize / 2;
        }
        
    }

    protected virtual void SpawnSkin()
    {
        if (skinOptions != SpawnSkinOptions.None)
        {
            //drop current droppable item
            if (equip)
                equip.DropCurrentItem();
            //destroy current skin
            if (curSkin)
                Destroy(curSkin);

            if (skinOptions == SpawnSkinOptions.FromData)
            {
                if (curData.setSkin)
                {
                    //spawn new skin
                    curSkin = Instantiate(curData.skinPrefab, transform.position, transform.rotation);
                    curSkin.transform.SetParent(transform);
                    //set Rotation
                    curSkin.transform.localEulerAngles = curData.skinRotation;
                    itemSpawnLocation = curData.itemSpawnLocation;

                    //set collider size
                    var capsule = col as CapsuleCollider2D;
                    if (capsule)
                    {
                        capsule.size = curData.skinSize;
                        capsule.offset = new Vector2(0, capsule.size.y / 2);
                    }
                }
            }
            else if (skinOptions == SpawnSkinOptions.Override)
            {
                curSkin = Instantiate(spawnSkinOverride, transform.position, transform.rotation);
                curSkin.transform.SetParent(transform);
            }    
            else if (skinOptions == SpawnSkinOptions.Child)
                curSkin = childSkinOverride;

        }
            
        if (curSkin)
        {
            //set animator
            var an = curSkin.GetComponent<Animator>();
            if (anim)
                anim.Animator = an;

            itemSpawnTrans = curSkin.transform.FindDeepChild(itemSpawnLocation.stringValue);

            GetMaterials();
        }
        else
            Debug.Log("No skin is set up on " + gameObject.name + "! This unit will not be able to use items.");
    }

    protected virtual void SpawnUI()
    {
        //spawn ui
        if (ui)
            Destroy(ui.gameObject);
        if (spawnUI != SpawnUIOptions.None)
        {
            if (spawnUI == SpawnUIOptions.FromData)
            {
                if (curData.spawnUI)
                {
                    if (curData.UIToSpawn)
                    {
                        //spawn ui
                        ui = Instantiate(curData.UIToSpawn);
                        if (curData.parentUIToUnit)
                        {
                            ui.transform.position = transform.position;
                            ui.transform.rotation = transform.rotation;
                            ui.transform.SetParent(transform);
                        }

                    }
                }

            }
            else if (spawnUI == SpawnUIOptions.Override)
            {
                if (UIToSpawn)
                {
                    //spawn ui
                    ui = Instantiate(UIToSpawn).GetComponent<UIUnit>();
                    if (parentUIToUnit)
                    {
                        ui.transform.position = transform.position;
                        ui.transform.rotation = transform.rotation;
                        ui.transform.SetParent(transform);
                    }
                        
                }
            }
            if (ui)
            {
                //set ui values to the vitals
                for (int i = 0; i < valueAmountLocals.Length; i++)
                {
                    valueAmountLocals[i].CurUI = ui.GetValueAmountUI(i);
                }
                if (curData.avatarIcon)
                    ui.SetAvatarIcon(curData.avatarIcon);
            }

        }
    }

    protected virtual void SetupUnit(bool _resetVitals, bool _resetLives)
    {
        dead = false;
        //vitals
        if (_resetVitals)
        {
            ResetVitals();
        }

        //lives
        if (_resetLives)
            curLives = curData.maxLives;

        //buffs
        ActivateAllBuffs(false);
        curBuffs.Clear();
        if (curData.buffs.Length > 0)
            curBuffs.AddRange(curData.buffs);
        ActivateAllBuffs(true);

        //behaviours
        ActivateDeathBehaviours(true);

        if (anim)
            anim.PlayIdle();

        if (ui)
            RefreshUIValues();
    }

    void ResetVitals()
    {
        foreach (var vital in valueAmountLocals)
        {
            vital.Reset();
        }
    }

    public virtual void AddBuff(UnitBuff _buffToAdd)
    {
        _buffToAdd.ActivateBuff(this, true);
        curBuffs.Add(_buffToAdd);
    }

    public virtual void RemoveBuff(UnitBuff _buffToRemove)
    {
        if (!curBuffs.Contains(_buffToRemove))
            return;

        _buffToRemove.ActivateBuff(this, false);
        curBuffs.Remove(_buffToRemove);
    }

    public virtual void ActivateAllBuffs(bool _activate)
    {
        foreach (var buff in curBuffs)
        {
            buff.ActivateBuff(this, _activate);
        }
    }

    public virtual void AddHp(float _amount)
    {
        valueAmountLocals[curData.hpValue.indexValue].AddAmount(_amount);
    }

    protected virtual void CheckKillHeight()
    {
        if (dead)
            return;

        if (transform.position.y < GameManager.instance.GetKillHeight())
        {
            DamageHp(valueAmountLocals[curData.hpValue.indexValue].CurAmount);
        }
    }

    public virtual void DamageHp(float _damage)
    {
        if (dead || invincible)
            return;

        valueAmountLocals[curData.hpValue.indexValue].RemoveAmount(_damage);
        if (ui)
            RefreshUIValues();

        //anim
        if (anim)
            anim.PlayHurt();

        DoOnHitFX();
        Invincible();
        Stun();
        IgnorePhysics();
        ChangeMeshes();

        //kill player if health at 0
        if (valueAmountLocals[curData.hpValue.indexValue].CurAmount <= 0)
        {
            valueAmountLocals[curData.hpValue.indexValue].CurAmount = 0;
            Die();
        }

    }

    public virtual void DamageHp(float _damage, float _bounceForce, Vector2 _bounceDir)
    {
        if (dead)
            return;

        DamageHp(_damage);
        if (controller)
            controller.Bounce(_bounceDir, _bounceForce);

    }

    void DoOnHitFX()
    {
        if (curData.onHitFX == null)
            return;
        if (curData.onHitFX.Length < 1)
            return;
        foreach (var fx in data.onHitFX)
        {
            fx.DoFX(gameObject, gameObject);
        }
    }

    void Invincible()
    {
        if (!curData.invincibleOnHit)
            return;

        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(StartInvincible());
    }

    IEnumerator StartInvincible()
    {
        invincible = true;
        yield return new WaitForSeconds(curData.invincibleTime);
        invincible = false;
    }

    void Stun()
    {
        if (!curData.stunOnHit)
            return;

        if (stunnedCoroutine != null)
            StopCoroutine(stunnedCoroutine);
        stunnedCoroutine = StartCoroutine(StartStun());
    }

    IEnumerator StartStun()
    {
        stunned = true;
        //anim sync
        if (anim)
            anim.PlayStunned();
        controller.DisableMovement(true);
        yield return new WaitForSeconds(curData.stunTime);
        controller.DisableMovement(false);
        stunned = false;
        //anim sync
        if (anim)
            anim.PlayIdle();
    }

    void IgnorePhysics()
    {
        if (!curData.ignorePhysicsOnHit)
            return;

        if (physicsIgnoredCoroutine != null)
            StopCoroutine(physicsIgnoredCoroutine);
        physicsIgnoredCoroutine = StartCoroutine(StartIgnorePhysicsLayers());
    }

    IEnumerator StartIgnorePhysicsLayers()
    {
        physicsIgnored = true;
        IgnoreLayers(true);
        yield return new WaitForSeconds(curData.ignoreTime);
        IgnoreLayers(false);
        physicsIgnored = false;
    }

    void IgnoreLayers(bool _ignore)
    {
        foreach (var lay in curData.ignoreLayers)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, lay.indexValue, _ignore);
        }
    }

    void ChangeMeshes()
    {
        if (!curData.changeMeshMaterialOnHit)
            return;

        if (meshChangeCoroutine != null)
            StopCoroutine(meshChangeCoroutine);
        meshChangeCoroutine = StartCoroutine(StartMeshChange());
    }

    void GetMaterials()
    {
        if (!curData.changeMeshMaterialOnHit)
            return;

        meshesToChange = curSkin.GetComponentsInChildren<Renderer>();
        if (meshesToChange.Length > 0)
        {
            startMeshesMaterials = new Material[meshesToChange.Length];
            for (int i = 0; i < meshesToChange.Length; i++)
            {
                startMeshesMaterials[i] = meshesToChange[i].material;
            }
        }
        else
            Debug.Log("No Meshrenderers found on " + curSkin);

    }

    IEnumerator StartMeshChange()
    {
        ActivateChangeMesh(true);
        yield return new WaitForSeconds(curData.changeMeshTime);
        ActivateChangeMesh(false);

    }

    void ActivateChangeMesh(bool _activate)
    {
        if (_activate)
        {
            for (int i = 0; i < meshesToChange.Length; i++)
            {
                if (meshesToChange[i])
                    meshesToChange[i].material = curData.materialToUse;
            }
        }
        else
        {
            for (int i = 0; i < meshesToChange.Length; i++)
            {
                if (meshesToChange[i])
                    meshesToChange[i].material = startMeshesMaterials[i];
            }
        }

    }

    public virtual void RefreshUIValues()
    {
        //set ui values to the vitals
        for (int i = 0; i < valueAmountLocals.Length; i++)
        {
            valueAmountLocals[i].RefreshUI();
        }
    }

    public virtual void Die(string _reason = default(string))
    {
        dead = true;
        curLives--;
        ActivateAllBuffs(false);

        //anim
        if (anim)
            anim.PlayDead();

        
        DoNoLivesOptions();
        StartCoroutine(StartDeath());
    }

    void DoDeathFX()
    {
        if (curData.deathFX.Length > 0)
        {
            foreach (var fx in curData.deathFX)
            {
                fx.DoFX(gameObject, gameObject);
            }
        }
    }

    void DoNoLivesOptions()
    {
        if (curData.noLivesOptions == UnitData.NoLivesType.GameOverWin)
            GameManager.instance.LevelWin(gameObject.name + " has died!", curData.endTime, true);
        else if (curData.noLivesOptions == UnitData.NoLivesType.GameOverLose)
        {
            if (curLives <= 0)
                GameManager.instance.GameOverLose();
        }
            
    }

    IEnumerator StartDeath()
    {
        ActivateDeathBehaviours(false);
        DoDeathFX();

        Vector2 lastPos = transform.position;
        float timer = 0;
        while (timer < curData.deathTime)
        {
            timer += Time.deltaTime;
            if (timer > curData.deathTime)
                timer = curData.deathTime;
            if (curData.deathMovement == UnitData.DeathMovement.StopIfGrounded && controller)
            {
                if (controller.IsGrounded)
                    transform.position = lastPos;
                else
                    lastPos = transform.position;
            }
            else if (curData.deathMovement == UnitData.DeathMovement.FallThrough)
            {
                if (col)
                    col.enabled = false;
            }   
            yield return new WaitForFixedUpdate();
        }

        if (curData.deathOptions == UnitData.DeathType.Respawn)
            Respawn();
        else if (curData.deathOptions == UnitData.DeathType.Destroy)
            Destroy(gameObject, curData.delay);
    }

    void ActivateDeathBehaviours(bool _activate)
    {
        //optional disable other components
        foreach (var deathComp in disableOnDeath)
        {
            if (deathComp)
                deathComp.enabled = _activate;
        }
    }

    protected virtual void Respawn()
    {
        if (curLives <= 0 && !curData.infiniteLives)
            return;

        StartCoroutine(StartRespawn());
    }

    public IEnumerator StartRespawn()
    {
        yield return new WaitForSeconds(curData.respawnTime);

        if (curData.resetLevelOnDeath)
            //reset level at checkpoint 
            ResetLevel();
        else
        {
            ResetUnitPosition(spawnPos);
            //reset player health
            SetupUnit(true, false);
        }
            
    }

    protected virtual void ResetUnitPosition(Vector2 _pos)
    {
        Debug.Log("resetting " + gameObject.name + " position to " + _pos);
        transform.position = _pos;
        if (rb)
            rb.Sleep();
        if (col)
            col.enabled = true;
    }

    void ResetLevel()
    {
        GameManager.instance.GetSceneTransitionData().ResetCurLevel();
    }

}
