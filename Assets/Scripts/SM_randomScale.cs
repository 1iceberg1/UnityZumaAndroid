using System;
using UnityEngine;

[Serializable]
public class SM_randomScale : MonoBehaviour
{
	public float minScale;

	public float maxScale;

	public SM_randomScale()
	{
		minScale = 1f;
		maxScale = 2f;
	}

	public void Start()
	{
		float num = UnityEngine.Random.Range(minScale, maxScale);
		transform.localScale = new Vector3(num, num, num);
	}

	public void Update()
	{
	}

	public void Main()
	{
	}
}
