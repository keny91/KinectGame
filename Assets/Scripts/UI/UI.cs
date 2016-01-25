using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI : MonoBehaviour {


    public GameController gameController;
    public GameObject KinectObject;
    public GameObject gameObject;
    public BallControll ballController;
    public GameObject ballObject;
    public KinectTranslate KinectController;

    protected float lvl_timer;
    protected float CurrentTime;
    protected float timeLeft;
    protected Text NameText;
    public Text CountText;
    public Text WhoWon;
    public Text textScore1, textScore2;
  


    public bool gameRunning;
    public bool gamePaused;
    protected string lvlname;

    public GameObject GameMenuResult;
    public GameObject GameGoalScored;

    AudioSource audioS;
    public AudioClip AudioWon;


    public void Paused()
    {

        gameRunning = false;
        gamePaused = true;
    }

    public void Resume()
    {
        gameRunning = true;
        gamePaused = false;
        //time_ref = (int)Time.time;
    }



    protected void TimeUp(float time)
    {
        if (time <= 0)
        {

            /*
            
            audioS.clip = Abad;
            audioS.Play();
            */
            print("time UP!");

            // Case: 1  player won
            ballController.isStopped = true;
            textScore1.text = (gameController.score_player1).ToString();
            textScore2.text = (gameController.score_player2).ToString();
            GameMenuResult.SetActive(true);
            WhoWon = GameObject.Find("TextWho").GetComponent<Text>();
            if (gameController.score_player1> gameController.score_player2)
            {

                
                WhoWon.text = "YOU WON!";
                
            }
            else if (gameController.score_player1 < gameController.score_player2)
            {
                
                WhoWon.text = "Sorry, You lost!";

            }
            else if (gameController.score_player1 == gameController.score_player2)
            {
                WhoWon.text = "IT´S A TIE!";

            }


            
            //NOT READING AFTER gameMenuLOSE
            audioS.clip = AudioWon;
            audioS.Play();
            Paused();
        }
    }





    public void GoalReached()
    {
        Paused();
        /*
        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = Agood;
        audioS.Play();
        */
        GameGoalScored.SetActive(true);
    }





    // Use this for initialization
    void Start () {
        audioS = gameObject.AddComponent<AudioSource>();
        gameObject = GameObject.Find("GameController");
        gameController = (GameController)gameObject.GetComponent(typeof(GameController));

        ballObject = GameObject.Find("ObjectBall");
        ballController = (BallControll)ballObject.GetComponent(typeof(BallControll));
        audioS = gameObject.AddComponent<AudioSource>();
        Resume();
        GameMenuResult.SetActive(false);
        GameGoalScored.SetActive(false);


        lvl_timer = 120;

        lvlname = "Demo";
        CountText.text = (lvl_timer).ToString();
        CurrentTime = 0;
        //NameText = GameObject.Find("LvlName").GetComponent<Text>();
        CountText = GameObject.Find("CountText").GetComponent<Text>();
        



        //NameText.text = lvlname;
        CountText.text = lvl_timer.ToString();


    }
	
	// Update is called once per frame
	void Update () {
        int ThirdTime = (int)(lvl_timer / 3);
        int TenSeconds = 10;


        if (gameRunning)
        {
            // TIME UPDATE
            CurrentTime += Time.deltaTime;
            timeLeft = lvl_timer - CurrentTime;
            timeLeft = Mathf.Round(timeLeft);
            CountText.text = timeLeft.ToString();
            TimeUp(timeLeft);


            //result update
            textScore1.text = gameController.score_player1.ToString();
            textScore2.text = gameController.score_player2.ToString();


            // COLOR CHANGE
            if (timeLeft < ThirdTime)
            {
                if (timeLeft < TenSeconds)
                    CountText.color = Color.red;
                else
                    CountText.color = Color.yellow;

            }
            else
            {
                CountText.color = Color.black;
            }




            //objectBall.OnTriggerEnter(collider);
        }

    }
}
