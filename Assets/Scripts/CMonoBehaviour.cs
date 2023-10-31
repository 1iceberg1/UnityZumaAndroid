using UnityEngine;

public class CMonoBehaviour : MonoBehaviour
{
	public void DestroyIfExist(Transform objTransform)
	{
		if (objTransform != null)
		{
			UnityEngine.Object.Destroy(objTransform.gameObject);
		}
	}
}
