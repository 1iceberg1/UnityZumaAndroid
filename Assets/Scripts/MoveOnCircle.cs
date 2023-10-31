using UnityEngine;

public class MoveOnCircle : MonoBehaviour
{
	public float radius = 1f;

	public float speed = 1f;

	public float currentAngle;

	public Transform center;

	private void Update()
	{
		currentAngle += Time.deltaTime * speed;
		Transform transform = base.transform;
		Vector3 position = center.position;
		float x = position.x + Mathf.Cos(currentAngle) * radius;
		Vector3 position2 = center.position;
		transform.position = new Vector3(x, position2.y + Mathf.Sin(currentAngle) * radius);
	}
}
