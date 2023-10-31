using System;
using UnityEngine;

[Serializable]
public class SM_randoMMoveInBox : MonoBehaviour
{
	public float xspeed;

	public float yspeed;

	public float zspeed;

	public float speedDeviation;

	public float xDim;

	public float yDim;

	public float zDim;

	public SM_randoMMoveInBox()
	{
		xspeed = 1f;
		yspeed = 1.5f;
		zspeed = 2f;
		xDim = 0.3f;
		yDim = 0.3f;
		zDim = 0.3f;
	}

	public void Start()
	{
		transform.localPosition = new Vector3(0f, 0f, 0f);
		xspeed += (float)UnityEngine.Random.Range(-1, 1) * speedDeviation;
		yspeed += (float)UnityEngine.Random.Range(-1, 1) * speedDeviation;
		zspeed += (float)UnityEngine.Random.Range(-1, 1) * speedDeviation;
	}

	public void Update()
	{
		transform.Translate(new Vector3(xspeed, yspeed, zspeed) * Time.deltaTime);
		Vector3 localPosition = transform.localPosition;
		if (!(localPosition.x <= xDim))
		{
			xspeed = 0f - Mathf.Abs(xspeed);
		}
		Vector3 localPosition2 = transform.localPosition;
		if (!(localPosition2.x >= 0f - xDim))
		{
			xspeed = Mathf.Abs(xspeed);
		}
		Vector3 localPosition3 = transform.localPosition;
		if (!(localPosition3.y <= yDim))
		{
			yspeed = 0f - Mathf.Abs(yspeed);
		}
		Vector3 localPosition4 = transform.localPosition;
		if (!(localPosition4.y >= 0f - yDim))
		{
			yspeed = Mathf.Abs(yspeed);
		}
		Vector3 localPosition5 = transform.localPosition;
		if (!(localPosition5.z <= zDim))
		{
			zspeed = 0f - Mathf.Abs(zspeed);
		}
		Vector3 localPosition6 = transform.localPosition;
		if (!(localPosition6.z >= 0f - zDim))
		{
			zspeed = Mathf.Abs(zspeed);
		}
	}

	public void Main()
	{
	}
}
