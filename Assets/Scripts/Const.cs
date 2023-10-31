using UnityEngine;

public class Const
{
	public const float PAUSE_TIME = 5f;

	public const float BACKWARD_TIME = 1.3f;

	public static readonly Vector3[] direction = new Vector3[4]
	{
		Vector3.up,
		-Vector3.up,
		Vector3.right,
		-Vector3.right
	};

	public static readonly string[] ballTypeStrings = new string[7]
	{
		"1",
		"2",
		"3",
		"4",
		"5",
		"h",
		"b"
	};

	public static readonly Sphere.Type[] ballTypes = new Sphere.Type[7]
	{
		Sphere.Type.Sphere1,
		Sphere.Type.Sphere2,
		Sphere.Type.Sphere3,
		Sphere.Type.Sphere4,
		Sphere.Type.Sphere5,
		Sphere.Type.Hidden,
		Sphere.Type.Black
	};

	public static readonly int[] boosterIDs = new int[5]
	{
		1,
		2,
		3,
		4,
		5
	};
}
