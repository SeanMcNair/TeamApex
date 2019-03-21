using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageArea : MonoBehaviour
{
    [System.Serializable]
    public class DamageZone
    {
        //inspector variables
        public bool activeOnStart = true;
        public enum ZoneType { Box, Circle }
        public ZoneType zoneType = ZoneType.Box;
        public string zoneName = ZoneType.Box.ToString();
        public enum CenterType { Offset, Transform }
        public CenterType centerType;
        public Transform transPoint;
        public Vector2 center = Vector2.up;
        public Vector2 boxSize = Vector2.one;
        public enum AngleType { Override, TransformZ }
        public AngleType angleType;
        public float angle;
        public float radius = 0.5f;
        public int damageAmount = 1;
        public float bounceForce;
        public enum BounceDirectionSetup { ClosestPointAngle,XOnly, Override }
        public BounceDirectionSetup bounceSetup;
        public Vector2 bounceDir = Vector2.up;

        //"private" variables
        public bool active;
        public Vector2 curCenter;
        public List<Collider2D> enteredCols = new List<Collider2D>();
        public List<Collider2D> damagedCols = new List<Collider2D>();

        //gui variables
        public bool opened = true;
        public bool areYouSureActive = false;
        public ZoneType lastType;
        public Vector2 handlePoint;
    }

    public List<DamageZone> zones = new List<DamageZone>();
    [SerializeField]
    private LayerMask damageMask;

    private void Start()
    {
        SetupZones();
    }

    private void LateUpdate()
    {
        DetectAreas();
    }

    void SetupZones()
    {
        foreach (var zone in zones)
        {
            if (zone.activeOnStart)
                zone.active = true;
        }
    }

    void DetectAreas()
    {
        if (zones.Count < 1)
            return;

        foreach (var zone in zones)
        {
            //go back if not active
            if (!zone.active)
                return;

            //set offset
            if (zone.centerType == DamageZone.CenterType.Offset)
                zone.curCenter = (Vector2)transform.position + zone.center;
            else if (zone.centerType == DamageZone.CenterType.Transform)
                zone.curCenter = zone.transPoint.position;

            //get colliders
            if (zone.zoneType == DamageZone.ZoneType.Box)
            {
                if (zone.centerType == DamageZone.CenterType.Transform && zone.angleType == DamageZone.AngleType.TransformZ)
                {
                    //flip angle to transform Z depending on Y rotation
                    float yAngle = zone.transPoint.eulerAngles.y;
                    if (yAngle > 90)
                        zone.angle = -zone.transPoint.eulerAngles.z;
                    else
                        zone.angle = zone.transPoint.eulerAngles.z;
                }
                    
                zone.enteredCols = Physics2D.OverlapBoxAll(zone.curCenter, zone.boxSize, zone.angle, damageMask).ToList();
            }
            else if (zone.zoneType == DamageZone.ZoneType.Circle)
                zone.enteredCols = Physics2D.OverlapCircleAll(zone.curCenter, zone.radius, damageMask).ToList();

            //Check for entered collisions
            if (zone.enteredCols.Count > 0)
            {
                //check if damaged yet
                foreach (var col in zone.enteredCols)
                {
                    if (!zone.damagedCols.Contains(col) && col.gameObject != gameObject)//only damage once and not self
                    {
                        //gather direction information
                        Vector2 dir = zone.bounceDir;
                        if (zone.bounceSetup == DamageZone.BounceDirectionSetup.ClosestPointAngle)
                            dir = ((Vector2)col.bounds.ClosestPoint(zone.curCenter) - zone.curCenter).normalized;
                        else if (zone.bounceSetup == DamageZone.BounceDirectionSetup.XOnly)
                        {
                            dir = (Vector2)col.transform.position - zone.curCenter;
                            if (dir.x > 0)
                                dir = Vector2.right;
                            else if (dir.x < 0)
                                dir = Vector2.left;
                        }

                        DoDamage(col, zone.damageAmount,zone.bounceForce, dir);
                        zone.damagedCols.Add(col);
                    }
                       
                }
                
            }
            if (zone.damagedCols.Count > 0)
            {
                //check if damaged items left area
                for (int i = 0; i < zone.damagedCols.Count; i++)
                {
                    if (!zone.enteredCols.Contains(zone.damagedCols[i]))
                        zone.damagedCols.Remove(zone.damagedCols[i]);//Remove if exited zone
                }
            }

        }
    }

    void DoDamage(Collider2D _col, int _damage, float _bounceForce, Vector2 _dir)
    {
        var obj = _col.gameObject;

        var unit = obj.GetComponent<Unit>();
        if (unit)
        {
            unit.DamageHp(_damage, _bounceForce, _dir);
        }
    }

    /// <summary>
    /// Sets a specific damage zone active by int/index.
    /// </summary>
    /// <param name="_zoneInd">The index location of the zone to set active</param>
    /// <param name="_active">Set the zone active to true or false?</param>
    /// <param name="_time">[Optional] The amount of time to set active or not. Switches back after time is over</param>
    public void SetZoneActive(int _zoneInd, bool _active, float _time = default(float), float _delay = default(float))
    {
        if (_time > 0 || _delay > 0)
        {
            StopAllCoroutines();
            StartCoroutine(StartSetZoneActive(_zoneInd, null, _active, _time, _delay));  
        }             
        else
        {
            if (_zoneInd < zones.Count)
                zones[_zoneInd].active = _active;
            else
                Debug.LogError("No zone found at this index: " + _zoneInd + " on " + this.name);
        }
    }
    /// <summary>
    /// Sets a specific damage zone active by string/ZoneName.
    /// </summary>
    /// <param name="_zoneName">The name of the zone to set active</param>
    /// <param name="_active">Set the zone active to true or false?</param>
    /// <param name="_time">[Optional] The amount of time to set active or not. Switches back after time is over</param>
    public void SetZoneActive(string _zoneName, bool _active, float _time = default(float), float _delay = default(float))
    {
        if (_time > 0 || _delay > 0)
        {
            StopAllCoroutines();
            StartCoroutine(StartSetZoneActive(0, _zoneName, _active, _time, _delay));
        }    
        else
        {
            FindZoneByName(_zoneName).active = _active;
        }

    }
    /// <summary>
    /// Sets a specific damage zone active by string or int, with a frame delay and frame active window.
    /// </summary>
    /// <param name="_zoneName">The name of the zone to set active</param>
    /// <param name="_active">Set the zone active to true or false?</param>
    /// <param name="_zoneInd">The index location of the zone to set active</param>
    /// <param name="_framesActive">The amount of frames to set active or not. Switches back after frame count</param>
    /// <param name="_frameDelay">The amount of frames to delay the action</param>
    public void SetZoneActive(bool _active, int _framesActive, int _frameDelay, string _zoneName = default(string), int _zoneInd = default(int))
    {
        StopAllCoroutines();
       StartCoroutine(StartSetZoneActive(_zoneInd, _zoneName, _active, _framesActive, _frameDelay));
    }
    /// <summary>
    /// Set all zones to active or not active
    /// </summary>
    /// <param name="_active">Set active to true or false</param>
    public void SetAllZonesActive(bool _active)
    {
        foreach (var zone in zones)
        {
            zone.active = _active;
        }
    }

    IEnumerator StartSetZoneActive(int _zoneInd, string _zoneName, bool _active, int _framesActive, int _frameDelay)
    {
        DamageZone zone = null;
        if (_zoneName != null)
            zone = FindZoneByName(_zoneName);
        else
            zone = zones[_zoneInd];

        int delay = 0;
        while (delay < _frameDelay)
        {
            delay++;
            yield return new WaitForEndOfFrame();
        }
        zone.active = _active;
        int active = 0;
        while (active < _framesActive)
        {
            active++;
            yield return new WaitForEndOfFrame();
        }
        zone.active = !_active;
    }

    IEnumerator StartSetZoneActive(int _zoneInd, string _zoneName, bool _active, float _time, float _delay)
    {
        DamageZone zone = null;
        if (_zoneName != null)
            zone = FindZoneByName(_zoneName);
        else
            zone = zones[_zoneInd];
        //delay
        float timer = 0;
        while (timer < _delay)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        zone.active = _active;
        //active Time
        timer = 0;
        while (timer < _time)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        zone.active = !_active;
    }

    DamageZone FindZoneByName(string _zoneName)
    {
        foreach (var zone in zones)
        {
            if (zone.zoneName == _zoneName)
            {
                return zone;
            }
        }
        Debug.LogError("Could not find zone name: " + _zoneName + " in " + this.name);
        return null;
    }

}
