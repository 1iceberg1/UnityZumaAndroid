using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CryptoPlayerPrefsX
{
	private enum ArrayType
	{
		Float,
		Int32,
		Bool,
		String,
		Vector2,
		Vector3,
		Quaternion,
		Color
	}

	private static int endianDiff1;

	private static int endianDiff2;

	private static int idx;

	private static byte[] byteBlock;

	[CompilerGenerated]
	private static Action<int[], byte[], int> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<float[], byte[], int> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<Vector2[], byte[], int> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<Vector3[], byte[], int> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action<Quaternion[], byte[], int> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Action<Color[], byte[], int> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Action<List<int>, byte[]> _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Action<List<float>, byte[]> _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static Action<List<Vector2>, byte[]> _003C_003Ef__mg_0024cache8;

	[CompilerGenerated]
	private static Action<List<Vector3>, byte[]> _003C_003Ef__mg_0024cache9;

	[CompilerGenerated]
	private static Action<List<Quaternion>, byte[]> _003C_003Ef__mg_0024cacheA;

	[CompilerGenerated]
	private static Action<List<Color>, byte[]> _003C_003Ef__mg_0024cacheB;

	public static bool SetBool(string name, bool value)
	{
		try
		{
			CPlayerPrefs.SetInt(name, value ? 1 : 0);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static bool GetBool(string name)
	{
		return CPlayerPrefs.GetInt(name) == 1;
	}

	public static bool GetBool(string name, bool defaultValue)
	{
		if (CPlayerPrefs.HasKey(name))
		{
			return GetBool(name);
		}
		return defaultValue;
	}

	public static bool SetVector2(string key, Vector2 vector)
	{
		return SetFloatArray(key, new float[2]
		{
			vector.x,
			vector.y
		});
	}

	private static Vector2 GetVector2(string key)
	{
		float[] floatArray = GetFloatArray(key);
		if (floatArray.Length < 2)
		{
			return Vector2.zero;
		}
		return new Vector2(floatArray[0], floatArray[1]);
	}

	public static Vector2 GetVector2(string key, Vector2 defaultValue)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetVector2(key);
		}
		return defaultValue;
	}

	public static bool SetVector3(string key, Vector3 vector)
	{
		return SetFloatArray(key, new float[3]
		{
			vector.x,
			vector.y,
			vector.z
		});
	}

	public static Vector3 GetVector3(string key)
	{
		float[] floatArray = GetFloatArray(key);
		if (floatArray.Length < 3)
		{
			return Vector3.zero;
		}
		return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}

	public static Vector3 GetVector3(string key, Vector3 defaultValue)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetVector3(key);
		}
		return defaultValue;
	}

	public static bool SetQuaternion(string key, Quaternion vector)
	{
		return SetFloatArray(key, new float[4]
		{
			vector.x,
			vector.y,
			vector.z,
			vector.w
		});
	}

	public static Quaternion GetQuaternion(string key)
	{
		float[] floatArray = GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return Quaternion.identity;
		}
		return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetQuaternion(key);
		}
		return defaultValue;
	}

	public static bool SetColor(string key, Color color)
	{
		return SetFloatArray(key, new float[4]
		{
			color.r,
			color.g,
			color.b,
			color.a
		});
	}

	public static Color GetColor(string key)
	{
		float[] floatArray = GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	public static Color GetColor(string key, Color defaultValue)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetColor(key);
		}
		return defaultValue;
	}

	public static bool SetBoolArray(string key, bool[] boolArray)
	{
		if (boolArray.Length == 0)
		{
			UnityEngine.Debug.LogError("The bool array cannot have 0 entries when setting " + key);
			return false;
		}
		byte[] array = new byte[(boolArray.Length + 7) / 8 + 5];
		array[0] = Convert.ToByte(ArrayType.Bool);
		BitArray bitArray = new BitArray(boolArray);
		((ICollection)bitArray).CopyTo((Array)array, 5);
		Initialize();
		ConvertInt32ToBytes(boolArray.Length, array);
		return SaveBytes(key, array);
	}

	public static bool[] GetBoolArray(string key)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			byte[] array = Convert.FromBase64String(CPlayerPrefs.GetString(key));
			if (array.Length < 6)
			{
				UnityEngine.Debug.LogError("Corrupt preference file for " + key);
				return new bool[0];
			}
			if (array[0] != 2)
			{
				UnityEngine.Debug.LogError(key + " is not a boolean array");
				return new bool[0];
			}
			Initialize();
			byte[] array2 = new byte[array.Length - 5];
			Array.Copy(array, 5, array2, 0, array2.Length);
			BitArray bitArray = new BitArray(array2);
			bitArray.Length = ConvertBytesToInt32(array);
			ICollection collection = bitArray;
			bool[] array3 = new bool[collection.Count];
			collection.CopyTo(array3, 0);
			return array3;
		}
		return new bool[0];
	}

	public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetBoolArray(key);
		}
		bool[] array = new bool[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static bool SetStringArray(string key, string[] stringArray)
	{
		if (stringArray.Length == 0)
		{
			UnityEngine.Debug.LogError("The string array cannot have 0 entries when setting " + key);
			return false;
		}
		byte[] array = new byte[stringArray.Length + 1];
		array[0] = Convert.ToByte(ArrayType.String);
		Initialize();
		for (int i = 0; i < stringArray.Length; i++)
		{
			if (stringArray[i] == null)
			{
				UnityEngine.Debug.LogError("Can't save null entries in the string array when setting " + key);
				return false;
			}
			if (stringArray[i].Length > 255)
			{
				UnityEngine.Debug.LogError("Strings cannot be longer than 255 characters when setting " + key);
				return false;
			}
			array[idx++] = (byte)stringArray[i].Length;
		}
		try
		{
			CPlayerPrefs.SetString(key, Convert.ToBase64String(array) + "|" + string.Join(string.Empty, stringArray));
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static string[] GetStringArray(string key)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			string @string = CPlayerPrefs.GetString(key);
			int num = @string.IndexOf("|"[0]);
			if (num < 4)
			{
				UnityEngine.Debug.LogError("Corrupt preference file for " + key);
				return new string[0];
			}
			byte[] array = Convert.FromBase64String(@string.Substring(0, num));
			if (array[0] != 3)
			{
				UnityEngine.Debug.LogError(key + " is not a string array");
				return new string[0];
			}
			Initialize();
			int num2 = array.Length - 1;
			string[] array2 = new string[num2];
			int num3 = num + 1;
			for (int i = 0; i < num2; i++)
			{
				int num4 = array[idx++];
				if (num3 + num4 > @string.Length)
				{
					UnityEngine.Debug.LogError("Corrupt preference file for " + key);
					return new string[0];
				}
				array2[i] = @string.Substring(num3, num4);
				num3 += num4;
			}
			return array2;
		}
		return new string[0];
	}

	public static string[] GetStringArray(string key, string defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetStringArray(key);
		}
		string[] array = new string[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static bool SetIntArray(string key, int[] intArray)
	{
		return SetValue(key, intArray, ArrayType.Int32, 1, ConvertFromInt);
	}

	public static bool SetFloatArray(string key, float[] floatArray)
	{
		return SetValue(key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
	}

	public static bool SetVector2Array(string key, Vector2[] vector2Array)
	{
		return SetValue(key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
	}

	public static bool SetVector3Array(string key, Vector3[] vector3Array)
	{
		return SetValue(key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
	}

	public static bool SetQuaternionArray(string key, Quaternion[] quaternionArray)
	{
		return SetValue(key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
	}

	public static bool SetColorArray(string key, Color[] colorArray)
	{
		return SetValue(key, colorArray, ArrayType.Color, 4, ConvertFromColor);
	}

	public static int[] GetIntArray(string key)
	{
		List<int> list = new List<int>();
		GetValue(key, list, ArrayType.Int32, 1, ConvertToInt);
		return list.ToArray();
	}

	public static int[] GetIntArray(string key, int defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetIntArray(key);
		}
		int[] array = new int[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static float[] GetFloatArray(string key)
	{
		List<float> list = new List<float>();
		GetValue(key, list, ArrayType.Float, 1, ConvertToFloat);
		return list.ToArray();
	}

	public static float[] GetFloatArray(string key, float defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetFloatArray(key);
		}
		float[] array = new float[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static Vector2[] GetVector2Array(string key)
	{
		List<Vector2> list = new List<Vector2>();
		GetValue(key, list, ArrayType.Vector2, 2, ConvertToVector2);
		return list.ToArray();
	}

	public static Vector2[] GetVector2Array(string key, Vector2 defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetVector2Array(key);
		}
		Vector2[] array = new Vector2[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static Vector3[] GetVector3Array(string key)
	{
		List<Vector3> list = new List<Vector3>();
		GetValue(key, list, ArrayType.Vector3, 3, ConvertToVector3);
		return list.ToArray();
	}

	public static Vector3[] GetVector3Array(string key, Vector3 defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetVector3Array(key);
		}
		Vector3[] array = new Vector3[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static Quaternion[] GetQuaternionArray(string key)
	{
		List<Quaternion> list = new List<Quaternion>();
		GetValue(key, list, ArrayType.Quaternion, 4, ConvertToQuaternion);
		return list.ToArray();
	}

	public static Quaternion[] GetQuaternionArray(string key, Quaternion defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetQuaternionArray(key);
		}
		Quaternion[] array = new Quaternion[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static Color[] GetColorArray(string key)
	{
		List<Color> list = new List<Color>();
		GetValue(key, list, ArrayType.Color, 4, ConvertToColor);
		return list.ToArray();
	}

	public static Color[] GetColorArray(string key, Color defaultValue, int defaultSize)
	{
		if (CPlayerPrefs.HasKey(key))
		{
			return GetColorArray(key);
		}
		Color[] array = new Color[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	private static bool SetValue<T>(string key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList
	{
		if (array.Count == 0)
		{
			UnityEngine.Debug.LogError("The " + arrayType.ToString() + " array cannot have 0 entries when setting " + key);
			return false;
		}
		byte[] array2 = new byte[4 * array.Count * vectorNumber + 1];
		array2[0] = Convert.ToByte(arrayType);
		Initialize();
		for (int i = 0; i < array.Count; i++)
		{
			convert(array, array2, i);
		}
		return SaveBytes(key, array2);
	}

	private static void ConvertFromInt(int[] array, byte[] bytes, int i)
	{
		ConvertInt32ToBytes(array[i], bytes);
	}

	private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i], bytes);
	}

	private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i].x, bytes);
		ConvertFloatToBytes(array[i].y, bytes);
	}

	private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i].x, bytes);
		ConvertFloatToBytes(array[i].y, bytes);
		ConvertFloatToBytes(array[i].z, bytes);
	}

	private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i].x, bytes);
		ConvertFloatToBytes(array[i].y, bytes);
		ConvertFloatToBytes(array[i].z, bytes);
		ConvertFloatToBytes(array[i].w, bytes);
	}

	private static void ConvertFromColor(Color[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i].r, bytes);
		ConvertFloatToBytes(array[i].g, bytes);
		ConvertFloatToBytes(array[i].b, bytes);
		ConvertFloatToBytes(array[i].a, bytes);
	}

	private static void GetValue<T>(string key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
	{
		if (!CPlayerPrefs.HasKey(key))
		{
			return;
		}
		byte[] array = Convert.FromBase64String(CPlayerPrefs.GetString(key));
		if ((array.Length - 1) % (vectorNumber * 4) != 0)
		{
			UnityEngine.Debug.LogError("Corrupt preference file for " + key);
			return;
		}
		if ((ArrayType)array[0] != arrayType)
		{
			UnityEngine.Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
			return;
		}
		Initialize();
		int num = (array.Length - 1) / (vectorNumber * 4);
		for (int i = 0; i < num; i++)
		{
			convert(list, array);
		}
	}

	private static void ConvertToInt(List<int> list, byte[] bytes)
	{
		list.Add(ConvertBytesToInt32(bytes));
	}

	private static void ConvertToFloat(List<float> list, byte[] bytes)
	{
		list.Add(ConvertBytesToFloat(bytes));
	}

	private static void ConvertToVector2(List<Vector2> list, byte[] bytes)
	{
		list.Add(new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	private static void ConvertToVector3(List<Vector3> list, byte[] bytes)
	{
		list.Add(new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes)
	{
		list.Add(new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	private static void ConvertToColor(List<Color> list, byte[] bytes)
	{
		list.Add(new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	public static void ShowArrayType(string key)
	{
		byte[] array = Convert.FromBase64String(CPlayerPrefs.GetString(key));
		if (array.Length > 0)
		{
			ArrayType arrayType = (ArrayType)array[0];
			UnityEngine.Debug.Log(key + " is a " + arrayType.ToString() + " array");
		}
	}

	private static void Initialize()
	{
		if (BitConverter.IsLittleEndian)
		{
			endianDiff1 = 0;
			endianDiff2 = 0;
		}
		else
		{
			endianDiff1 = 3;
			endianDiff2 = 1;
		}
		if (byteBlock == null)
		{
			byteBlock = new byte[4];
		}
		idx = 1;
	}

	private static bool SaveBytes(string key, byte[] bytes)
	{
		try
		{
			CPlayerPrefs.SetString(key, Convert.ToBase64String(bytes));
		}
		catch
		{
			return false;
		}
		return true;
	}

	private static void ConvertFloatToBytes(float f, byte[] bytes)
	{
		byteBlock = BitConverter.GetBytes(f);
		ConvertTo4Bytes(bytes);
	}

	private static float ConvertBytesToFloat(byte[] bytes)
	{
		ConvertFrom4Bytes(bytes);
		return BitConverter.ToSingle(byteBlock, 0);
	}

	private static void ConvertInt32ToBytes(int i, byte[] bytes)
	{
		byteBlock = BitConverter.GetBytes(i);
		ConvertTo4Bytes(bytes);
	}

	private static int ConvertBytesToInt32(byte[] bytes)
	{
		ConvertFrom4Bytes(bytes);
		return BitConverter.ToInt32(byteBlock, 0);
	}

	private static void ConvertTo4Bytes(byte[] bytes)
	{
		bytes[idx] = byteBlock[endianDiff1];
		bytes[idx + 1] = byteBlock[1 + endianDiff2];
		bytes[idx + 2] = byteBlock[2 - endianDiff2];
		bytes[idx + 3] = byteBlock[3 - endianDiff1];
		idx += 4;
	}

	private static void ConvertFrom4Bytes(byte[] bytes)
	{
		byteBlock[endianDiff1] = bytes[idx];
		byteBlock[1 + endianDiff2] = bytes[idx + 1];
		byteBlock[2 - endianDiff2] = bytes[idx + 2];
		byteBlock[3 - endianDiff1] = bytes[idx + 3];
		idx += 4;
	}
}
