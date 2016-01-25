using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

    GameObject GameControlObject;
    public float MoveSpeed;
    public GameController gameController;
    public BallControll ballController;
    GameObject UInterface;
    GameObject ObjectBall;


    // Use this for initialization
    void Start() {
        MoveSpeed = 2f;
        ObjectBall = GameObject.Find("ObjectBall");
        ballController = (BallControll)ObjectBall.GetComponent(typeof(BallControll));
        GameControlObject = GameObject.Find("GameController");
        gameController = (GameController)GameControlObject.GetComponent(typeof(GameController));

        // Get the position of the respawning ball

    }


    public void moveTo(Vector3 point)
    {
        Vector3 newPoint = point;
        newPoint.x = point.x;
        newPoint.y = point.y;
        newPoint.z = gameObject.transform.position.z;
        gameObject.transform.position = newPoint;
    }


    void StartKick()
    {


    }


    void MoveRival(){


    }





    // Update is called once per frame
    void Update () {
        
            
    }
}
