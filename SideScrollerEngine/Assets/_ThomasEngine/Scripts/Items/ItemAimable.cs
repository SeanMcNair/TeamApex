using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemAimable : ItemUseable
{
    public new ItemAimableData Data { get { return (ItemAimableData)data; } }
    protected Transform muzzle;
    public Transform Muzzle { get { return muzzle; } }
    protected LineRenderer line;
    protected Color startColor;
    protected Color endColor;
    protected Transform aimRef;
    protected UnitController controller;

    private List<GameObject> swapAimerRefs = new List<GameObject>();
    private Camera mainCamera;

    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
    }

    protected override void OnDisable()
    {
        KillAimerReferences();
        base.OnDisable();
    }

    protected virtual void FixedUpdate()
    {
        if (!controller)
            return;

        DetectReticalSwaps();
        DrawAimerReference();
        
    }

    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        if (dropped)
            return;
        controller = curUnitOwner.GetComponent<UnitController>();
        muzzle = transform.Find(Data.muzzlePos);
        SpawnLineRenderer();
        SpawnDefaultReticals();
        SetActiveAimReference(0);
    }

    void SpawnLineRenderer()
    {
        if (!Data.aimFX)
            return;

        if (Data.aimFX.lineRenderer)
        {
            line = Instantiate(Data.aimFX.lineRenderer, muzzle.position, Quaternion.identity).GetComponent<LineRenderer>();
            startColor = line.startColor;
            endColor = line.endColor;
            if (Data.aimFX.lineType == AimFX.LineType.Firefade)
                SetLineAlpha(0);
        }
    }

    void SpawnDefaultReticals()
    {
        if (!Data.aimFX)
            return;
        if (Data.aimFX.aimRetical)
            SpawnAimerReference(Data.aimFX.aimRetical);
        if (Data.aimFX.reticalSwaps.Length > 0)
        {
            foreach (var ret in Data.aimFX.reticalSwaps)
            {
                SpawnAimerReference(ret.swapReticalPrefab);
            }
        }   
    }

    void SpawnAimerReference(GameObject _spawn)
    {
        if (!Data.aimFX)
            return;

        if (_spawn && controller)
        {
            var aimer = Instantiate(_spawn);
            aimer.transform.rotation = Quaternion.identity;
            if (Data.aimFX.reticalPosType == AimFX.PositionType.World)
            {
                aimer.transform.position = controller.AimPos;
            }
            else
            {
                var ui = UIPlayer.instance;
                if (ui)
                {
                    aimer.transform.SetParent(ui.transform);
                    aimer.transform.localScale = Vector3.one;
                    aimer.transform.position = Input.mousePosition;
                }
                
            }

           swapAimerRefs.Add(aimer);
                
        }

    }

    void SetActiveAimReference(int _index)
    {
        if (swapAimerRefs.Count < 1)
            return;

        for (int i = 0; i < swapAimerRefs.Count; i++)
        {
            var aimer = swapAimerRefs[i];
            if (aimer)
            {
                aimer.SetActive(i == _index);
                if (aimer.activeSelf)
                    aimRef = aimer.transform;
            }
            
        }
    }

    void DrawAimerReference()
    {
        if (!Data.aimFX)
            return;

        PositionRetical();
        if (Data.aimFX.lineType == AimFX.LineType.Constant)
            DrawLine(controller.AimPos);
    }

    void PositionRetical()
    {
        if (!aimRef)
            return;

        if (Data.aimFX.reticalPosType == AimFX.PositionType.World)
            aimRef.position = controller.AimPos;
        else if (Data.aimFX.reticalPosType == AimFX.PositionType.ScreenCanvas)
        {
            var pos = mainCamera.WorldToScreenPoint(controller.AimPos);
            aimRef.position = pos;
        }
            
    }

    void DrawLine(Vector2 _endPos)
    {
        if (!line || _endPos == Vector2.zero)
            return;

        line.SetPosition(0, muzzle.position);
        line.SetPosition(1, _endPos);
    }

    void DetectReticalSwaps()
    {
        if (!Data.aimFX)
            return;

        if (Data.aimFX.reticalSwaps.Length < 1)
            return;

        if (swapAimerRefs.Count < 1)
            return;

        for (int i = 0; i < Data.aimFX.reticalSwaps.Length; i++)
        {
            var swap = Data.aimFX.reticalSwaps[i];
            var defaultRef = swapAimerRefs[0];
            var retTrans = swapAimerRefs[i + 1];
            var col = Physics2D.OverlapPoint(controller.AimPos, swap.swapMask);
            if (col && retTrans)
            {
                if (aimRef != retTrans.transform)
                    SetActiveAimReference(i + 1);
            }
            else if (defaultRef)
            {
                if (aimRef != defaultRef.transform)
                SetActiveAimReference(0);
            }
                
        }
    }

    public virtual void DoLineFX(Vector2 _endPos)
    {
        if (line)
        {
            if (Data.aimFX.lineType == AimFX.LineType.Firefade)
                StartCoroutine(StartLineFade(_endPos));
        }
    }

    IEnumerator StartLineFade(Vector2 _endPos)
    {
        DrawLine(_endPos);
        float timer = 0;
        while (timer < Data.aimFX.fadeTime)
        {
            timer += Time.deltaTime;
            var perc = timer / Data.aimFX.fadeTime;
            SetLineAlpha(Mathf.Lerp(startColor.a, 0, perc));
            yield return new WaitForEndOfFrame();
        }
    }

    void SetLineAlpha(float _value)
    {
        var start = new Color(startColor.r, startColor.g, startColor.b, _value);
        var end = new Color(endColor.r, endColor.g, endColor.b, _value);
        line.startColor = start;
        line.endColor = end;
    }

    void KillAimerReferences()
    {
        if (line)
            Destroy(line.gameObject);
        if (swapAimerRefs.Count > 0)
        {
            for (int i = 0; i < swapAimerRefs.Count; i++)
            {
                var swap = swapAimerRefs[i];
                if (swap)
                {
                    Destroy(swap.gameObject); 
                }
                   
            }
            swapAimerRefs.Clear();
        }
    }

}
