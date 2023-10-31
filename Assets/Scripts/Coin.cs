using UnityEngine;

public class Coin : MonoBehaviour
{
	private void OnMoveComplete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
