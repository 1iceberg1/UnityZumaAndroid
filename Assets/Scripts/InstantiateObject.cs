using System.Collections;
using UnityEngine;

public class InstantiateObject : MonoBehaviour
{
	public GameObject gameObj;

	public bool isChild = true;

	public float delayTime;

	[HideInInspector]
	public bool repeat;

	[HideInInspector]
	public bool infinity = true;

	[HideInInspector]
	public int numRepeat;

	[HideInInspector]
	public float gapTime;

	private void Start()
	{
		if (repeat && gapTime == 0f)
		{
			UnityEngine.Debug.LogError("You must set gap time between each repeat.");
		}
		else
		{
			StartCoroutine(DoWork());
		}
	}

	private IEnumerator DoWork()
	{
		yield return new WaitForSeconds(delayTime);
		int count = 0;
		do
		{
			GameObject clone = UnityEngine.Object.Instantiate(gameObj, base.transform.position, base.transform.rotation);
			clone.transform.SetParent((!isChild) ? null : base.transform);
			clone.transform.localScale = Vector3.one;
			count++;
			if (!repeat)
			{
				break;
			}
			yield return new WaitForSeconds(gapTime);
		}
		while (infinity || count != numRepeat + 1);
	}
}
