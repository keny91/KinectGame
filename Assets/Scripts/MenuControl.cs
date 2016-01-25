using UnityEngine;
using System.Collections;

public class MenuControl : MonoBehaviour {

    public void LevelButton_1()
    {
        Application.LoadLevel("LVL1");
    }

    public void LevelButton_2()
    {
        Application.LoadLevel("LVL2");
    }

    public void LevelButton_3()
    {
        Application.LoadLevel("LVL3");
    }


    // Use this for initialization
    void Start () {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
