using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Preferences : MonoBehaviour {

    public int difficulty;
    public bool leftie;
    public Toggle leftFietoogle;
    public Dropdown theDrop;


	// Use this for initialization
	void Start () {
	
	}
	

    public void OnStartGame()
    {
        leftie = leftFietoogle.isOn;
        difficulty = theDrop.value;
        DontDestroyOnLoad(transform.gameObject);
        Application.LoadLevel(1);
        print(difficulty);
    }



	// Update is called once per frame
	void Update () {
	
	}
}
