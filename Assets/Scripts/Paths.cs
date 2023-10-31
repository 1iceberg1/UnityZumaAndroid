using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{
	public static Paths instance;

	public List<Vector3>[] waypoints = new List<Vector3>[2];

	private void Awake()
	{
		instance = this;
		iTweenPath[] components = GetComponents<iTweenPath>();
		for (int i = 0; i < components.Length; i++)
		{
			waypoints[i] = iTween.GetSmoothPoints(components[i].nodes.ToArray(), 5);
		}
	}
}
