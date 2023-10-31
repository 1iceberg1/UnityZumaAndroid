using UnityEngine;

public class Pusher : Sphere
{
	public Animator anim1;

	public Animator anim2;

	private void Update()
	{
		anim1.speed = ((!isPausing) ? speed : 0f) / controller.defaultSpeed;
		anim2.speed = ((!isPausing) ? speed : 0f) / controller.defaultSpeed;
	}

	protected override void UpdateRotation(float speed)
	{
		if (base.waypoints != null && pathIndex + 1 >= 0 && pathIndex >= 0)
		{
			base.transform.LookAt2D(base.waypoints[pathIndex + 1]);
		}
	}
}
