using UnityEngine;

public class OnComplete : MonoBehaviour
{
	public void OnMoveComplete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
