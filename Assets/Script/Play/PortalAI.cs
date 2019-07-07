using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortalAI :RaycastController  {
	PlayerController playerController;
	public GameObject playerGameObject;
	public LayerMask playerMask;
	Vector3 mainPositon;
	bool isHomeCollide=false;
	bool isGetAnimateFinished =false;
	float timeToWait = 1.0f;
	float currentTime=0f;


	public GameObject TopHud, MainHud;
	public GameObject EventSystem;

	public override void  Start () {
		base.Start ();
		playerController = playerGameObject.gameObject.GetComponent<PlayerController> ();
	}
	void Update () {
		UpdateRaycastOrigins ();
		if (!isHomeCollide) {
			CalculatePlayerMovement (mainPositon);
		}
		else {
			if(currentTime > timeToWait){
				//Do Animate
				if(!isGetAnimateFinished){
					isGetAnimateFinished =true;
					EventSystem.gameObject.GetComponent<HudEvent>().Hud_SetTopHudGameObject(TopHud);
					EventSystem.gameObject.GetComponent<HudEvent>().Hud_FinishedToggle(MainHud);
				}
			}
			else{
				currentTime ++;	
			}
		}
	}
	void CalculatePlayerMovement(Vector3 mainPositon){
		float directionX = Mathf.Sign (mainPositon.x);
		float directionY = Mathf.Sign (mainPositon.y);
		bool isPlayerHit = false;
		//FOR HORIZONTAL
		{
			float rayLength = Mathf.Abs (mainPositon.x) + skinWidth;
			
			if (Mathf.Abs(mainPositon.x) < skinWidth) {
				rayLength = 2 * skinWidth;
			}
			for(int i=0;i < this.horizontalRayCount;i++){
				//-------------------------------->Side A
				Vector2 rayOrigin = raycastOrigins.bottomLeft;
				rayOrigin +=Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin A
					isPlayerHit =true;
				}
				//---------------------------------->Side B
				rayOrigin =raycastOrigins.bottomRight;
				rayOrigin +=Vector2.up * (horizontalRaySpacing * i);
				hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin B
					isPlayerHit =true;
				}
			}
		}
		//FOR VERTICAL
		{
			float rayLength = Mathf.Abs (mainPositon.y) + skinWidth;		
			for(int i=0;i < this.verticalRayCount;i++){
				//------------------------------------->Side A
				Vector2 rayOrigin =raycastOrigins.bottomLeft;
				rayOrigin +=Vector2.right * (verticalRaySpacing * i + mainPositon.y);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin A
					isPlayerHit =true;
				}
				rayOrigin =raycastOrigins.topLeft;
				rayOrigin +=Vector2.right * (verticalRaySpacing * i + mainPositon.y);
				hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin B
					isPlayerHit =true;
				}
			}
		}
		if (isPlayerHit) {
			isHomeCollide =true;
		}
		//---------------------------------->
	}
}
