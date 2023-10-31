using System;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
	public Action<Collider2D> onTriggerEnter;

	public Action<Collider2D> onTriggerExit;

	public Action<Collision2D> onCollisionEnter;

	public Action<Collision2D> onCollisionExit;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (onTriggerEnter != null)
		{
			onTriggerEnter(collider);
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (onTriggerExit != null)
		{
			onTriggerExit(collider);
		}
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (onCollisionEnter != null)
		{
			onCollisionEnter(coll);
		}
	}

	private void OnCollisionExit2D(Collision2D coll)
	{
		if (onCollisionExit != null)
		{
			onCollisionExit(coll);
		}
	}
}
