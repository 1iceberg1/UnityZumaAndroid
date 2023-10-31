using System;
using UnityEngine;

[Serializable]
public class SM_materialColorRandomizer : MonoBehaviour
{
	public Color color1;

	public Color color2;

	public bool unifiedColor;

	private float ColR;

	private float ColG;

	private float ColB;

	private float ColA;

	public SM_materialColorRandomizer()
	{
		color1 = new Color(0.6f, 0.6f, 0.6f, 1f);
		color2 = new Color(0.4f, 0.4f, 0.4f, 1f);
		unifiedColor = true;
	}

	public void Start()
	{
		if (!unifiedColor)
		{
			ColR = UnityEngine.Random.Range(color1.r, color2.r);
			ColG = UnityEngine.Random.Range(color1.g, color2.g);
			ColB = UnityEngine.Random.Range(color1.b, color2.b);
			ColA = UnityEngine.Random.Range(color1.a, color2.a);
		}
		if (unifiedColor)
		{
			float value = UnityEngine.Random.value;
			ColR = Mathf.Min(color1.r, color2.r) + Mathf.Abs(color1.r - color2.r) * value;
			ColG = Mathf.Min(color1.g, color2.g) + Mathf.Abs(color1.g - color2.g) * value;
			ColB = Mathf.Min(color1.b, color2.b) + Mathf.Abs(color1.b - color2.b) * value;
		}
		GetComponent<Renderer>().material.color = new Color(ColR, ColG, ColB, ColA);
	}

	public void Main()
	{
	}
}
