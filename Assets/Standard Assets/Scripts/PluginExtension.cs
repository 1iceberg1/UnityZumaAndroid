using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PluginExtension
{
	public static void SetX(this Transform transform, float x)
	{
		Vector3 position = transform.position;
		float y = position.y;
		Vector3 position2 = transform.position;
		Vector3 vector2 = transform.position = new Vector3(x, y, position2.z);
	}

	public static void SetY(this Transform transform, float y)
	{
		Vector3 position = transform.position;
		float x = position.x;
		Vector3 position2 = transform.position;
		Vector3 vector2 = transform.position = new Vector3(x, y, position2.z);
	}

	public static void SetZ(this Transform transform, float z)
	{
		Vector3 position = transform.position;
		float x = position.x;
		Vector3 position2 = transform.position;
		Vector3 vector2 = transform.position = new Vector3(x, position2.y, z);
	}

	public static void SetPosition2D(this Transform transform, Vector3 target)
	{
		float x = target.x;
		float y = target.y;
		Vector3 position = transform.position;
		Vector3 vector2 = transform.position = new Vector3(x, y, position.z);
	}

	public static void SetLocalX(this Transform transform, float x)
	{
		Vector3 localPosition = transform.localPosition;
		float y = localPosition.y;
		Vector3 localPosition2 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition2.z);
	}

	public static void SetLocalY(this Transform transform, float y)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition2.z);
	}

	public static void SetLocalZ(this Transform transform, float z)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, localPosition2.y, z);
	}

	public static void MoveLocalX(this Transform transform, float deltaX)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x + deltaX;
		Vector3 localPosition2 = transform.localPosition;
		float y = localPosition2.y;
		Vector3 localPosition3 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition3.z);
	}

	public static void MoveLocalY(this Transform transform, float deltaY)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = transform.localPosition;
		float y = localPosition2.y + deltaY;
		Vector3 localPosition3 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition3.z);
	}

	public static void MoveLocalZ(this Transform transform, float deltaZ)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = transform.localPosition;
		float y = localPosition2.y;
		Vector3 localPosition3 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition3.z + deltaZ);
	}

	public static void MoveLocalXYZ(this Transform transform, float deltaX, float deltaY, float deltaZ)
	{
		Vector3 localPosition = transform.localPosition;
		float x = localPosition.x + deltaX;
		Vector3 localPosition2 = transform.localPosition;
		float y = localPosition2.y + deltaY;
		Vector3 localPosition3 = transform.localPosition;
		Vector3 vector2 = transform.localPosition = new Vector3(x, y, localPosition3.z + deltaZ);
	}

	public static void SetLocalScaleX(this Transform transform, float x)
	{
		Vector3 localScale = transform.localScale;
		float y = localScale.y;
		Vector3 localScale2 = transform.localScale;
		Vector3 vector2 = transform.localScale = new Vector3(x, y, localScale2.z);
	}

	public static void SetLocalScaleY(this Transform transform, float y)
	{
		Vector3 localScale = transform.localScale;
		float x = localScale.x;
		Vector3 localScale2 = transform.localScale;
		Vector3 vector2 = transform.localScale = new Vector3(x, y, localScale2.z);
	}

	public static void SetLocalScaleZ(this Transform transform, float z)
	{
		Vector3 localScale = transform.localScale;
		float x = localScale.x;
		Vector3 localScale2 = transform.localScale;
		Vector3 vector2 = transform.localScale = new Vector3(x, localScale2.y, z);
	}

	public static void Times(this int count, Action action)
	{
		for (int i = 0; i < count; i++)
		{
			action();
		}
	}

	public static void SetColorAlpha(this Image image, float a)
	{
		Color color = image.color;
		color.a = a;
		image.color = color;
	}

	public static void LookAt2D(this Transform transform, Vector3 target, float angle = 0f)
	{
		Vector3 vector = target - transform.position;
		angle += Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
	}

	public static void LookAt2D(this Transform transform, Transform target, float angle = 0f)
	{
		transform.LookAt2D(target.position, angle);
	}

	public static float Delta(this float number, float delta)
	{
		return number + UnityEngine.Random.Range(0f - delta, delta);
	}

	public static float DeltaPercent(this float number, float percent)
	{
		return number.Delta(number * percent);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random random = new System.Random();
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
