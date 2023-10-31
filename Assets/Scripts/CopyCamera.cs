using UnityEngine;

public class CopyCamera : MonoBehaviour
{
	public Camera target;

	public bool copySize;

	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		if (copySize)
		{
			cam.orthographicSize = target.orthographicSize;
		}
	}
}
