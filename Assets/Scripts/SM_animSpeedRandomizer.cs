using System;
using UnityEngine;

[Serializable]
public class SM_animSpeedRandomizer : MonoBehaviour
{
	public float minSpeed;

	public float maxSpeed;

	public SM_animSpeedRandomizer()
	{
		minSpeed = 0.7f;
		maxSpeed = 1.5f;
	}

	public void Start()
	{
	}

	public void Main()
	{
	}
}
