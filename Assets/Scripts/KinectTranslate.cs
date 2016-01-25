using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class KinectTranslate : MonoBehaviour {

    Vector3 main_arm_dir;
    Vector3 alt_arm_dir;

    public GameObject BlockPrefab;

    bool leftie;
    bool runningPositionLocation1;
    bool runningPositionLocation2;
    bool handOnCooldown1;
    bool handOnCooldown2;




    GameObject GameControlObject;
    public GameController gameController;
    GameObject ObjectBall;
    public BallControll ballController;
    GameObject Player;
    Vector3 PlayerOriginalPosition, PlayerPosition;
    public Vector3 OriginalPositionRightHand , OriginalPositionLeftHand;



    GameObject body;
    GameObject MainArm , AltArm;
    GameObject LeftArm, RightArm;
    Transform JointLeftArm, JointRightArm;
    GameObject Impact;

    public struct arm
    {
        public int frameCount;
        public GameObject Prefab;
       // public Transform hand, elbow, shoulder;
        public GameObject ObjHand, ObjElvow, ObjShoulder;

        public bool handOnCooldown;
        public bool runningPositionLocation;
        public bool listen2gestures;
        public bool deactivated;

        public Vector3 pointer;
        public Vector3 Opointer;
        public GameObject ObjectHit;

        public bool mainArm;
        public Color rayColor;
    }



    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    

   public arm rightArm, leftArm;

    // Use this for initialization
    void Start () {

        GameControlObject = GameObject.Find("GameController");
        gameController = (GameController)GameControlObject.GetComponent(typeof(GameController));
        ObjectBall = GameObject.Find("ObjectBall");
        ballController = (BallControll)ObjectBall.GetComponent(typeof(BallControll));
        //BodySourceManager = GameObject.Find("BodyManager");
        //RayColor = Color.white;


        // Create struct right arm
        //arm rightArm = new arm();
        rightArm.frameCount = 0;
        rightArm.deactivated = false;
        //rightArm.Object = GameObject.Find("RightArm");
        rightArm.runningPositionLocation = false;
        rightArm.handOnCooldown = false;
        rightArm.pointer= new Vector3(0,0,0);
        rightArm.Opointer = new Vector3(0, 0, 0);
        rightArm.listen2gestures = false;
        rightArm.rayColor = Color.red;

        // Create struct left arm
        //arm leftArm = new arm();
        leftArm.frameCount = 0;
        leftArm.deactivated = false;
        //leftArm.Object = GameObject.Find("LeftArm");
        leftArm.runningPositionLocation = false;
        leftArm.handOnCooldown = false;
        leftArm.listen2gestures = false;
        leftArm.pointer = new Vector3(0, 0, 0);
        leftArm.Opointer = new Vector3(0, 0, 0);
        leftArm.rayColor = Color.red;

        // Get camera Position
        Player = GameObject.Find("PlayerRef");
        PlayerOriginalPosition = Player.transform.position;
        OriginalPositionRightHand = new Vector3(0, 0, 0);
        OriginalPositionLeftHand = new Vector3(0, 0, 0);
        //Camera.main.transform.position = PlayerOriginalPosition;


        //Camera.main.transform.position.x = Player.transform.position.x;



        if (gameController.leftie)
        {
            leftie = true;
            leftArm.mainArm = true;
            rightArm.mainArm = false;    
        }
        else
        {
            leftie=false;
            rightArm.mainArm = true;
            leftArm.mainArm = false;
        }

	}
    
	public Vector3 JointRelativePosition(Vector3 positionOrigin, Vector3 positionFin, bool invertZ)
    {
        Vector3 Vmovement = positionFin - positionOrigin;
        Vmovement = Vmovement.normalized;
        if (invertZ)
            Vmovement.z = -Vmovement.z;
        return Vmovement;
    }


    /*
        RefreshArms: Gets the position of the players arms and determines where are they pointing
        
        */

    void RefreshArms(bool invertZ)
    {

            float lenght = 1f;
            body = GameObject.Find(gameController.body);

            Transform RHand, LHand, RElbow, LElvow, RShoulder, LShoulder, HeadPosition;
            
            HeadPosition = body.transform.FindChild("Head");

            RHand = body.transform.FindChild("HandRight");
            //print("Rhand:" + RHand.position); // debugg info
            LHand = body.transform.FindChild("HandLeft");
            RElbow = body.transform.FindChild("ElbowRight");
            LElvow = body.transform.FindChild("ElbowLeft");
            RShoulder = body.transform.FindChild("ShoulderRight");
            LShoulder = body.transform.FindChild("ShoulderLeft");


        rightArm.ObjShoulder = GameObject.Find("RightShoulder");
        rightArm.ObjElvow = GameObject.Find("RightElvow");
        rightArm.ObjHand = GameObject.Find("RightHand");
        leftArm.ObjShoulder = GameObject.Find("LeftShoulder");
        leftArm.ObjElvow = GameObject.Find("LeftElvow");
        leftArm.ObjHand = GameObject.Find("LeftHand");

        /*  Camera and body movement  */
        PlayerPosition = HeadPosition.position;
        Vector3 newPosition = PlayerPosition;
        PlayerPosition.y = PlayerPosition.y+2.3f; // keep the z
        PlayerPosition.z = -PlayerPosition.z;
        if (PlayerPosition.z < 5.0f)
            PlayerPosition.z = 5.0f;
        else if (PlayerPosition.z > 7f)
            PlayerPosition.z = 7f;
        if (Vector3.Magnitude((PlayerOriginalPosition - PlayerPosition)) > 0.01)
            Player.transform.position = PlayerPosition;

        PlayerOriginalPosition = PlayerPosition;

        //Shoulders are usually fixed  GOT from the position of the gameobjects
        rightArm.ObjShoulder.transform.position = rightArm.ObjShoulder.transform.position;
        leftArm.ObjShoulder.transform.position = leftArm.ObjShoulder.transform.position;


            //Right
            //Elbows determined by 
            Vector3 VM = JointRelativePosition(RShoulder.position, RElbow.position, invertZ);
        //print("VM: " + VM);
        rightArm.ObjElvow.transform.position = rightArm.ObjShoulder.transform.position + VM * lenght;
        //Hand determined (After elbow)
        VM = JointRelativePosition(RElbow.position, RHand.position, invertZ);
        rightArm.ObjHand.transform.position = rightArm.ObjElvow.transform.position + VM * lenght;

            //left
            //Elbows determined by 
            VM = JointRelativePosition(LShoulder.position, LElvow.position, invertZ);
            leftArm.ObjElvow.transform.position = leftArm.ObjShoulder.transform.position + VM * lenght;
            //Hand determined (After elbow)
            VM = JointRelativePosition(LElvow.position, LHand.position, invertZ);
            leftArm.ObjHand.transform.position = leftArm.ObjElvow.transform.position + VM * lenght;

        /*
        //lines
        */
        Color RayColor = Color.green;
        Debug.DrawLine(rightArm.ObjElvow.transform.position, rightArm.ObjHand.transform.position, RayColor);
        Debug.DrawLine(leftArm.ObjElvow.transform.position, leftArm.ObjHand.transform.position, RayColor);

        Debug.DrawLine(rightArm.ObjElvow.transform.position, rightArm.ObjShoulder.transform.position, RayColor);
        Debug.DrawLine(leftArm.ObjElvow.transform.position, leftArm.ObjShoulder.transform.position, RayColor);

    }



    /*
        PlayerHorizontalMove: player can shifts his position
        */

    void PlayerHorizontalMove(Vector3 playerPosition, float step)
    {
        float Th = 3.3f;
        Vector3 newPositionVector = Camera.main.transform.position;
        if (playerPosition.x < -Th)
            playerPosition.x = -Th;
        else if (playerPosition.x > Th)
            playerPosition.x = Th;
        newPositionVector.x = playerPosition.x;
        //ObjectAI.transform.position = Vector3.MoveTowards(ObjectAI.transform.position, newPositionVector, step);
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPositionVector, step);
    }


    /*
        Get and determine main arm direction
        */
    Vector3[] getMainArm(bool leftie)
    {
        Vector3[] ArmsVector = new Vector3[2];
        bool error = true;
        // Case LEFT handed
        if (leftie) {

        }
        // Case RIGHT handed
        else
        {

        }

        // Unknown errors
        if (error)
        {
            ArmsVector[0] = new Vector3(0, 0, 0);
            ArmsVector[1] = new Vector3(0, 0, 0);
            return ArmsVector;
        }

        else
        {
            return ArmsVector;
        }
    }




    /*
        Standard method for waiting X seconds
        
        */
    public IEnumerator Wait(float waitTime)
    {
        print("Waiting for" + waitTime + "seconds");
        yield return new WaitForSeconds(waitTime);
        print("End Waiting for" + waitTime + "seconds");
        
        
    }


    public IEnumerator WaitNDestroy(float waitTime, arm TheArm)
    {

        TheArm.handOnCooldown = true;
        print("Start Waiting for" + waitTime + "seconds");
        yield return new WaitForSeconds(waitTime);
        print("End Waiting for" + waitTime + "seconds");
        Destroy(TheArm.Prefab);
        TheArm.handOnCooldown = false;


    }


    /*
        MovingLeft: True if the pointer has shift from Right to Left
        */

    public bool MovingLeft(Vector3 Origin, Vector3 Point)
    {

        Vector3 move = JointRelativePosition(Origin, Point, false);
        if (move.x<0)
            return true;
        else
            return false;
    }

    /*
        MovingRight: True if the pointer has shift from Left to Right
    */
    public bool MovingRight(Vector3 Origin, Vector3 Point)
    {

        Vector3 move = JointRelativePosition(Origin, Point, false);
        if (move.x > 0)
            return true;
        else
            return false;
    }

    /*
       MovingUp: True if the pointer has shift from Down to Upward
   */
    public bool MovingUp(Vector3 Origin, Vector3 Point)
    {

        Vector3 move = JointRelativePosition(Origin, Point, false);
        if (move.y > 0 )
            return true;
        else
            return false;
    }



    /*
        This will run in the background


        If succedded it will start a cooldown
        */
    public IEnumerator FixedPointer(arm TheArm)
    {
        TheArm.runningPositionLocation = true;
        float smallDistance = 2f;
        float LargeDistance = 4f;
        Vector3 OPoint = TheArm.pointer;
        Vector3 thePoint;
        bool isDone = false;
        bool check = false;

        print("First Position of " + TheArm.ObjHand.name + " is" + OPoint);


        //StartCoroutine(Wait(3f, TheArm));
        print("Waiting for" + 3f + "seconds....... TIME ="+ Time.time);
        yield return new WaitForSeconds(3f);
        print("End Waiting for" + 3f + "seconds....... TIME =" + Time.time);

        // get the point
        
        thePoint = TheArm.pointer;
        print("Second Position of " + TheArm.ObjHand.name + " is" + thePoint);
        check = checkRayOnPoint(smallDistance, thePoint, OPoint);
        if (check)
        {
            print(smallDistance + "distance is "+ check);
            TheArm.rayColor = Color.blue;
            float distance = Vector3.Distance(thePoint, OPoint);
            /*
            StartCoroutine(Wait(0.3f));
            // get the point
            print("runing second part of the courutine");
            thePoint = TheArm.pointer;

            if (checkRayOnPoint(smallDistance, thePoint, OPoint))
            {
                print(smallDistance + "distance");
                RayColor = Color.blue;
                distance = Vector3.Distance(thePoint, OPoint);
            }

            */

        }
        else
        {
            
            print("Aborting wait, at time = " + Time.time);
            TheArm.rayColor = Color.white;
            TheArm.runningPositionLocation = false;

        }


        
        


        /*
        UpdateRayPointer(TheArm);
        thePoint = TheArm.pointer;
        check = checkRayOnPoint(smallDistance, OPoint, thePoint);
        if (check)
        {

        }
        else
            isDone = true;

        */
        TheArm.runningPositionLocation = true;
        // Get where the raycast hits thePoint = 

        

        //This is a coroutine
        //isStopped = false;
        //Vector3 velocityBall = gameController.StartKick(gameController.BallSpeed, direction);
        //GetComponent<Rigidbody>().velocity = velocityBall;
    }



    /*
        UpdateRayPointer. Find if we should cast a ray
    */

    Vector3 UpdateRayPointer(ref arm theArm)
    {

        Vector3 RayColission = new Vector3(0,0,0);
        
        RaycastHit hit;
        theArm.pointer = RayColission;
        //Color rayColor = RayColor;


        Vector3 Ref1 = theArm.ObjHand.transform.position;
        Vector3 Ref2 = theArm.ObjElvow.transform.position;
        Vector3 Ref3 = theArm.ObjShoulder.transform.position;

        Vector3 Direction = JointRelativePosition(Ref2, Ref1, false);

        Ray theRay = new Ray(Ref1, Direction);
        Physics.Raycast(theRay, out hit, 100);
        theArm.ObjectHit = hit.transform.gameObject;
        theArm.pointer = hit.point;
        Debug.DrawLine(Ref1, hit.point, theArm.rayColor);

        return hit.point;
    }




   public bool checkValidPointer(Vector3 thePoint)
    {
        float ConstrainX;
        float ConstrainY;
        float ConstrainZMin = 8.4f;
        float ConstrainZMax = 21;

        //print(thePoint.z+ "...... is the Z value" );  // Debug
        if (ConstrainZMin < thePoint.z && thePoint.z < ConstrainZMax)
        { 
            return true;
        }
            
        
        else
        {
            return false;
        }
    }


    /*
        Distance2Point calculates the distance between 2 points
        */
    public bool checkRayOnPoint (float maxDistance, Vector3 reference, Vector3 thePoint)
    {
        float distance = Vector3.Distance(reference, thePoint);
       // print("distance from" + reference + " and "+ thePoint  + " is = "+ distance);
            if (distance < maxDistance)
            {
                return true;
            }
            else
                return false;
        }


    /*
        DismissAction
        */

    void DismissAction (arm theArm)
    {

    }

    /*
        checkIfActive: the input arm is occupied        
        */
    void checkIfActive(ref arm theArm)
    {
        if (!theArm.runningPositionLocation)
        {
            print(theArm.runningPositionLocation);
            print("Starting Courutine with: "+ theArm.ObjHand.name+ "\n");
            StartCoroutine( FixedPointer(theArm));
        }
        else
        {
            print("A courutine was running already for: " + theArm.ObjHand.name + "So ignore\n");
        }
    }




    /*
        ExpandObject

        */


    void ExpandObject(arm theArm, Vector3 direction)
    {
        float scalingDuration = 0.3f;
        Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + direction.x * 5, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
        float scaleStep = Time.deltaTime / scalingDuration;
        rightArm.Prefab.transform.localScale = Vector3.Lerp(transform.localScale, newScale, scaleStep);

    }




    public bool CheckStaticFrames (ref arm theArm)
    {
        int frames_cap1 = 15;
        int frames_cap2 = 30;
        int frames_cap3 = 45;
        //Depending on the number of frames
        //A 0-15 change color to white 


        if(theArm.handOnCooldown){

            theArm.rayColor = Color.yellow;
            theArm.frameCount = 0;
        }
        else if (theArm.deactivated){
            theArm.rayColor = Color.yellow;
            theArm.frameCount = 0;

        }


        else
        {

            if (theArm.frameCount < frames_cap1)
            {
                theArm.rayColor = Color.white;
                return false;
            }

            //B 15-30 change color to blue
            else if (theArm.frameCount > frames_cap1 && theArm.frameCount < frames_cap2)
            {
                theArm.rayColor = Color.blue;

            }


            //C +30 change color to red and ready to deploy
            else if (theArm.frameCount > frames_cap2)
            {
                theArm.rayColor = Color.green;
                return true;      // IF we reach this point there is no need to keep checking this
            }

        }

        

        return false;
    }









    // Update is called once per frame
    void Update () {

        bool loop_done1= false; //RightArm
        bool loop_done2 = false; //LeftArm



        bool invertZ = true;
        int frames_analyzed;
        int frames_cap1 = 15;
        int frames_cap2 = 30;
        int frames_cap3 = 45;
        float DistanceSwift = 1f;






        // STEP 1: at least a body is detected
        if (gameController.body != "undef")
        {
            print(gameController.body);
            RefreshArms(invertZ);   // Refresh arm position


            // Get collision points
            leftArm.pointer = UpdateRayPointer(ref leftArm);
            rightArm.pointer = UpdateRayPointer(ref rightArm);





            /***************** RIGHT ARM **************/

            if (!rightArm.deactivated && !rightArm.handOnCooldown)
            { 

            //RIGHT HAND
            // CASE 1: First time
            if (rightArm.Opointer.x == 0 && rightArm.Opointer.y == 0 && rightArm.Opointer.z == 0)
            {
                rightArm.frameCount = 0;

                //print("RestartingCAPTION.... VALID ZONE:" +checkValidPointer(rightArm.pointer));  // Debugg
                if (!checkValidPointer(rightArm.pointer))
                    rightArm.rayColor = Color.red;
                else
                {
                    //rightArm.rayColor = Color.white;
                    rightArm.Opointer = rightArm.pointer;
                    OriginalPositionRightHand = rightArm.ObjHand.transform.position;
                    Impact = rightArm.ObjectHit;
                }


                loop_done1 = true;


            }

            //CASE 2: On range of the Opoint
            else if (!loop_done1 && checkRayOnPoint(DistanceSwift, rightArm.Opointer, rightArm.pointer))
            {
                rightArm.frameCount++;
                //print("dEBUGGING: " + rightArm.pointer);

            }
            //CASE 3: too far away -> reset
            // Exception if we are LISTENING TO GESTURES
            else if (!loop_done1 && !checkRayOnPoint(DistanceSwift, rightArm.Opointer, rightArm.pointer) && !rightArm.listen2gestures)
            {
                rightArm.frameCount = 0;
                //if (checkValidPointer(rightArm.pointer))

                loop_done1 = true;
                rightArm.Opointer = new Vector3(0, 0, 0);
                OriginalPositionRightHand = new Vector3(0, 0, 0);
                //rightArm.listen2gestures = false;

            }


            // DO only if it has been approved and STOP if we reach the GREEN state
            if (!loop_done1 && !rightArm.listen2gestures)
            {
                rightArm.listen2gestures = CheckStaticFrames(ref rightArm);

            }




            // if we have determined the point WE GET X FRAMES
            else if (rightArm.listen2gestures == true)
            {

                if (rightArm.Prefab == null)
                {
                    if (rightArm.ObjectHit.tag == "VerticalWall")
                    {
                        rightArm.Prefab = (GameObject)Instantiate(BlockPrefab, rightArm.Opointer, Quaternion.Euler(0, 180, 90));
                            
                        }
                    else if (rightArm.ObjectHit.tag == "HorizontalWall")
                        {
                            rightArm.Prefab = (GameObject)Instantiate(BlockPrefab, rightArm.Opointer, Quaternion.Euler(0, 0, 0));
                            
                        }
                       StartCoroutine( WaitNDestroy(4f, rightArm));
                    }






                //Destroy(rightArm.Prefab);
                //


                if (!checkRayOnPoint(DistanceSwift, rightArm.Opointer, rightArm.pointer) && rightArm.handOnCooldown == false) // if it has moved far enough we Star the process
                {

                    print("separated a distance");
                    //GameObject Impact =rightArm.ObjectHit;
                    Vector3 direction = JointRelativePosition(OriginalPositionRightHand, rightArm.ObjHand.transform.position, false);

                    //CASE 1: Impact with top
                    if (Impact.name == "top")
                    {

                      //  print("impacted on top");
                        if (!(rightArm.ObjHand.transform.position.y > OriginalPositionRightHand.y)) // not moving up
                        {
                            print("is moving down " + JointRelativePosition(OriginalPositionRightHand, rightArm.ObjHand.transform.position, false));

                            float scalingDuration = 0.3f;
                            Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + 6, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
                            //float scaleStep = Time.deltaTime / scalingDuration;
                            rightArm.Prefab.transform.localScale = newScale;

                            WaitNDestroy(2f, rightArm);
                            //Destroy(rightArm.Prefab);


                        }
                        else
                        {
                          //  print("Destroyed on TOP ( moving up)");
                            Destroy(rightArm.Prefab);
                            rightArm.listen2gestures = false;
                        }

                        rightArm.listen2gestures = false;
                        rightArm.Opointer = new Vector3(0, 0, 0);
                        loop_done1 = false;

                    }



                        //CASE 2: Impact with right
                        if (Impact.name == "right")
                        {

                           // print("impacted on right");
                            if (!(rightArm.ObjHand.transform.position.x > OriginalPositionRightHand.x)) // not moving up
                            {
                                print("is moving right " + JointRelativePosition(OriginalPositionRightHand, rightArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + 6, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                rightArm.Prefab.transform.localScale = newScale;

                                WaitNDestroy(2f, rightArm);
                                //Destroy(rightArm.Prefab);


                            }
                            else
                            {
                                print("Destroyed on right ( moving right)");
                                Destroy(rightArm.Prefab);
                                rightArm.listen2gestures = false;
                            }

                            rightArm.listen2gestures = false;
                            rightArm.Opointer = new Vector3(0, 0, 0);
                            loop_done1 = false;

                        }


                        //CASE 3: Impact with left
                        if (Impact.name == "left")
                        {

                           // print("impacted on left");
                            if (!(rightArm.ObjHand.transform.position.x < OriginalPositionRightHand.x)) // not moving up
                            {
                                //print("is moving left " + JointRelativePosition(OriginalPositionRightHand, rightArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + 6, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                rightArm.Prefab.transform.localScale = newScale;

                                WaitNDestroy(2f, rightArm);
                                //Destroy(rightArm.Prefab);


                            }
                            else
                            {
                               // print("Destroyed on left ( moving left)");
                                Destroy(rightArm.Prefab);
                                rightArm.listen2gestures = false;
                            }

                            rightArm.listen2gestures = false;
                            rightArm.Opointer = new Vector3(0, 0, 0);
                            loop_done1 = false;
                            

                        }


                        //CASE 4: Impact with bottom
                        if (Impact.name == "bottom")
                        {

                            if (!(rightArm.ObjHand.transform.position.y < OriginalPositionRightHand.y)) // not moving up
                            {
                                //print("is moving bottom " + JointRelativePosition(OriginalPositionRightHand, rightArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + 6, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                rightArm.Prefab.transform.localScale = newScale;

                                WaitNDestroy(2f, rightArm);
                                //Destroy(rightArm.Prefab);


                            }
                            else
                            {
                               // print("Destroyed on bottom ( moving bottom)");
                                Destroy(rightArm.Prefab);
                                rightArm.listen2gestures = false;
                            }

                            rightArm.listen2gestures = false;
                            
                            loop_done1 = false;

                        }



                        /*
                        if ((rightArm.frameCount == 0) || (rightArm.frameCount == 2) || (rightArm.frameCount = 4))
                        {

                        }

                        Vector3 direction = JointRelativePosition(rightArm.Opointer, rightArm.pointer, false);
                        float scalingDuration = 0.3f;
                        Vector3 newScale = new Vector3(rightArm.Prefab.transform.localScale.x + direction.x*5, rightArm.Prefab.transform.localScale.y, rightArm.Prefab.transform.localScale.z);
                        float scaleStep = Time.deltaTime / scalingDuration;
                        rightArm.Prefab.transform.localScale = Vector3.Lerp(transform.localScale, newScale, scaleStep);

                        rightArm.listen2gestures = false;
                        StartCoroutine(Wait(3f));
                        */

                        rightArm.listen2gestures = false;
                        loop_done1 = false;
                        rightArm.Opointer = rightArm.pointer;
                        rightArm.frameCount = 0;
                        rightArm.pointer = new Vector3(0, 0, 0);

                    }
                }
            }
            /***************** END RIGHT ARM **************/



            /***************** LEFT ARM **************/

            if (!leftArm.deactivated && !leftArm.handOnCooldown)
            {

                // HAND
                // CASE 1: First time
                if (leftArm.Opointer.x == 0 && leftArm.Opointer.y == 0 && leftArm.Opointer.z == 0)
                {
                    leftArm.frameCount = 0;

                    //print("RestartingCAPTION.... VALID ZONE:" +checkValidPointer(leftArm.pointer));  // Debugg
                    if (!checkValidPointer(leftArm.pointer))
                        leftArm.rayColor = Color.red;
                    else
                    {
                        //leftArm.rayColor = Color.white;
                        leftArm.Opointer = leftArm.pointer;
                        OriginalPositionLeftHand = leftArm.ObjHand.transform.position;
                        Impact = leftArm.ObjectHit;
                    }


                    loop_done2 = true;


                }

                //CASE 2: On range of the Opoint
                else if (!loop_done2 && checkRayOnPoint(DistanceSwift, leftArm.Opointer, leftArm.pointer))
                {
                    leftArm.frameCount++;
                    //print("dEBUGGING: " + leftArm.pointer);

                }
                //CASE 3: too far away -> reset
                // Exception if we are LISTENING TO GESTURES
                else if (!loop_done2 && !checkRayOnPoint(DistanceSwift, leftArm.Opointer, leftArm.pointer) && !rightArm.listen2gestures)
                {
                    leftArm.frameCount = 0;
                    //if (checkValidPointer(rightArm.pointer))

                    loop_done2 = true;
                    leftArm.Opointer = new Vector3(0, 0, 0);
                    
                    OriginalPositionLeftHand = new Vector3(0, 0, 0);
                    //rightArm.listen2gestures = false;

                }


                // DO only if it has been approved and STOP if we reach the GREEN state
                if (!loop_done2 && !leftArm.listen2gestures)
                {
                    leftArm.listen2gestures = CheckStaticFrames(ref leftArm);

                }




                // if we have determined the point WE GET X FRAMES
                else if (leftArm.listen2gestures == true)
                {

                    if (leftArm.Prefab == null)
                    {
                        if (leftArm.ObjectHit.tag == "VerticalWall")
                        {
                            leftArm.Prefab = (GameObject)Instantiate(BlockPrefab, leftArm.Opointer, Quaternion.Euler(0, 180, 90));
                        }
                        else if (leftArm.ObjectHit.tag == "HorizontalWall")
                            leftArm.Prefab = (GameObject)Instantiate(BlockPrefab, leftArm.Opointer, Quaternion.Euler(0, 0, 0));

                        StartCoroutine(WaitNDestroy(4f, leftArm));
                    }






                    //Destroy(rightArm.Prefab);
                    //


                    if (!checkRayOnPoint(DistanceSwift, leftArm.Opointer, leftArm.pointer) && leftArm.handOnCooldown == false) // if it has moved far enough we Star the process
                    {

                        print("separated a distance");
                        //GameObject Impact =rightArm.ObjectHit;
                        Vector3 direction = JointRelativePosition(OriginalPositionLeftHand, leftArm.ObjHand.transform.position, false);

                        //CASE 1: Impact with top
                        if (Impact.name == "top")
                        {

                            print("impacted on top");
                            if (!(leftArm.ObjHand.transform.position.y > OriginalPositionLeftHand.y)) // not moving up
                            {
                                print("is moving down " + JointRelativePosition(OriginalPositionLeftHand, leftArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(leftArm.Prefab.transform.localScale.x + 6, leftArm.Prefab.transform.localScale.y, leftArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                leftArm.Prefab.transform.localScale = newScale;

                                WaitNDestroy(1f, leftArm);
                                //Destroy(rightArm.Prefab);


                            }
                            else
                            {
                                print("Destroyed on TOP ( moving up)");
                                Destroy(leftArm.Prefab);
                                leftArm.listen2gestures = false;
                            }

                            leftArm.listen2gestures = false;
                            leftArm.Opointer = new Vector3(0, 0, 0);
                            loop_done2 = false;

                        }



                        //CASE 2: Impact with right
                        if (Impact.name == "right")
                        {

                            //print("impacted on right");
                            if (!(leftArm.ObjHand.transform.position.x > OriginalPositionLeftHand.x)) // not moving up
                            {
                                print("is moving right " + JointRelativePosition(OriginalPositionLeftHand, leftArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(leftArm.Prefab.transform.localScale.x + 6, leftArm.Prefab.transform.localScale.y, leftArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                leftArm.Prefab.transform.localScale = newScale;

                                WaitNDestroy(1f, leftArm);
                                //Destroy(leftArm.Prefab);


                            }
                            else
                            {
                                print("Destroyed on right ( moving right)");
                                Destroy(leftArm.Prefab);
                                leftArm.listen2gestures = false;
                            }

                            leftArm.listen2gestures = false;
                            leftArm.Opointer = new Vector3(0, 0, 0);
                            loop_done2 = false;

                        }


                        //CASE 3: Impact with left
                        if (Impact.name == "left")
                        {

                           // print("impacted on left");
                            if (!(leftArm.ObjHand.transform.position.x < OriginalPositionLeftHand.x)) // not moving up
                            {
                               // print("is moving left " + JointRelativePosition(OriginalPositionLeftHand, leftArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(leftArm.Prefab.transform.localScale.x + 6, leftArm.Prefab.transform.localScale.y, leftArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                leftArm.Prefab.transform.localScale = newScale;

                                
                                //Destroy(leftArm.Prefab);


                            }
                            else
                            {
                                //print("Destroyed on left ( moving left)");
                                Destroy(leftArm.Prefab);
                                leftArm.listen2gestures = false;
                            }

                            leftArm.listen2gestures = false;
                            leftArm.Opointer = new Vector3(0, 0, 0);
                            loop_done2 = false;

                        }


                        //CASE 4: Impact with bottom
                        if (Impact.name == "bottom")
                        {

                            if (!(leftArm.ObjHand.transform.position.y < OriginalPositionLeftHand.y)) // not moving up
                            {
                                print("is moving bottom " + JointRelativePosition(OriginalPositionLeftHand, leftArm.ObjHand.transform.position, false));

                                float scalingDuration = 0.3f;
                                Vector3 newScale = new Vector3(leftArm.Prefab.transform.localScale.x + 6, leftArm.Prefab.transform.localScale.y, leftArm.Prefab.transform.localScale.z);
                                //float scaleStep = Time.deltaTime / scalingDuration;
                                leftArm.Prefab.transform.localScale = newScale;

                                
                                //Destroy(leftArm.Prefab);


                            }
                            else
                            {
                                print("Destroyed on bottom ( moving bottom)");
                                Destroy(leftArm.Prefab);
                                leftArm.listen2gestures = false;
                            }

                            

                        }

                        leftArm.listen2gestures = false;
                        loop_done2 = false;
                        leftArm.Opointer = rightArm.pointer;
                        leftArm.frameCount = 0;
                        leftArm.pointer = new Vector3(0, 0, 0);
                    }
                }
            }
            /***************** END LEFT ARM **************/






        }



    }







}
