using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSetActive : MonoBehaviour
{

    [SerializeField]
    private GameObject setActiveObject;
    [SerializeField]
    private bool deactivateOnExit = true;


	void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            if (setActiveObject)
                setActiveObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.tag == "Player" && deactivateOnExit)
        {
            if (setActiveObject)
                setActiveObject.SetActive(false);
        }
    }
}
