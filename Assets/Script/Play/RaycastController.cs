using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {
	public LayerMask collisionMask;
	public LayerMask collectorMask;
	public LayerMask portalMask;
	public const float skinWidth = 0.015f;
	public int horizontalRayCount =4;
	public int verticalRayCount = 4;
	public float horizontalRaySpacing,verticalRaySpacing;
	public RaycastOrigins raycastOrigins;
	[HideInInspector]
	public BoxCollider2D collider;
	public virtual void Awake(){
		this.collider = GetComponent<BoxCollider2D> ();	
	}
	public virtual void Start () {
		CalculateRaySpacing ();
	}
	public void UpdateRaycastOrigins(){
		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);
		
		this.raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		this.raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		this.raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		this.raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	public void CalculateRaySpacing(){
		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);
		
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
		
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	public struct RaycastOrigins{
		public Vector2 topLeft,topRight,bottomLeft,bottomRight;
	}
}
