using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class FollowTarget2D : MonoBehaviour
	{
		public Transform target;

		public Vector3 offset = new Vector3(0f, 7.5f, 0f);

		private void Update()
		{
			if (!(target == null))
			{
				base.transform.SetPosition2D(target.position + offset);
			}
		}
	}
}
