using UnityEngine;

public class Shooter : MonoBehaviour
{
	public Transform parentRegion;

	public float defaultSpeed;

	[HideInInspector]
	public float speed;

	public Vector3 direction;

	public bool autoMoveProjectile = true;

	protected virtual void Start()
	{
		speed = defaultSpeed;
	}

	public void Shoot(Projectile projectile)
	{
		projectile.transform.position = base.transform.position;
		projectile.transform.rotation = base.transform.rotation;
		projectile.transform.SetParent(parentRegion);
		projectile.transform.localScale = Vector3.one;
		projectile.speed = speed;
		projectile.direction = direction;
		if (autoMoveProjectile)
		{
			projectile.Move();
		}
	}
}
