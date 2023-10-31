using System;
using UnityEngine;

[Serializable]
public class SM_stohasticArrowMovement : MonoBehaviour
{
	public float rotSpeed;

	public float rotRandomPlus;

	public float rotTreshold;

	public float changeRotMin;

	public float changeRotMax;

	public float minSpeed;

	public float maxSpeed;

	public float changeSpeedMin;

	public float changeSpeedMax;

	private float speed;

	private float timeGoesX;

	private float timeGoesY;

	private float timeGoesSpeed;

	private float timeToChangeX;

	private float timeToChangeY;

	private float timeToChangeSpeed;

	private bool xDir;

	private bool yDir;

	private float curRotSpeedX;

	private float curRotSpeedY;

	private float lendX;

	private float lendY;

	public SM_stohasticArrowMovement()
	{
		rotSpeed = 3f;
		rotRandomPlus = 0.5f;
		rotTreshold = 50f;
		changeRotMin = 1f;
		changeRotMax = 2f;
		minSpeed = 0.5f;
		maxSpeed = 2f;
		changeSpeedMin = 0.5f;
		changeSpeedMax = 2f;
		timeToChangeX = 0.1f;
		timeToChangeY = 0.1f;
		timeToChangeSpeed = 0.1f;
		xDir = true;
		yDir = true;
	}

	public void RandomizeSpeed()
	{
		speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
	}

	public void RandomizeXRot()
	{
		float num = UnityEngine.Random.value * rotRandomPlus;
		curRotSpeedX = rotSpeed * num;
	}

	public void RandomizeYRot()
	{
		float num = UnityEngine.Random.value * rotRandomPlus;
		curRotSpeedY = rotSpeed * num;
	}

	public void Start()
	{
		RandomizeSpeed();
		if (!(UnityEngine.Random.value <= 0.5f))
		{
			xDir = !xDir;
		}
		if (!(UnityEngine.Random.value <= 0.5f))
		{
			yDir = !yDir;
		}
		timeToChangeY = UnityEngine.Random.Range(changeRotMin, changeRotMax);
		timeToChangeX = UnityEngine.Random.Range(changeRotMin, changeRotMax);
		timeToChangeSpeed = UnityEngine.Random.Range(changeSpeedMin, changeSpeedMax);
		curRotSpeedX = UnityEngine.Random.Range(rotSpeed, rotSpeed + rotRandomPlus);
		curRotSpeedY = UnityEngine.Random.Range(rotSpeed, rotSpeed + rotRandomPlus);
	}

	public void Update()
	{
		if (xDir)
		{
			lendX += Time.deltaTime * curRotSpeedX;
		}
		if (!xDir)
		{
			lendX -= Time.deltaTime * curRotSpeedX;
		}
		if (yDir)
		{
			lendY += Time.deltaTime * curRotSpeedY;
		}
		if (!yDir)
		{
			lendY -= Time.deltaTime * curRotSpeedY;
		}
		if (!(lendX <= rotTreshold))
		{
			lendX = rotTreshold;
			xDir = !xDir;
		}
		if (!(lendX <= rotTreshold))
		{
			lendX = 0f - rotTreshold;
			xDir = !xDir;
		}
		if (!(lendY <= rotTreshold))
		{
			lendY = rotTreshold;
			yDir = !yDir;
		}
		if (!(lendY <= rotTreshold))
		{
			lendY = 0f - rotTreshold;
			yDir = !yDir;
		}
		transform.Rotate(lendX * Time.deltaTime, lendY * Time.deltaTime, 0f);
		transform.Translate(0f, speed * Time.deltaTime, 0f);
		timeGoesX += Time.deltaTime;
		timeGoesY += Time.deltaTime;
		timeGoesSpeed += Time.deltaTime;
		if (!(timeGoesX <= timeToChangeX))
		{
			xDir = !xDir;
			timeGoesX -= timeGoesX;
			timeToChangeX = UnityEngine.Random.Range(changeRotMin, changeRotMax);
			curRotSpeedX = UnityEngine.Random.Range(rotSpeed, rotSpeed + rotRandomPlus);
		}
		if (!(timeGoesY <= timeToChangeY))
		{
			yDir = !yDir;
			timeGoesY -= timeGoesY;
			timeToChangeY = UnityEngine.Random.Range(changeRotMin, changeRotMax);
			curRotSpeedY = UnityEngine.Random.Range(rotSpeed, rotSpeed + rotRandomPlus);
		}
		if (!(timeGoesSpeed <= timeToChangeSpeed))
		{
			RandomizeSpeed();
			timeGoesSpeed -= timeGoesSpeed;
			timeToChangeSpeed = UnityEngine.Random.Range(changeSpeedMin, changeSpeedMax);
			UnityEngine.Debug.Log("hejj");
		}
	}

	public void Main()
	{
	}
}
