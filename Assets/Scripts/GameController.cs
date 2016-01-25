using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public int score_player1;
    public int score_player2;
    int difficulty;
    public string body;

    //GameObject GameControlObject;
    public GameController gameController;
    public BallControll ballController;
    public AI AIController;
    GameObject UInterface;
    GameObject ObjectBall;
    GameObject ObjectAI;
    Vector3 RivalsOriginalPosition; // Rival´s starting point
    


    public float BallSpeed;
    public float RivalReactDistance;
    public float RivalSpeed;
    public bool RivalReposition;    // The rival will try to reposition himself in the origin when not reaching for the ball
    public bool leftie;    // If true the player is left-handed

    // This is the section where the rival is allowed to move
    float RivalMoveLimit_UP = 6.9f;
    float RivalMoveLimit_DOWN =1f;
    float RivalMoveLimit_LEFT = -3.9f;
    float RivalMoveLimit_RIGTH = 3.9f;

    GameObject PreferencesObject;
    Preferences preferencesController;



    // Use this for initialization
    void Start () {

        // Init Scores
        score_player1 = 0;
        score_player2 = 0;

        body = "undef";
        

        ObjectBall = GameObject.Find("ObjectBall");
        ballController = (BallControll)ObjectBall.GetComponent(typeof(BallControll));
        PreferencesObject = GameObject.Find("Preferences");
        preferencesController = (Preferences)PreferencesObject.GetComponent(typeof(Preferences));
        ObjectAI = GameObject.Find("Rival");
        AIController = (AI)ObjectAI.GetComponent(typeof(AI));
        RivalsOriginalPosition = ObjectAI.transform.position;
        /* 
            Depending on the difficulty or the game mode, we set a different set of parameters
        */

        //Get difficulty
        //difficulty = 1;

        difficulty = preferencesController.difficulty;
        leftie = preferencesController.leftie;

        switch (difficulty)
        {
            /*
                GAME MODE 1:
                    Normal speed, rival does not reallocate after hitting
            */
            case 0:
                print("Selected Easy Difficulty");
                BallSpeed=5f;
                RivalReactDistance= 20f;
                RivalSpeed = 2f;
                RivalReposition = true;
                break;
            /*


            GAME MODE 1:
                Normal speed, rival does not reallocate after hitting
        */

            case 1:
                print("Selected normal Difficulty");
                BallSpeed = 10f;
                RivalReactDistance = 20f;
                RivalSpeed = 2f;
                RivalReposition = true;
                break;


            case 2:
                print("Selected Hard Difficulty");
                BallSpeed = 10f;
                RivalReactDistance = 10f;
                RivalSpeed = 1f;
                RivalReposition = false;
                break;



            default:
                print("Default Game Mode");
                BallSpeed = 10f;
                RivalReactDistance = 10f;
                RivalSpeed = 1f;
                RivalReposition = true;
                break;

        }


    }



    /*
        A quick method to start a waiting courotine
        
        */









    /*
        SCORE: quick method
        */

    public void score(int player)
    {
        switch (player)
        {
            case 1:
                score_player1++;
                
                break;
            case 2:
                score_player2++;

                break;

            default:
                print("\n Wrong parameter inserted, check call to SCORE.");
                break;
        }

        print("Score: Player1: "+ score_player1+ " Player2: "+ score_player2);
    }



    /*
        This function helps to determine where the ball will hit

        RETURNS the point where the ball will aproximatelly hit
        */
    public Vector3 rayImpact(bool paintTrack)
    {
        if (ballController.isStopped)
            return new Vector3(0,0,0);

        bool done = false;
        RaycastHit hit;
        Vector3 refPoint;
        Vector3 direction;
        int bounce_limit = 20;
        int bounce_counter = 0;
        Color rayColor = Color.white;
        Ray BallTrack;
        // We start by casting a ray from the ball to the next wall


        refPoint = ObjectBall.transform.position;
        direction = ObjectBall.GetComponent<Rigidbody>().velocity;
        //Ray BallTrack = new Ray(refPoint, direction);
        //Physics.Raycast(BallTrack, out hit, 100);
        


        while (done == false  && bounce_counter< bounce_limit)
        {
            BallTrack = new Ray(refPoint, direction);
            Physics.Raycast(BallTrack, out hit, 100);

            if (hit.transform.gameObject.tag == "VerticalWall")
            {
                direction.y = -direction.y;
                rayColor = Color.white;

            }
            else if(hit.transform.gameObject.tag == "HorizontalWall")
            {
                direction.x = -direction.x;
                rayColor = Color.white;
            }
            else if(hit.transform.gameObject.tag == "Goal")
            {

                rayColor = Color.red;
                done = true;
            }

            if(paintTrack)
                Debug.DrawLine(refPoint, hit.point, rayColor);


            refPoint = hit.point;
            bounce_counter++;
        }
        return refPoint; 
    }




    /*
        MoveRival: the rival block will move to the impact point
        */
        void MoveRival(Vector3 ToPoint)
    {
        float step = RivalSpeed * Time.deltaTime;
        // We have to maintain the rival´s Z position
        Vector3 newPositionVector = ObjectAI.transform.position;

        // Horizontal limit check
        if (ToPoint.x <= RivalMoveLimit_RIGTH)
        {
            if(ToPoint.x >= RivalMoveLimit_LEFT)
                newPositionVector.x = ToPoint.x;
            else
                newPositionVector.x = RivalMoveLimit_LEFT;

        }
        else
        {
            newPositionVector.x = RivalMoveLimit_RIGTH;
        }



        // Vertical limit check
        if (ToPoint.y <= RivalMoveLimit_UP)
        {
            if (ToPoint.y >= RivalMoveLimit_DOWN)
                newPositionVector.y = ToPoint.y;
            else
                newPositionVector.y = RivalMoveLimit_DOWN;

        }
        else
        {
            newPositionVector.y = RivalMoveLimit_UP;
        }
        


        ObjectAI.transform.position = Vector3.MoveTowards(ObjectAI.transform.position, newPositionVector, step);
        //print("Position x:"+ObjectAI.transform.position.x+"\n");
        //print("Position y:" + ObjectAI.transform.position.y + "\n");
    }

    /*
        RivalStartKick: choose a random direction 
        Float speed: the speed that the ball will have.
        Int direction: the ball will randomly be fired in the XY axis, but we need to know (can be 1 or -1)
        1-> Moves toward opponent ; -1->moves towards the player
        */

   public Vector3 StartKick(float speed, int direction)
    {
        float Tvel = 0.5f;
        float Dvel = 1f;
        if (direction!=1 && direction != -1)
        {
            print("Wrong direction input at StartKick() " + direction + " was invalid, it can only be 1 or -1.\n");
            return new Vector3(0,0,0);
        }

        Vector3 ballVelocity = new Vector3(Random.Range(-Tvel, Tvel), Random.Range(-Tvel, Tvel), 0);
        double Vx = Tvel - Mathf.Abs(ballVelocity.x);
        double Vy = Tvel - Mathf.Abs(ballVelocity.y);
        ballVelocity.z = (float)(Dvel + (Vy + Vx) / 2);  // the smaller Vx and Vy are the bigger Vz will be
        ballVelocity.z = direction * ballVelocity.z;  // Direction in the X axis
        ballVelocity = speed * ballVelocity;

        return ballVelocity;
    }




    // Update is called once per frame
    void Update () {


        Vector3 pointAtGoal = new Vector3(0,0,0);
        Vector3 BallVelocity = ObjectBall.GetComponent<Rigidbody>().velocity;
        //Vector3 NewVelocity;

        Ray BallTrack = new Ray(ObjectBall.transform.position, BallVelocity);

        if (ballController.hasCollided)
        {
            pointAtGoal = rayImpact(true);

        }
        
        float distance = Vector3.Distance(ObjectBall.transform.position, pointAtGoal);  // Distance from the ball to any of the goals
        AIController.MoveSpeed = 2f;

        bool statement = ObjectBall.GetComponent<Rigidbody>().velocity.z > 0 && distance <= RivalReactDistance;
        // The Rival will only move when the Ball comes towards it and it is a certain distance from it.
        if (statement)
        {
            MoveRival(pointAtGoal);      
        }
        else if(!statement && RivalReposition)
        {
            MoveRival(RivalsOriginalPosition);
        }
            


        // IF velocity.z is 

        /*
        Vector2 relativePosition = new Vector2(pointAtGoal.x - ObjectAI.transform.position.x, pointAtGoal.y - ObjectAI.transform.position.y);
        if (ObjectAI.transform.position.x != pointAtGoal.x)
        {

            

            // if we move further than the point
            if (relativePosition.x < 0)
            {
                if (Mathf.Abs(relativePosition.x) < AIController.MoveSpeed)
                    ObjectAI.transform.Translate(relativePosition.x,0,0);
                else
                {
                    ObjectAI.transform.Translate(AIController.MoveSpeed, 0, 0);
                }
            }


            else if(relativePosition.x > 0)
            {
                if (Mathf.Abs(relativePosition.x) < AIController.MoveSpeed)
                    ObjectAI.transform.Translate(relativePosition.x, 0, 0);
                else
                {
                    ObjectAI.transform.Translate(AIController.MoveSpeed, 0, 0);
                }

            }
            
        }
        if (ObjectAI.transform.position.y != pointAtGoal.y)
        {

        }


        */



    }







}
