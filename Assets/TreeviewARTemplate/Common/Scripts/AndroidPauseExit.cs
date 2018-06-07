using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidPauseExit : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

// We don't need to do this in iOS since there is the option PlayerSettings BehaviorOnBackground: Exit
#if UNITY_ANDROID
    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            Application.Quit();
        }

        //if (focus)
        //{
        //    //TODO:: destroy everything and reload the initial scene - need to specially handle Singletons


        //    SceneManager.LoadScene(0);
        //}
    }
#endif
}
