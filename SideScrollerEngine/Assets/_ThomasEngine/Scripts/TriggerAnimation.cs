using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour {


    [Header("Choose an Animation file to play or Animator")]
    [SerializeField]
    private Animation animFile;
    [SerializeField]
    private Animator mechanim;
    [Header("Choose a state to start playing if Animator is chosen")]
    [SerializeField]
    private string mechanimState;
    [Header("Name a bool to trigger as an option instead (optional)")]
    [SerializeField]
    private string boolName;

    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            if (animFile)
                animFile.Play();
            else if (mechanim)
            {
                if (boolName != null)
                    StartCoroutine(TriggerBool());
                else
                    mechanim.Play(mechanimState);
            }
                
        }
    }

    IEnumerator TriggerBool()
    {
        mechanim.SetBool(boolName, true);
        yield return new WaitForEndOfFrame();
        mechanim.SetBool(boolName, false);
    }
}
