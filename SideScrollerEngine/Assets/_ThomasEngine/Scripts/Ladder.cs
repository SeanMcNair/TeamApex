using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            PlayerController pl = col.GetComponent<PlayerController>();
            pl.IsOnLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            PlayerController pl = col.GetComponent<PlayerController>();
            pl.IsOnLadder = false;
        }
    }
}
