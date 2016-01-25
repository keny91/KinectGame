using UnityEngine;
using System.Collections;

public class BackGroundMusic : MonoBehaviour {

    /*
        This class PLAYS a music file associated to the "music" GameObject in the menu
    */

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("Music"));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
