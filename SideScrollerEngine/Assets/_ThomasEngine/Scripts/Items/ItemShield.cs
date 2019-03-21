using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShield : ItemAimable
{
    [System.Serializable]
    private class HeldCol
    {
        public Collider2D col;
        public Vector2 localPos;
        public Vector2 dir;
        public float dist;

        public bool IsMatch(Collider2D _col)
        {
            if (col)
                return col.transform == _col.transform;
            else
                return false;
        }
    }

    public new ItemShieldData Data { get { return (ItemShieldData)data; } }
    public GameObject spawnedShield;

    private bool shielding;

    private Collider2D[] cols;
    private List<Collider2D> enteredCols = new List<Collider2D>();
    private List<Collider2D> bouncedCols = new List<Collider2D>();
    private float lastLength;

    private List<HeldCol> heldCols = new List<HeldCol>();
    private bool holding;
    public int lastHeldColsCount;

    private bool rotLocked;
    private Quaternion lastRot;
    private Vector2 aimPos;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        shielding = false;
        if (spawnedShield)
            Destroy(spawnedShield.gameObject);
    }

    private void Update()
    {
        GetInputs();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        DetectHolding();

    }
    
    void GetInputs()
    {
        if (Input.GetButtonDown(Data.toggleShieldButton.stringValue))
            ToggleShield();

        holding = Input.GetButton(Data.grabButton.stringValue);
    }

    void ToggleShield()
    {
        base.OnRunItem();
        shielding = !shielding;
        if (shielding)
        {
            if (!spawnedShield)
                spawnedShield = Instantiate(Data.shieldPrefab, muzzle.position, muzzle.rotation);
            StartCoroutine(StartShielding());
        }
        else
            Destroy(spawnedShield.gameObject);
    }

    IEnumerator StartShielding()
    {
        rotLocked = false;
        lastRot = new Quaternion();
        EnableUnitMovement(false);
        while (shielding)
        {
            CheckDeactivate();
            RotateShield();
            CheckHeightLock();
            ClampShieldDistance();
            SetShieldPosition();

            if (Data.bounceProjectiles && IsFireReady)
            {
                DetectBounce();
            }

            yield return new WaitForFixedUpdate();
        }
        EnableUnitMovement(true);
    }

    void EnableUnitMovement(bool _enable)
    {
        if (!Data.disableUnitMovementWhenShieldActive)
            return;

        var unitCont = curUnitOwner.GetComponent<UnitController>();
        if (unitCont)
        {
            unitCont.DisableMovement(!_enable);
            unitCont.JumpEnabled = _enable;
        }
    }

    void CheckDeactivate()
    {
        if (Data.deactivateShieldOnReload)
        {
            if (!IsFireReady)
            {
                if (spawnedShield.activeSelf)
                {
                    EnableUnitMovement(true);
                    spawnedShield.SetActive(false);
                }
            }  
            else if (IsFireReady)
            {
                if (!spawnedShield.activeSelf)
                {
                    EnableUnitMovement(false);
                    spawnedShield.SetActive(true);
                }  
            }
                
        }
    }

    void RotateShield()
    {
        //face shield direction
        spawnedShield.transform.up = controller.AimDirection;
    }

    void CheckHeightLock()
    {
        aimPos = controller.AimPos;
        if (Data.lockToModelPrefabHeight)
        {
            if (controller.AimPos.y < muzzle.position.y)
            {
                if (!rotLocked)
                {
                    lastRot = spawnedShield.transform.rotation;
                    rotLocked = true;
                }

                aimPos = new Vector2(aimPos.x, muzzle.position.y);
                spawnedShield.transform.rotation = lastRot;
            }
            else
                rotLocked = false;
        }
    }

    void ClampShieldDistance()
    {
        var dist = Vector2.Distance(muzzle.position, aimPos);
        //set clamp distance
        var curclamp = Mathf.Clamp(dist, Data.minDistance, Data.maxDistance);
        //set shield pos
        Vector2 origin = aimPos - (Vector2)muzzle.position;
        origin *= curclamp / dist;
        aimPos = (Vector2)muzzle.position + origin;
    }

    void SetShieldPosition()
    {
        spawnedShield.transform.position = aimPos;
    }

    void DetectBounce()
    {
        cols = Physics2D.OverlapBoxAll(spawnedShield.transform.TransformPoint(Data.detectOffset), Data.detectSize, spawnedShield.transform.eulerAngles.z, Data.bounceMask);

        if (cols.Length > 0)
        {
                CheckColliderEntered();
                CheckColliderExited();

            if (holding)
            {
                if (heldCols.Count < cols.Length)
                {
                    heldCols.Clear();
                    foreach (var col in cols)
                        AddHeldCol(col);
                }
                if (lastHeldColsCount != heldCols.Count)
                {
                    var proj = heldCols[heldCols.Count - 1].col.GetComponent<Projectile>();
                    if (proj)
                        RemoveAmount(proj.Damage);
                    lastHeldColsCount = heldCols.Count;
                }

            }
            else
            {
                foreach (var col in cols)
                {
                    BounceObject(col);
                }
                foreach (var col in enteredCols)
                {
                    if (!bouncedCols.Contains(col))
                    {
                        bouncedCols.Add(col);
                    }
                    else
                        return;
                    var proj = col.GetComponent<Projectile>();
                    if (proj)
                        RemoveAmount(proj.Damage);
                }
                
            }
        }
        else if (enteredCols.Count > 1 || bouncedCols.Count > 1)
        {
            enteredCols.Clear();
            bouncedCols.Clear();
        }
            
    }

    void CheckColliderEntered()
    {
        foreach (var col in cols)
        {
            if (!enteredCols.Contains(col))
                enteredCols.Add(col);
        }
    }

    void CheckColliderExited()
    {
        if (cols.Length < 1)
        {
            enteredCols.Clear();
            return;
        }    
        for (int i = 0; i < enteredCols.Count; i++)
        {
            bool match = false;
            foreach (var col in cols)
            {
                match = col == enteredCols[i];
            }
            if (!match)
                enteredCols.Remove(enteredCols[i]);
        }
    }

    void DetectHolding()
    {
        if (Data.grabObjects)
        {
            if (holding && IsFireReady)
            {
                for (int i = 0; i < heldCols.Count; i++)
                {
                    //set shield pos
                    var worldPos = spawnedShield.transform.TransformPoint(heldCols[i].localPos);
                    if (heldCols[i].col)
                    {
                        var rb = heldCols[i].col.GetComponent<Rigidbody2D>();
                        if (rb)
                            rb.Sleep();
                        heldCols[i].col.transform.position = worldPos;
                    }
                        
                }
            }
            else if (heldCols.Count > 0)
            {
                foreach (var heldCol in heldCols)
                {
                    if (heldCol.col)
                    {
                        if (Data.deactivateShieldOnReload && !IsFireReady)
                            Destroy(heldCol.col.gameObject);
                        else
                            BounceObject(heldCol.col);
                    }

                }
                heldCols.Clear();
            }

        }
    }

    void AddHeldCol(Collider2D _col)
    {
        var pro = _col.GetComponent<Projectile>();
        if (pro)
            pro.PauseProjectile(true);
        Vector2 pos = _col.transform.position;
        var heldCol = new HeldCol()
        {
            col = _col,
            localPos = spawnedShield.transform.InverseTransformPoint(pos),
            dir = (pos - (Vector2)spawnedShield.transform.position).normalized,
            dist = Vector2.Distance(spawnedShield.transform.position, pos)
        };
        heldCols.Add(heldCol);
    }

    void BounceObject(Collider2D _col)
    {
        //did bullet hit back of shield?
        var dir = (_col.transform.position - spawnedShield.transform.position).normalized;
        var localDir = spawnedShield.transform.InverseTransformDirection(dir);
        if (localDir.y < 0)
            return;

        //get projectile component from hit
        var pro = _col.GetComponent<Projectile>();
        if (!pro)
        {
            Utils.LogComponentNullError(typeof(Projectile), _col.gameObject);
            return;
        }

        //set force settings
        var force = Data.bounceForce;
        if (Data.useIncomingForce)
            force = pro.VelocitySpeed;
        var mask = Data.damageMask;
        if (!Data.changeDamageMaskAfterBounce)
            mask = pro.Mask;
        //pause projectile (sleep)
        pro.PauseProjectile(false);
        //shoot in controller direction
        pro.ShootProjectile(force, pro.Damage, controller.AimDirection, mask, pro.Sender, pro.Target, pro.TargetPos, true);
        //RemoveAmount(pro.Damage);
    }
}
