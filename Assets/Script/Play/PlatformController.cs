using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlatformController : RaycastController {
	public LayerMask passengerMask;
	List<PassengerMovement> passengerMovement;
	Dictionary<Transform,Controller2D> passengerDictonary = new Dictionary<Transform,Controller2D >();

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
	public override void  Start () {
		base.Start ();
		globalWayPoints = new Vector3[localWaypoints.Length];
		for(int i=0;i<localWaypoints.Length;i++){
			globalWayPoints[i] = localWaypoints[i] + transform.position;
		}
	}
	void Update () {
		UpdateRaycastOrigins ();
		Vector3 velocity = CalculatePlatformMovement();
		CalculatePassengerMovement (velocity);
		MovePassenger (true);
		transform.Translate (velocity);
		MovePassenger (false);
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
	void MovePassenger(bool beforeMovePlatform){
		foreach (PassengerMovement passenger in passengerMovement) {
			if(!passengerDictonary.ContainsKey(passenger.transform)){
				passengerDictonary.Add(passenger.transform,passenger.transform.GetComponent<Controller2D>());
			}
			if(passenger.moveBeforePlatform == beforeMovePlatform){
				passengerDictonary[passenger.transform].Move(passenger.velocity,passenger.standingOnPlatform);
			}

		}
	}
	void CalculatePassengerMovement(Vector3 velocity){
		HashSet<Transform> movePassenger = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();
		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);
		//Vertically moving platform
		if(velocity.y!=0){
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;
			
			for(int i=0;i < this.verticalRayCount;i++){
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft: raycastOrigins.topLeft;
				rayOrigin +=Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength,passengerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){
					if(!movePassenger.Contains(hit.transform)){
						movePassenger.Add(hit.transform);
						float pushX = (directionY ==1)?velocity.x:0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),directionY == 1, true));

					}
				}
			}
		}
		//Horizontal moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
			for(int i=0;i < this.horizontalRayCount;i++){
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft: raycastOrigins.bottomRight;
				rayOrigin +=Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX,rayLength,passengerMask);
				if(hit){
					if(!movePassenger.Contains(hit.transform)){
						movePassenger.Add(hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth; 
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),false, true));
					}
				}
			}
		}
		//Passenger on top of horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = skinWidth * 2;
			
			for(int i=0;i < this.verticalRayCount;i++){
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up ,rayLength,passengerMask);
				Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength,Color.red);
				if(hit){
					if(!movePassenger.Contains(hit.transform)){
						movePassenger.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),true, false));
					}
				}
			}
		}
	}
	struct PassengerMovement{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;
		public PassengerMovement(Transform _transform,Vector3 _velocity,bool _standingOnPlatform,bool _moveBeforePlatform){
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
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
}
