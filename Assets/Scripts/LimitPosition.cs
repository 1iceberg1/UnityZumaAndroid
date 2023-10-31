using UnityEngine;

public class LimitPosition : MonoBehaviour
{
	public Vector2 maxXAndY;

	public Vector2 minXAndY;

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float y = position2.y;
		x = Mathf.Clamp(x, minXAndY.x, maxXAndY.x);
		y = Mathf.Clamp(y, minXAndY.y, maxXAndY.y);
		Transform transform = base.transform;
		float x2 = x;
		float y2 = y;
		Vector3 position3 = base.transform.position;
		transform.position = new Vector3(x2, y2, position3.z);
	}
}
