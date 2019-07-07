using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyAI : RaycastController {
	PlayerController playerController;
	public GameObject playerGameObject;
	public LayerMask playerMask;
	public Vector3[] localWaypoints;
	Vector3[] globalWayPoints;
	public float speed;
	int fromwayPointIndex;
	float percentBetweenWayPoint;
	public bool cyclic;
	public float waitTime;
	float nextMoveTime;
	[Range(0,2)]
	public float easeAmount;
	[HideInInspector] 
	public bool isPlayerHit = false;
	Vector3 tempVelocity;
	float scaleData;
	public override void  Start () {
		base.Start ();
		playerController = playerGameObject.gameObject.GetComponent<PlayerController> ();
		globalWayPoints = new Vector3[localWaypoints.Length];
		for(int i=0;i<localWaypoints.Length;i++){
			globalWayPoints[i] = localWaypoints[i] + transform.position;
		}
		scaleData = transform.localScale.x;
	}
	void Update () {
		UpdateRaycastOrigins ();
		//Game pause---------------------------------------------------->
		if (HudEvent.isGamepause) {return;}
		//-------------------------------------------------------------->
		tempVelocity = CalculatePlatformMovement();
		CalculatePlayerMovement (tempVelocity);
		transform.Translate (tempVelocity);
		CalculateEnemyDirection ();
	}
	float Ease(float x){
		float a = easeAmount + 1;
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}
	Vector3 CalculatePlatformMovement(){
		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}
		fromwayPointIndex %= globalWayPoints.Length;
		int toWayPointIndex = (fromwayPointIndex + 1) % globalWayPoints.Length;
		float distanceBetweenWayPoints = Vector3.Distance (globalWayPoints [fromwayPointIndex], globalWayPoints [toWayPointIndex]);
		percentBetweenWayPoint += Time.deltaTime * speed / distanceBetweenWayPoints;
		percentBetweenWayPoint = Mathf.Clamp01 (percentBetweenWayPoint);
		float easePercentage = Ease(percentBetweenWayPoint);
		Vector3 newPos = Vector3.Lerp (globalWayPoints [fromwayPointIndex], globalWayPoints [toWayPointIndex], easePercentage);
		if (percentBetweenWayPoint >= 1) {
			percentBetweenWayPoint =0;
			fromwayPointIndex ++;
			if(!cyclic){
				if(fromwayPointIndex >= globalWayPoints.Length -1){
					fromwayPointIndex =0;
					System.Array.Reverse(globalWayPoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}
		
		return newPos - transform.position;
	}
	void CalculatePlayerMovement(Vector3 velocity){
		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);
		isPlayerHit = false;
		//FOR HORIZONTAL
		{
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
			
			if (Mathf.Abs(velocity.x) < skinWidth) {
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
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;		
			for(int i=0;i < this.verticalRayCount;i++){
				//------------------------------------->Side A
				Vector2 rayOrigin =raycastOrigins.bottomLeft;
				rayOrigin +=Vector2.right * (verticalRaySpacing * i + velocity.y);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin A
					isPlayerHit =true;
				}
				rayOrigin =raycastOrigins.topLeft;
				rayOrigin +=Vector2.right * (verticalRaySpacing * i + velocity.y);
				hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength,playerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){//Collision Begin B
					isPlayerHit =true;
				}
			}
		}
		if (isPlayerHit) {
			playerController.playerAction = PlayerController.PlayerAction.Dead;
		}
		//---------------------------------->
	}
	void OnDrawGizmos(){
		if (localWaypoints != null) {
			Gizmos.color = Color.blue;
			float size = .3f;
			for(int i=0;i<localWaypoints.Length;i++){
				Vector3 globalWayPointPos =(Application.isPlaying)?globalWayPoints[i]:localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWayPointPos - Vector3.up * size,globalWayPointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWayPointPos - Vector3.left * size,globalWayPointPos + Vector3.left * size);
			}
		}
	}
	public void CalculateEnemyDirection(){
		if (tempVelocity.x > 0) {
			//right
			transform.localScale = new Vector3(-scaleData,transform.localScale.y,transform.localScale.z);
		} 
		else if (tempVelocity.x < 0) {
			//left
			transform.localScale = new Vector3(scaleData,transform.localScale.y,transform.localScale.z);
		} 
	}
}
