using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickAndDrag : MonoBehaviour
{

    [SerializeField] private LayerMask draggableMask;
    [SerializeField] private string dragButton = "Fire1";
    [SerializeField] private float throwPower = 5;

    private Vector2 mousePos;
    private Vector2 aimPos;

	// Use this for initialization
	void Start ()
    {
        GetComponents();
	}
	
	// Update is called once per frame
	void Update ()
    {
        MouseAim();
        GetInputs();
	}

    void GetComponents()
    {
        //aim = GetComponent<PlayerAim>();
    }

    void MouseAim()
    {
        //return if controller is connected
        if (Input.GetJoystickNames().Length > 0)
            return;

        //get mouse position on screen
        mousePos = Input.mousePosition;
        aimPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
    }

    void GetInputs()
    {
        if (Input.GetButtonDown(dragButton))
            Drag();
    }

    private void Drag()
    {
        Collider2D col = Physics2D.OverlapPoint(aimPos, draggableMask);
        if (col)
        {
            StartCoroutine(StartDrag(col.gameObject));
        }
    }

    public void StopDrag()
    {
        StopAllCoroutines();
    }

    IEnumerator StartDrag(GameObject _draggable)
    {
        var rb = _draggable.GetComponent<Rigidbody2D>();
        var lastPos = aimPos;
        var vel = Vector2.zero;
        while (Input.GetButton(dragButton))
        {
            vel = aimPos - lastPos;
            _draggable.transform.position = aimPos;
            lastPos = aimPos;
            yield return new WaitForFixedUpdate();
        }
        if (rb)
            rb.velocity = vel * throwPower;
        
    }
}
