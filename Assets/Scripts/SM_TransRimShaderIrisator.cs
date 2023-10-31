using System;
using UnityEngine;

[Serializable]
public class SM_TransRimShaderIrisator : MonoBehaviour
{
	public float topStr;

	public float botStr;

	public float minSpeed;

	public float maxSpeed;

	private float speed;

	private float timeGoes;

	private bool timeGoesUp;

	public SM_TransRimShaderIrisator()
	{
		topStr = 2f;
		botStr = 1f;
		minSpeed = 1f;
		maxSpeed = 1f;
		timeGoesUp = true;
	}

	public void RandomizeSpeed()
	{
		speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
	}

	public void Start()
	{
		timeGoes = botStr;
		speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
	}

	public void Update()
	{
		if (!(timeGoes <= topStr))
		{
			timeGoesUp = false;
			RandomizeSpeed();
		}
		if (!(timeGoes >= botStr))
		{
			timeGoesUp = true;
			RandomizeSpeed();
		}
		if (timeGoesUp)
		{
			timeGoes += Time.deltaTime * speed;
		}
		if (!timeGoesUp)
		{
			timeGoes -= Time.deltaTime * speed;
		}
		float value = timeGoes;
		GetComponent<Renderer>().material.SetFloat("_AllPower", value);
	}

	public void Main()
	{
	}
}
