using UnityEngine;
using System.Collections;
using System.IO;


public class BallControll : MonoBehaviour {

    Vector3 speed;
    float currentSpeed;
    float speedFactor;
    public bool isStopped;
    bool WaitingForPlayerKick;
    public bool hasCollided;


    GameObject UInterface;
    GameObject ObjectBall;
    Transform RespanwPoint;
    GameObject OriginPoint;
    GameObject GameControlObject;
    public float MoveSpeed;
    public GameController gameController;
    public GameObject KinectObject;
    public KinectTranslate KinectController;
    private KinectTranslate.arm theArm;
    Vector3 OriginalPositionHand;


    AudioSource audioS;
    public AudioClip CollisionSound1, CollisionSound2, CollisionSound3, GoalClip;


    Vector3 HandBallPosition;

    //gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = false;

    //Firepower
    //child0
    //element
    //child1

    // Use this for initialization
    void Start () {
        OriginalPositionHand = new Vector3(0,0,0);
        //currentSpeed = 10f;
        HandBallPosition = GameObject.Find("RightHand").transform.position;
        speedFactor = 10f;
        WaitingForPlayerKick = false;
        isStopped = false;
        hasCollided = true;
        ObjectBall = GameObject.Find("ObjectBall");
        OriginPoint = GameObject.Find("Center");
        GameControlObject = GameObject.Find("GameController");
        KinectObject = GameObject.Find("KinectController");
        // Get the position of the respawning ball
        RespanwPoint = OriginPoint.transform.GetChild(0);
        //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(1, 1, 0);
        gameController = (GameController)GameControlObject.GetComponent(typeof(GameController));
        KinectController = (KinectTranslate)KinectObject.GetComponent(typeof(KinectTranslate));

        currentSpeed = gameController.BallSpeed;
        Vector3 velocityBall = gameController.StartKick(currentSpeed, 1);
        //Vector3 velocityBall = new Vector3(currentSpeed, currentSpeed / 2, currentSpeed);
        GetComponent<Rigidbody>().velocity = velocityBall;
        //GetComponent<Rigidbody>().AddForce(velocityBall);


        audioS = gameObject.AddComponent<AudioSource>();
        
        if (gameController.leftie)
        {
            theArm = KinectController.leftArm;
        }
        else
        {
            theArm = KinectController.rightArm;
        }

    }

    AudioClip SelectRandomClip()
    {
        AudioClip Clip;
        int decision = Mathf.CeilToInt(Random.Range(0f,3f));

        if (decision == 1)
            Clip = CollisionSound1;
        else if(decision == 2)
            Clip = CollisionSound2;
        else
            Clip = CollisionSound3;



        return Clip;
    }


    /*
        This method creates the balls bounciness and detects when goals are scored   
        
        */
    public void OnTriggerEnter(Collider impact)
    {
        //print("Collision");  // DEbugg
        //if (impact.transform.gameObject.name == Ball.name && this.picked == false)
        // if (impact.transform.gameObject.name == "RollerBall")

        Vector3 vel = GetComponent<Rigidbody>().velocity;
        
        GetComponent<Rigidbody>().velocity = vel;

        if (impact.tag == "VerticalWall")
        {
            //print("Impact in verticalWall"+ vel.y);
            vel.y = -vel.y;


            audioS.clip = SelectRandomClip();
            audioS.Play();
            //print(vel.y);

        }
        else if (impact.tag == "HorizontalWall")
        {
            audioS.clip = SelectRandomClip();
            audioS.Play();
            vel.x = -vel.x;
        }

        else if (impact.tag == "Rival")
        {
            audioS.clip = SelectRandomClip();
            audioS.Play();
            vel.z = -vel.z;
        }
        else if (impact.tag == "Goal")

        {
            int direction = 0;
            if (impact.name == "FrontWall")
            {
                direction = -1;
                gameController.score(1);
                audioS.clip = GoalClip;
                audioS.Play();
                // print("Colission detected with FrontWall");

            }

            else if (impact.name == "BackWall" && !WaitingForPlayerKick)
            {
                direction = 1;
                gameController.score(2);
                theArm.frameCount = 0;
                theArm.Opointer = new Vector3(0, 0, 0);
                // print("Colission detected with BackWall");
                audioS.clip = GoalClip;
                audioS.Play();
                theArm.deactivated = true;
                WaitingForPlayerKick = true;
                print("WaitingForPlayerKick:  "+ WaitingForPlayerKick);
            }

            vel.z = -vel.z;


            RespanwPoint = impact.transform.GetChild(0);
            respawn(RespanwPoint, direction);

           
            //gameObject.transform.position = RespanwPoint.transform.position;


            //hitByBall();
            //print("collision with powerup");
        }


        GetComponent<Rigidbody>().velocity= vel;
        hasCollided = true;
        

    }


    public IEnumerator Pause(float duration, int direction)
    {
        //This is a coroutine
        // Debug.Log("Start Wait() function. The time is: " + Time.time);
        
        yield return new WaitForSeconds(duration);

        isStopped = false;
        Vector3 velocityBall = gameController.StartKick(gameController.BallSpeed, direction);
        GetComponent<Rigidbody>().velocity = velocityBall;
    }


    /*
        After GOAL -> the ball will respawn in diferent locations
        */
    void respawn(Transform RessPoint, int direction)
    {

        
        isStopped = true;
        if(RessPoint.parent.name == "FrontWall")
        {
            gameObject.transform.position = RessPoint.position;
            StartCoroutine(Pause(2.0f, direction));
        }
            
        
        else if(RessPoint.parent.name == "BackWall")
        {
            WaitingForPlayerKick = true;
            theArm.deactivated = true;
            OriginalPositionHand = theArm.ObjHand.transform.position;
            gameObject.transform.position = theArm.ObjHand.transform.position;
        }
        //print("end of for 2s");   // Debbug

    }


    // Update is called once per frame
    void Update () {
        //gameObject.GetComponent<Rigidbody>().AddForce(transform.right * 3.0f);
        //gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 3.0f);
        //transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
        Vector3 ActualHandBallPosition;
        bool gestureMade = false;
        bool StaticForFrames = false;
        bool loop_done1 = false;
        float DistanceSwift = 1f;
        


        if (gameController.leftie)
        {
            theArm = KinectController.leftArm;
        }
        else
        {
            theArm = KinectController.rightArm;
        }



        if (isStopped)
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        if (gameController.body != "undef")
        {

            ActualHandBallPosition = theArm.ObjHand.transform.position;

            if (isStopped && WaitingForPlayerKick)
            {
               
                gameObject.transform.position = ActualHandBallPosition;


                if (!KinectController.checkRayOnPoint(0.5f, OriginalPositionHand, ActualHandBallPosition) && ActualHandBallPosition.z - OriginalPositionHand.z > 0.3f)
                {
                    
                    print("BIG MOVE DETECTED");

                    // if MOVE UP ->SHOOT THE BALL in the direction
                    //if (KinectController.MovingUp(KinectController.OriginalPositionRightHand, HandBallPosition))

                    print("Moving UP:  " + OriginalPositionHand + "...." + HandBallPosition);

                    Vector3 velocityBall = KinectController.JointRelativePosition(OriginalPositionHand, ActualHandBallPosition, false);
                    GetComponent<Rigidbody>().velocity = velocityBall * currentSpeed;

                    WaitingForPlayerKick = false;
                    isStopped = false;
                    theArm.deactivated = false;


                    

                }
                else
                {
                    OriginalPositionHand = ActualHandBallPosition;
                    WaitingForPlayerKick = true;
                    theArm.deactivated = true;
                }

            }


                /*

                //ActualHandBallPosition = theArm.ObjHand.transform.position;
                //print(ActualHandBallPosition); // DEBUG
                if (theArm.ObjHand != null)
                {
                    ActualHandBallPosition = theArm.ObjHand.transform.position;
                    //print("1: GOT HAND POSITION: " + ActualHandBallPosition + " \n");
                }

                else
                    ActualHandBallPosition = new Vector3(0,0,0);

                print("WaitingForPlayerKick:" + WaitingForPlayerKick);
            if (isStopped && WaitingForPlayerKick && ActualHandBallPosition != new Vector3(0, 0, 0))
            {



                    //KinectController.CheckStaticFrames(ref KinectController.rightArm);

                    //print("Ball Position updated to: " + ActualHandBallPosition); //Debugg
                    gameObject.transform.position = ActualHandBallPosition;

                    StaticForFrames = KinectController.CheckStaticFrames(ref theArm);
                    gestureMade = KinectController.checkRayOnPoint(3f, HandBallPosition, ActualHandBallPosition);
                    //KinectController.MovingUp(HandBallPosition, ActualHandBallPosition);







                    if(!StaticForFrames)
                    {
                        if (theArm.Opointer.x == 0 && theArm.Opointer.y == 0 && theArm.Opointer.z == 0)
                        {
                            print("2.1 Set up contruction point first time");
                            theArm.frameCount = 0;

                            theArm.rayColor = Color.white;
                            theArm.Opointer = theArm.pointer;
                            OriginalPositionHand = theArm.ObjHand.transform.position;
                            //Impact = theArm.ObjectHit;
                            loop_done1 = true;





                        }

                        //CASE 2: On range of the Opoint
                        else if (!loop_done1 && KinectController.checkRayOnPoint(2f, OriginalPositionHand, ActualHandBallPosition))
                        {
                            print("2.2 Counting frames");
                            theArm.frameCount++;
                            //print("dEBUGGING: " + rightArm.pointer);

                        }
                        //CASE 3: too far away -> reset
                        // Exception if we are LISTENING TO GESTURES
                        else if (!loop_done1 && !KinectController.checkRayOnPoint(DistanceSwift, OriginalPositionHand, ActualHandBallPosition) && !StaticForFrames)
                        {
                            print("2.3 Reseted point to...");
                            theArm.frameCount = 0;
                            //if (checkValidPointer(rightArm.pointer))

                            loop_done1 = true;
                            theArm.Opointer = new Vector3(0, 0, 0);
                            OriginalPositionHand = new Vector3(0, 0, 0);
                            StaticForFrames = false;

                        }

                    }








                    if (StaticForFrames)
                    {
                        //player kick    KinectController.MovingUp(HandBallPosition, ActualHandBallPosition)
                        Vector3 velocityBall = KinectController.JointRelativePosition(KinectController.OriginalPositionRightHand, ActualHandBallPosition, false);
                        //GetComponent<Rigidbody>().velocity = velocityBall;

                        print("3 frame test passed");


                        // If we move away we will either SHOOT the ball OR DISMISS the point
                        if (!KinectController.checkRayOnPoint(1f, OriginalPositionHand, ActualHandBallPosition))
                        {
                            print("4. BIG MOVE DETECTED");

                            // if MOVE UP ->SHOOT THE BALL in the direction
                            //if (KinectController.MovingUp(KinectController.OriginalPositionRightHand, HandBallPosition))

                                print("Moving UP:  "+ OriginalPositionHand + "...."+ HandBallPosition);


                                GetComponent<Rigidbody>().velocity = velocityBall * currentSpeed;
                                WaitingForPlayerKick = false;
                                isStopped = false;
                                theArm.deactivated = false;



                        }

                    }

                    else
                    {
                        HandBallPosition = ActualHandBallPosition;
                    }

                } 
            */



            }


        if (!isStopped)
        {
            Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;

          
            var newVelocity = currentVelocity.normalized * currentSpeed;
            GetComponent<Rigidbody>().velocity = Vector3.Lerp(currentVelocity, newVelocity, Time.deltaTime * gameController.BallSpeed);

        }
        

        //print(RespanwPoint.name);
        //print(RespanwPoint.transform.position);

    }
}
