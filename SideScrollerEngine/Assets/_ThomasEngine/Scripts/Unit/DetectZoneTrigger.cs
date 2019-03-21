using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DetectZoneTrigger : MonoBehaviour
{
    [SerializeField] protected DetectZone detectZone;

    public bool IsDetected { get { return cols.Count > 0; } }

    private List<Collider2D> cols = new List<Collider2D>();
    private List<Collider2D> enteredCols = new List<Collider2D>();

    protected virtual void OnEnter(Collider2D _col) { }
    protected virtual void OnStay(Collider2D _col) { }
    protected virtual void OnExit(Collider2D _col) { }

    protected virtual void FixedUpdate()
    {
        DoDetection();
        CheckEntered();
        CheckExit();
    }

    void DoDetection()
    {
        cols = detectZone.DetectColliders(transform).ToList();
    }

    //check all entered colliders to make sure they match up with the cols
    void CheckExit()
    {
        if (enteredCols.Count < 1)
            return;

        for (int i = 0; i < enteredCols.Count; i++)
        {
            //match found?
            if (!cols.Contains(enteredCols[i]))//no
            {
                //a collider has exited detection zone
                OnExit(enteredCols[i]);
                enteredCols.Remove(enteredCols[i]);
            }
            else//yes
            {
                //the collider is still in detection zone
                OnStay(enteredCols[i]);
            }
        }

    }

    //check all cols to see if there are any new colliders
    void CheckEntered()
    {
        if (cols.Count < 1)
            return;

        for (int i = 0; i < cols.Count; i++)
        {
            //match found in entered?
            if (!enteredCols.Contains(cols[i]))
            {
                //new collider entered detection zone
                enteredCols.Add(cols[i]);
                OnEnter(cols[i]);
            }
        }

    }

}
