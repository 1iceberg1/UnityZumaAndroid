using System;
using UnityEngine;
//using UnityScript.Lang;

[Serializable]
public class SM_animRandomizer : MonoBehaviour
{
	public AnimationClip[] animList;

	public AnimationClip actualAnim;

	public float minSpeed;

	public float maxSpeed;

	public SM_animRandomizer()
	{
		minSpeed = 0.7f;
		maxSpeed = 1.5f;
	}

	public void Start()
	{
		//float num = Mathf.Round(UnityEngine.Random.Range(0, Extensions.get_length((System.Array)animList)));
		//actualAnim = animList[(int)num];
	}

	public void Main()
	{
	}
}
