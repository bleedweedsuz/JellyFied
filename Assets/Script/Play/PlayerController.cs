using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Controller2D))]
public class PlayerController : MonoBehaviour {
    //----------------AUDIO SOURCE------------------//
    //----------------------------------------------//
    
    //----------------------------------------------//
    //----------------------------------------------//
	public int collector = 0;
	Vector3 originalPosition;
	public bool isGameStart=false;
	//For Moving Player
	public static float moveSpeed = 5;
	float velocityXSmoothing;
	float accelerationTimeAirborne=0.2f,accelerationTimeGrounded=0.1f;
	//For Jumping Smoothly
	public float jumpHeight = 3.5f;
	public float timeToJumpApex = .4f;
	float gravity;
	float jumpVelocity = 8f;
	//Current Velocity of Player
	Vector3 velocity;
	//Class used for to control the player Class
	Controller2D controller;
	public float wallSlideMax;   
	public float wallStickTime = .2f;
	float timeToWallunStick;
	public Vector2 wallJumpClimb,wallJumpOff,wallLeap;
	//---------
	[Range(-1,1)]
	public int playerDirection = -1;//Left Position
	public static bool collideWall = false;
	public static bool isPlayerDead =false;
	public GameObject EventSystem;
	public GameObject MainHud;
	public GameObject TopHud;
	HudEvent hudEvent;
	public enum PlayerAction{
		Running,Idle,WallCollide,Jumping,WallSlide,Dead
	}
	public PlayerAction playerAction = PlayerAction.Idle;
	public PlayerAction ctemp_playerAction = PlayerAction.Idle;
	public PlayerAction oplayerAction = PlayerAction.WallCollide ;
	//Game Dialog Boxes
	public GameObject TapToPlay;
	float scaleData;
	public GameObject playerGameObjectHolder;
	public ParticleSystem playerParticleSystem;
	public ParticleSystem playerDeadParticleSystem;
	Animator playerAnimator;
	float TimeToWaitWhileDead =1.0f;
	float TimeAddedWhileDead =0;
	public GameObject ScoreBoard;
    public AudioSource mainBackAudioSource;
    public Sprite Vol_ON, Vol_OFF;
    public Image volBackImage;
	void Start () {
		isPlayerDead = false;
		originalPosition = gameObject.transform.localPosition;
		scaleData = transform.localScale.x;
		controller = GetComponent<Controller2D> ();
		playerAnimator = playerGameObjectHolder.gameObject.GetComponent<Animator> ();
		kinematicEquationForPlayer ();
		hudEvent = EventSystem.GetComponent<HudEvent> ();
        PlayBackAudioSource();
        if (GlobalVariables.ISMainBackAudio)
        {
            volBackImage.sprite = Vol_ON;
        }
        else
        {
            volBackImage.sprite = Vol_OFF;
        }
	}
	void Update () {
		if(!isGameStart){
			if (HudEvent.isGamepause) {
				TapToPlay.SetActive(false);
				return;
			}
			else{
				TapToPlay.SetActive(true);
			}
			#if UNITY_EDITOR
			if (Input.GetKeyDown (KeyCode.Space)) {
				isGameStart =true;
				TapToPlay.SetActive(false);
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

					}break;
					case TouchPhase.Ended:
					{
						isGameStart =true;
						TapToPlay.SetActive(false);
						
					}break;
					default: break;
					}
				}
			}
			#endif
			return;
		}
		//Game pause---------------------------------------------------->
		if (HudEvent.isGamepause) {
			playerAction = PlayerAction.Idle;
			playerAnimator.SetInteger ("State", 0);
			playerParticleSystem.Stop();
			playerDeadParticleSystem.Stop();
			return;
		}
		//--------------------------------------------------------------->
		//--------------------------->
		//-----------------Check for collision detection
		if (playerAction == PlayerAction.Dead) {
			//------->
			if(playerDeadParticleSystem.isStopped){
				playerDeadParticleSystem.Play();
				playerGameObjectHolder.SetActive(false);
				//Clear Particle
				playerParticleSystem.Stop();
				playerParticleSystem.Clear();
			}
			//-------->Update Time
			TimeAddedWhileDead +=Time.deltaTime;
			if(TimeAddedWhileDead > TimeToWaitWhileDead){
				TimeAddedWhileDead =0;
				playerDeadParticleSystem.Stop();
				playerDeadParticleSystem.Clear();
				playerAction = PlayerAction.Idle;
				isGameStart =false;
				TapToPlay.SetActive(true);
				transform.localPosition = originalPosition;
				playerDirection =-1;
				playerGameObjectHolder.SetActive(true);
				playerParticleSystem.gameObject.SetActive(false);
				velocity = Vector3.zero;

				//------------------->
				hudEvent.Hud_SetTopHudGameObject(TopHud);
				hudEvent.Hud_TopHudToggle(MainHud);
			}
			return;
		}
		//---------------------------->
		Vector2 input = new Vector2 (playerDirection, 0);
		int wallDirx = (controller.collisionInfo.left) ? -1 : 1;
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing,(controller.collisionInfo.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		bool wallSliding = false;
		if((controller.collisionInfo.left==true || controller.collisionInfo.right ==true) && !controller.collisionInfo.below == true && velocity.y <0){
			wallSliding =true;
			if(velocity.y< - wallSlideMax){
				velocity.y = -wallSlideMax;
			}
			if(timeToWallunStick >0){
				velocityXSmoothing =0;
				velocity.x = 0;
				if(input.x !=wallDirx && input.x !=0){
					timeToWallunStick -=Time.deltaTime;
				}
				else{
					timeToWallunStick = wallStickTime;
				}
			}
			else{
				timeToWallunStick = wallStickTime;
			}
		}
		if (this.controller.collisionInfo.above || this.controller.collisionInfo.below) {
			velocity.y = 0;
		}
		#if UNITY_EDITOR
			if (Input.GetKeyDown (KeyCode.Space)) {
				if(wallSliding){
					if(collideWall){
						collideWall =false;
						playerDirection = playerDirection==1?-1:1;
					}
					if(wallDirx == input.x){
						velocity.x = - wallDirx * wallJumpClimb.x;
						velocity.y = wallJumpClimb.y;
					}
					else if(input.x ==0){

						velocity.x = - wallDirx * wallJumpOff.x;
						velocity.y = wallJumpOff.y;
					}
					else{
						velocity.x = - wallDirx * wallLeap.x;
						velocity.y = wallLeap.y;
					}
				}
				if(controller.collisionInfo.below){
					velocity.y = jumpVelocity;
				}
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
					if(wallSliding){
						if(collideWall){
							collideWall =false;
							playerDirection = playerDirection==1?-1:1;
						}
						if(wallDirx == input.x){
							velocity.x = - wallDirx * wallJumpClimb.x;
							velocity.y = wallJumpClimb.y;
						}
						else if(input.x ==0){
							
							velocity.x = - wallDirx * wallJumpOff.x;
							velocity.y = wallJumpOff.y;
						}
						else{
							velocity.x = - wallDirx * wallLeap.x;
							velocity.y = wallLeap.y;
						}
					}
					if(controller.collisionInfo.below){
						velocity.y = jumpVelocity;
					}
				}break;
				case TouchPhase.Ended:
				{
					//-------------->
				}break;
				default: break;
				}
			}
		}
		#endif
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
		collideWall = (controller.collisionInfo.right || controller.collisionInfo.left) ? true : false;
		//Player Action

		if (wallSliding) {
			playerAction = PlayerAction.WallSlide;
			playerAnimator.SetInteger ("State", 3);
			particleOpener(false);
		} 
		else if (collideWall) {
			playerAction = PlayerAction.WallCollide;
			if(oplayerAction != PlayerAction.WallSlide){
				playerAnimator.SetInteger ("State", 4);
			}
			particleOpener(false);
		} else if (!controller.collisionInfo.right && !controller.collisionInfo.left && !controller.collisionInfo.below && !controller.collisionInfo.above) {
			playerAction = PlayerAction.Jumping;
			playerAnimator.SetInteger ("State", 2);
			particleOpener(false);
		} else if (!collideWall) {
			playerAction = PlayerAction.Running;		
			playerAnimator.SetInteger ("State", 1);
			particleOpener(true);
		} else {
			playerAction = PlayerAction.Idle;
			playerAnimator.SetInteger ("State", 0);
			particleOpener(false);
		}
		if (ctemp_playerAction != playerAction) {
			oplayerAction = ctemp_playerAction;
			ctemp_playerAction = playerAction;
		}
		//Change Direction
		if(playerDirection == 1){//Right
			transform.localScale = new Vector3(scaleData,transform.localScale.y,transform.localScale.z);
			switch(playerAction){
				case PlayerAction.Jumping: break;
				case PlayerAction.Running: particleRotation(270);break;
				case PlayerAction.WallSlide: break;
			}
		}
		else{//Left
			transform.localScale = new Vector3(-scaleData,transform.localScale.y,transform.localScale.z);
			switch(playerAction){
				case PlayerAction.Jumping: break;
				case PlayerAction.Running: particleRotation(90);break;
				case PlayerAction.WallSlide: break;
			}
		}
	}
	void particleRotation(int angle){
		if (((int)playerParticleSystem.transform.rotation.eulerAngles.y) != angle) {
			playerParticleSystem.transform.Rotate (new Vector3 (0, angle, 0));
		}
	}
	void particleOpener(bool state){
		if (state) {
			if (!playerParticleSystem.gameObject.activeSelf) {
				playerParticleSystem.gameObject.SetActive (true);
				playerParticleSystem.Play ();
			}
		} 
		else {
			if (playerParticleSystem.gameObject.activeSelf) {
				playerParticleSystem.gameObject.SetActive (false);
				playerParticleSystem.Stop ();
			}
		}

	}
	void kinematicEquationForPlayer(){
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
	}
	public void AddCollector(){
		collector++;
	}
    public void PlayBackAudioSource()
    {
        if (mainBackAudioSource != null && GlobalVariables.ISMainBackAudio == true)
        {
            mainBackAudioSource.mute = false;
            mainBackAudioSource.Play();
        }
    }
    public void ToggleAudioSource()
    {
        if (GlobalVariables.ISMainBackAudio)
        {
            StopBackAudioSource();
            volBackImage.sprite = Vol_OFF;
        }
        else
        {
            GlobalVariables.ISMainBackAudio = true;
            PlayBackAudioSource();
            volBackImage.sprite = Vol_ON;
        }
    }
    public void StopBackAudioSource()
    {
        if (mainBackAudioSource != null)
        {
            GlobalVariables.ISMainBackAudio = false;
            mainBackAudioSource.Stop();
        }
    }
}
