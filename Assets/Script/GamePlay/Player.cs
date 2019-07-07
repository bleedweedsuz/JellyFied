using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    #region "enum Decleration"
    public enum Direction { Left, Right }
    public enum ControllerMode { TypeA,TypeB,TypeC,TypeD}
    #endregion
    #region "Public Variables"
    public float moveSpeed;
    public float jumpValue;
    public LayerMask groundMask;
    public Direction playerDirection = Direction.Right;
    public ControllerMode controllerMode = ControllerMode.TypeA;
    public float swipeMinDistanceValue = 3f;
    public bool isGamepause = true;
    public GameObject swipePrefabDisplayObject;
    public bool isSwipeDisplayOn = true;
    #endregion
    #region "Private Variables"
    GameObject myPlayer;
    Camera mainCamera;
    bool isFalling = false;
    Rigidbody playerRigidBody;
    Vector3 startPos;
    #endregion
    #region "Default Methods"
    void Start () {
        myPlayer = gameObject;
        mainCamera = Camera.main;
        playerRigidBody = myPlayer.GetComponent<Rigidbody>();
	}
	void Update () {
        cameraMovement();
        if (!isGamepause)
        {
            //Change Direction
            inputController();
            //Make Updates in the Character
            controlPlayer();
        }
        //Game While Pausing
        else
        {
            //Input Controller
            inputControllerWhenPause();
            //Some Control
        }
	}
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)//Collision with ground => 8
        {
            isFalling = false;
        }
    }
    #endregion
    #region "Custom Build Methods"
    void cameraMovement()
    {
        //Change only the y Value in the camera
        Vector3 cameraPos = mainCamera.transform.localPosition;
        cameraPos.y = transform.localPosition.y - 2;
        mainCamera.transform.position = Vector3.Lerp(cameraPos, transform.localPosition, 0.1f) + new Vector3(0, 0, -2f);
    }
    //--------------------------------------
    void inputController()
    {
        #if UNITY_EDITOR
            //Unity Editor
            if (Input.GetAxis("Horizontal") > 0) { if (playerDirection != Direction.Right) { playerDirection = Direction.Right; } }
            else if (Input.GetAxis("Horizontal") < 0) { if (playerDirection != Direction.Left) { playerDirection = Direction.Left; } }
            if (Input.GetButtonDown("Jump") && isFalling == false)
            {
                isFalling = true;
                Vector3 pos = playerRigidBody.velocity;
                pos.y = jumpValue;
                playerRigidBody.velocity = pos;
            }
        #endif
        #if UNITY_ANDROID || UNITY_IPHONE
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                //Tap Begin
                                startPos = touch.position;
                            }break;
                        case TouchPhase.Ended:
                            {
                                float swipeHorizontal_distanceValue = (new Vector3(0,touch.position.y,0) - new Vector3(0,startPos.y,0)).magnitude;
                                if (swipeHorizontal_distanceValue > swipeMinDistanceValue)
                                {
                                    float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                                    if (swipeValue > 0)
                                    {
                                        playerDirection = Direction.Right;
                                        swipeDisplay(touch.position);
                                    }
                                    else if (swipeValue < 0)
                                    {
                                        playerDirection = Direction.Left;
                                        swipeDisplay(touch.position);
                                    }
                                }
                                else
                                {
                                    //This Means Taps
                                    //Jump Mode
                                    if (isFalling == false)
                                    {
                                        isFalling = true;
                                        Vector3 pos = playerRigidBody.velocity;
                            
                                        pos.y = jumpValue;
                                        playerRigidBody.velocity = pos;
                                    }
                                }
                            }break;
                        //fuck the others
                        default: break;
                    }
                }
            }
        #endif
    }
    void inputControllerWhenPause()
    {
        #if UNITY_EDITOR
            //Unity Editor
            if (Input.GetButtonDown("Jump"))
            {
                isGamepause = false;
            }
        #endif
        #if UNITY_ANDROID || UNITY_IPHONE
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                isGamepause = false;
                            }break;
                        default: break;
                    }
                }
            }
        #endif
    }
    //--------------------------------------
    void controlPlayer()
    {
        //Controlling Player
        switch (playerDirection)
        {
            case Direction.Left:
                {
                    //Go Left Go Left
                    Vector3 pos = playerRigidBody.velocity;
                    pos.x = -moveSpeed;
                    playerRigidBody.velocity = pos;
                } break;
            case Direction.Right:
                {
                    //Go Right Go Right
                    Vector3 pos = playerRigidBody.velocity;
                    pos.x = moveSpeed;
                    playerRigidBody.velocity = pos;
                } break;
            default: break;
        }
    }
    void swipeDisplay(Vector3 swipePos)
    {
        if (swipePrefabDisplayObject != null && isSwipeDisplayOn == true)
        {
            swipePrefabDisplayObject.transform.position = swipePos;
        }
    }
    #endregion
}
