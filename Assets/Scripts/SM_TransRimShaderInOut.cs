using System;
using UnityEngine;

[Serializable]
public class SM_TransRimShaderInOut : MonoBehaviour
{
	public float str;

	public float fadeIn;

	public float stay;

	public float fadeOut;

	private float timeGoes;

	private float currStr;

	public SM_TransRimShaderInOut()
	{
		str = 1f;
		fadeIn = 1f;
		stay = 1f;
		fadeOut = 1f;
	}

	public void Start()
	{
		GetComponent<Renderer>().material.SetFloat("_AllPower", currStr);
	}

	public void Update()
	{
		timeGoes += Time.deltaTime;
		if (!(timeGoes >= fadeIn))
		{
			currStr = timeGoes * str * (1f / fadeIn);
		}
		if (!(timeGoes <= fadeIn) && !(timeGoes >= fadeIn + stay))
		{
			currStr = str;
		}
		if (!(timeGoes <= fadeIn + stay))
		{
			currStr = str - (timeGoes - (fadeIn + stay)) * (1f / fadeOut);
		}
		GetComponent<Renderer>().material.SetFloat("_AllPower", currStr);
	}

	public void Main()
	{
	}
}
