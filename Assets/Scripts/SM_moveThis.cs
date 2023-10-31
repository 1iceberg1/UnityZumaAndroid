using System;
using UnityEngine;

[Serializable]
public class SM_moveThis : MonoBehaviour
{
	public float translationSpeedX;

	public float translationSpeedY;

	public float translationSpeedZ;

	public bool local;

	public SM_moveThis()
	{
		translationSpeedY = 1f;
		local = true;
	}

	public void Update()
	{
		if (local)
		{
			transform.Translate(new Vector3(translationSpeedX, translationSpeedY, translationSpeedZ) * Time.deltaTime);
		}
		if (!local)
		{
			transform.Translate(new Vector3(translationSpeedX, translationSpeedY, translationSpeedZ) * Time.deltaTime, Space.World);
		}
	}

	public void Main()
	{
	}
}
