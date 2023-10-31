using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(SkeletonUtilityBone))]
	[ExecuteInEditMode]
	public class SkeletonUtilityGroundConstraint : SkeletonUtilityConstraint
	{
		[Tooltip("LayerMask for what objects to raycast against")]
		public LayerMask groundMask;

		[Tooltip("The 2D")]
		public bool use2D;

		[Tooltip("Uses SphereCast for 3D mode and CircleCast for 2D mode")]
		public bool useRadius;

		[Tooltip("The Radius")]
		public float castRadius = 0.1f;

		[Tooltip("How high above the target bone to begin casting from")]
		public float castDistance = 5f;

		[Tooltip("X-Axis adjustment")]
		public float castOffset;

		[Tooltip("Y-Axis adjustment")]
		public float groundOffset;

		[Tooltip("How fast the target IK position adjusts to the ground.  Use smaller values to prevent snapping")]
		public float adjustSpeed = 5f;

		private Vector3 rayOrigin;

		private Vector3 rayDir = new Vector3(0f, -1f, 0f);

		private float hitY;

		private float lastHitY;

		protected override void OnEnable()
		{
			base.OnEnable();
			Vector3 position = base.transform.position;
			lastHitY = position.y;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		public override void DoUpdate()
		{
			rayOrigin = base.transform.position + new Vector3(castOffset, castDistance, 0f);
			hitY = float.MinValue;
			if (use2D)
			{
				RaycastHit2D raycastHit2D = (!useRadius) ? Physics2D.Raycast(rayOrigin, rayDir, castDistance + groundOffset, groundMask) : Physics2D.CircleCast(rayOrigin, castRadius, rayDir, castDistance + groundOffset, groundMask);
				if (raycastHit2D.collider != null)
				{
					Vector2 point = raycastHit2D.point;
					hitY = point.y + groundOffset;
					if (Application.isPlaying)
					{
						hitY = Mathf.MoveTowards(lastHitY, hitY, adjustSpeed * Time.deltaTime);
					}
				}
				else if (Application.isPlaying)
				{
					float current = lastHitY;
					Vector3 position = base.transform.position;
					hitY = Mathf.MoveTowards(current, position.y, adjustSpeed * Time.deltaTime);
				}
			}
			else
			{
				bool flag = false;
				if ((!useRadius) ? Physics.Raycast(rayOrigin, rayDir, out RaycastHit hitInfo, castDistance + groundOffset, groundMask) : Physics.SphereCast(rayOrigin, castRadius, rayDir, out hitInfo, castDistance + groundOffset, groundMask))
				{
					Vector3 point2 = hitInfo.point;
					hitY = point2.y + groundOffset;
					if (Application.isPlaying)
					{
						hitY = Mathf.MoveTowards(lastHitY, hitY, adjustSpeed * Time.deltaTime);
					}
				}
				else if (Application.isPlaying)
				{
					float current2 = lastHitY;
					Vector3 position2 = base.transform.position;
					hitY = Mathf.MoveTowards(current2, position2.y, adjustSpeed * Time.deltaTime);
				}
			}
			Vector3 position3 = base.transform.position;
			position3.y = Mathf.Clamp(position3.y, Mathf.Min(lastHitY, hitY), float.MaxValue);
			base.transform.position = position3;
			Bone bone = utilBone.bone;
			Vector3 localPosition = base.transform.localPosition;
			bone.X = localPosition.x;
			Bone bone2 = utilBone.bone;
			Vector3 localPosition2 = base.transform.localPosition;
			bone2.Y = localPosition2.y;
			lastHitY = hitY;
		}

		private void OnDrawGizmos()
		{
			Vector3 vector = rayOrigin + rayDir * Mathf.Min(castDistance, rayOrigin.y - hitY);
			Vector3 to = rayOrigin + rayDir * castDistance;
			Gizmos.DrawLine(rayOrigin, vector);
			if (useRadius)
			{
				Gizmos.DrawLine(new Vector3(vector.x - castRadius, vector.y - groundOffset, vector.z), new Vector3(vector.x + castRadius, vector.y - groundOffset, vector.z));
				Gizmos.DrawLine(new Vector3(to.x - castRadius, to.y, to.z), new Vector3(to.x + castRadius, to.y, to.z));
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(vector, to);
		}
	}
}
