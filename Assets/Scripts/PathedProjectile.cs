using UnityEngine;

public class PathedProjectile : MonoBehaviour
{
	private Transform destination;

	private float speed;

	private void Start()
	{
	}

	public void Initalize(Transform destination, float speed)
	{
		this.destination = destination;
		this.speed = speed;
	}

	private void Update()
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, destination.position, Time.deltaTime * speed);
		float sqrMagnitude = (destination.transform.position - base.transform.position).sqrMagnitude;
		if (!(sqrMagnitude > 0.0001f))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
	}
}
