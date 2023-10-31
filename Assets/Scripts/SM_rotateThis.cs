using System;
using UnityEngine;

[Serializable]
public class SM_rotateThis : MonoBehaviour
{
	public float rotationSpeedX;

	public float rotationSpeedY;

	public float rotationSpeedZ;

	public bool local;

	private Vector3 rotationVector;

	public SM_rotateThis()
	{
		rotationSpeedX = 90f;
		local = true;
		rotationVector = new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
	}

	public void Update()
	{
		if (local)
		{
			transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime);
		}
		if (!local)
		{
			transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime, Space.World);
		}
	}

	public void Main()
	{
	}
}
