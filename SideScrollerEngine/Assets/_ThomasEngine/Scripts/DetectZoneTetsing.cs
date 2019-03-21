using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectZoneTetsing : MonoBehaviour
{
    [SerializeField] private DetectZone detectZone;

    float timer = 0;

    private void Start()
    {
        detectZone.AddEnterTrigger(this, EnterTest);
        detectZone.AddExitTrigger(this, ExitTest);
        detectZone.AddStayTrigger(this, StayTest);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            detectZone.RemoveEnterTrigger(this, EnterTest);
            detectZone.RemoveExitTrigger(this, ExitTest);
            detectZone.RemoveStayTrigger(this, StayTest);
        }
    }

    void EnterTest(Collider2D _col)
    {
        Debug.Log(_col.gameObject + " entered");
    }

    void StayTest(Collider2D _col)
    {
        Debug.Log(_col.gameObject + " stays");
    }

    void ExitTest(Collider2D _col)
    {
        Debug.Log(_col.gameObject + " Exits");
    }
}
