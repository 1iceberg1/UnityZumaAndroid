using System;
using UnityEngine;

[Serializable]
public class SM_UVScroller : MonoBehaviour
{
	public int targetMaterialSlot;

	public float speedY;

	public float speedX;

	private float timeWentX;

	private float timeWentY;

	public SM_UVScroller()
	{
		speedY = 0.5f;
	}

	public void Start()
	{
	}

	public void Update()
	{
		timeWentY += Time.deltaTime * speedY;
		timeWentX += Time.deltaTime * speedX;
		GetComponent<Renderer>().materials[targetMaterialSlot].SetTextureOffset("_MainTex", new Vector2(timeWentX, timeWentY));
	}

	public void Main()
	{
	}
}
