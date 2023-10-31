using UnityEngine;

public class Bound : MonoBehaviour
{
	private void OnTriggerExit(Collider collider)
	{
		Sphere component = collider.gameObject.GetComponent<Sphere>();
		if (component != null && component.status == Sphere.Status.Flying)
		{
			component.MyDestroy();
			GameState.EndFlying();
		}
	}
}
