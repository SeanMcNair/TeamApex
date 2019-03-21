using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrawable : ItemAimable
{
    public new ItemDrawableData Data { get { return (ItemDrawableData)data; } }

    private bool drawing;
    public bool IsDrawing { get { return drawing; } }
    private bool growing;
    public bool IsGrowing { get { return growing; } }


    private Vector2 lastPos;
    private float aimdist;
    private bool drawable;
    private List<Vector2> drawPoints = new List<Vector2>();
    private TrailRenderer trail;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        growing = false;
        drawPoints.Clear();
        KillTrail();
    }

    private void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        if (Input.GetButtonDown(Data.drawButton.stringValue))
            CheckDrawArea();
        if (Input.GetButton(Data.drawButton.stringValue) && drawable)
            DrawLine();
        else if (Input.GetButtonUp(Data.drawButton.stringValue))
            StopDrawing();
    }

    void CheckDrawArea()
    {
        drawable = true;
        if (Data.useMask)
        {
            var drawableCols = Physics2D.OverlapPointAll(controller.AimPos, Data.drawableMask);
            var blockableCols = Physics2D.OverlapPointAll(controller.AimPos, Data.blockableMask);
            drawable = drawableCols.Length > 0 && blockableCols.Length < 1;
            if (!drawable)
            {
                StopUseItem();
                Debug.Log("Not a drawable area!");
                return;
            }
        }
        trail = Instantiate(Data.trailRenderer, controller.AimPos, Quaternion.identity).GetComponent<TrailRenderer>();
        lastPos = controller.AimPos;
        aimdist = 0;
    }

    void StopDrawing()
    {
        drawing = false;
        KillTrail();
        GrowLine();
    }

    void KillTrail()
    {
        if (trail)
            Destroy(trail.gameObject);
    }

    void DrawLine()
    {
        if (growing)
            return;

        if (ammoAmountLocal.CurAmount > 0 && aimdist < Data.aimDistance)
        {
            aimdist = Vector2.Distance(muzzle.position, controller.AimPos);
            trail.transform.position = controller.AimPos;
            if (lastPos != controller.AimPos)
                AddDrawPoint(controller.AimPos);
            lastPos = controller.AimPos;
        }
        else
            GrowLine();
    }

    void AddDrawPoint(Vector2 _pos)
    {
        if (drawPoints.Count > 0)
        {
            var dist = Vector2.Distance(_pos, drawPoints[drawPoints.Count - 1]);
            if (dist > Data.sensitivityDistance)
            {
                RemoveAmount(dist);
                drawPoints.Add(_pos);
            }    
        }
        else
            drawPoints.Add(_pos);
    }

    void GrowLine()
    {
        if (drawPoints.Count < 1)
            return;

        if (!growing)
            StartCoroutine(StartGrowLine());
    }

    IEnumerator StartGrowLine()
    {
        growing = true;
        int i = 0;
        var seed = Instantiate(Data.topPrefab, drawPoints[i], Quaternion.identity);
        float timer = 0;
        float distanceTravelled = 0;
        var lastPos = drawPoints[i];
        while (i < drawPoints.Count - 1 && seed)
        {
            //get drawpoints data
            var pos = drawPoints[i];
            var dest = drawPoints[i + 1];
            var dir = (drawPoints[i + 1] - drawPoints[i]).normalized;
            var distance = Vector2.Distance(pos, dest);

            //timer data
            var time = distance / Data.growSpeed;
            timer += Time.deltaTime;
            float perc = timer / time;

            if (!seed)
                break;

            //move the seed
            seed.transform.up = dir;
            seed.transform.position = Vector2.Lerp(pos, dest, perc);

            //how far has the seed moved?
            distanceTravelled += Vector2.Distance(lastPos, seed.transform.position);

            //spawn a vine after amount travelled
            if (distanceTravelled > Data.sensitivityDistance || i == 0)
            {
                SpawnLinePiece(seed.transform.position, dir);
                distanceTravelled = 0;
            }

            //go through all positions
            if (perc >= 1)
            {
                perc = 0;
                timer = 0;
                i++;
            }
                   
            //store last seed position for next loop
            lastPos = seed.transform.position;
            yield return new WaitForEndOfFrame();
        }
        growing = false;
        drawPoints.Clear();
        KillTrail();
    }

    void SpawnLinePiece(Vector2 _pos, Vector2 _dir)
    {
        var spawn = Instantiate(Data.growLinePrefab, _pos, Quaternion.identity);
        spawn.transform.up = _dir;
    }
}
