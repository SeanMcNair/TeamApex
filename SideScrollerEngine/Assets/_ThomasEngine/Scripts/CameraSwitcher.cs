using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSwitcher : MonoBehaviour 
{

    [System.Serializable]
    private class SceneCamera
    {
        public GameObject camObj;
        [Header("Animation Optional...must be LEGACY")]
        public Animation animMovement;
        public float activeTime = 1;       
    }

    [SerializeField]
    private SceneCamera[] cameras;
    [SerializeField]
    private bool loadSceneOnFinish = true;
    [SerializeField]
    private string sceneToLoadOnFinish;

    private float timer;
    private int curInd;
    private SceneCamera curCam;
    private float curTime;

	
    void Start()
    {
        SwitchCamera(curInd);
    }

	// Update is called once per frame
	void Update () 
	{
        timer += Time.deltaTime;
        if (timer > curTime)
        {
            if (curInd < cameras.Length - 1)
            {
                curInd++;
                SwitchCamera(curInd);
                timer = 0;
            }
            else if (loadSceneOnFinish)
            {
                SceneManager.LoadScene(sceneToLoadOnFinish);
            }
      
        }
	}

    void SwitchCamera(int _ind)
    {
        foreach (var cam in cameras)
        {
            if (cam == cameras[_ind])
            {
                curCam = cam;
                curTime = curCam.activeTime;
                curCam.camObj.SetActive(true);

                if (curCam.animMovement)
                    curCam.animMovement.Play();
            }                
            else
                cam.camObj.SetActive(false);
        }
    }
}
