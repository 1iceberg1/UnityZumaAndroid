using UnityEngine;

public class PathedProjectileSpawner : MonoBehaviour
{
	public Transform destination;

	public PathedProjectile Projectile;

	public float speed;

	public float fireRate;

	private float nextShotInSeconds;

	private void Start()
	{
		nextShotInSeconds = fireRate;
	}

	private void Update()
	{
		if (!((nextShotInSeconds -= Time.deltaTime) > 0f))
		{
			nextShotInSeconds = fireRate;
			PathedProjectile pathedProjectile = UnityEngine.Object.Instantiate(Projectile, base.transform.position, base.transform.rotation);
			pathedProjectile.Initalize(destination, speed);
		}
	}

	private void OnDrawGimos()
	{
		if (!(destination == null))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, destination.position);
		}
	}
}
