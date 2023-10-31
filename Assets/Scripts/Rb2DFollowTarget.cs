using UnityEngine;

public class Rb2DFollowTarget : MonoBehaviour
{
	public Transform target;

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (target != null)
		{
			rb.MovePosition(target.position);
		}
	}
}
