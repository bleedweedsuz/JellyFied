using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraController : MonoBehaviour {
	public GameObject playerObject;
	public List<Vector3> gizmosList = new List<Vector3>();
	float size = 0.4f;
	Camera mainCamera;
	[Range(1,5)]
	public float NewCamPos=1.8f;
	void Start(){
		mainCamera = Camera.main;
	}
	void Update () {
		//Change only the y Value in the camera
		Vector3 cameraPos = mainCamera.transform.localPosition;
		Vector3 playerPos = playerObject.transform.localPosition;

		if (gizmosList!= null) {
			//Check position for Left
			//Cordinate A
			Vector3 A = gizmosList[0];
			//Cordinate B
			Vector3 B = gizmosList[1];
			//Cordinate C
			Vector3 C = gizmosList[2];
			//Cordinate D
			Vector3 D = gizmosList[3];
			//Pass all Co-Ordinate to check data
			//For X
			if(cameraPos.x < A.x || cameraPos.x < B.x){//For Left
				cameraPos.x = A.x;
			}
			else if(cameraPos.x > C.x || cameraPos.x > D.x){
				cameraPos.x = C.x;
			}
			//For Y
			if(cameraPos.y < A.y || cameraPos.y < D.y){
				cameraPos.y = A.y;
			}
			else if(cameraPos.y > B.y || cameraPos.y > C.y){
				cameraPos.y = B.y;
			}
		}
		mainCamera.transform.position = Vector3.Lerp (cameraPos, playerPos, 0.1f) + new Vector3 (0, 0, -NewCamPos);
	}
	public void OnDrawGizmos(){
		if (gizmosList != null) {
			Gizmos.color = Color.red;
			for(int i=0;i<gizmosList.Count;i++){
				Vector3 globalWayPointPos = gizmosList[i];
				Gizmos.DrawLine(globalWayPointPos - Vector3.up * size,globalWayPointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWayPointPos - Vector3.left * size,globalWayPointPos + Vector3.left * size);
			}
		}
	}
	public List<Vector3> getGizmosList(){
		return this.gizmosList;
	}
}
