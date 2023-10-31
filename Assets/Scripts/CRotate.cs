using UnityEngine;

public class CRotate : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		base.transform.Rotate(Vector3.forward * Time.deltaTime * speed);
	}
}
