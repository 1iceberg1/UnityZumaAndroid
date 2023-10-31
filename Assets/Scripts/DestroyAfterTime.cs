using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float time;

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, time);
	}
}
