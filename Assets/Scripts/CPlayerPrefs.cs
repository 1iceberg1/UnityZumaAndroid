using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZKW.CryptoPlayerPrefs;

public static class CPlayerPrefs
{
	private static Dictionary<string, string> keyHashs;

	private static Dictionary<string, int> xorOperands;

	private static int salt = int.MaxValue;

	private static bool _useRijndael = true;

	private static bool _useHashKey;

	private static bool _useXor;

	public static bool HasKey(string key)
	{
		string key2 = hashedKey(key);
		return PlayerPrefs.HasKey(key2);
	}

	public static void DeleteKey(string key)
	{
		string key2 = hashedKey(key);
		PlayerPrefs.DeleteKey(key2);
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static void SetInt(string key, int val)
	{
		string text = hashedKey(key);
		int num = val;
		if (_useXor)
		{
			int num2 = computeXorOperand(key, text);
			int num3 = computePlusOperand(num2);
			num = ((val + num3) ^ num2);
		}
		if (_useRijndael)
		{
			PlayerPrefs.SetString(text, encrypt(text, string.Empty + num));
		}
		else
		{
			PlayerPrefs.SetInt(text, num);
		}
	}

	public static void SetLong(string key, long val)
	{
		SetString(key, val.ToString());
	}

	public static void SetString(string key, string val)
	{
		string text = hashedKey(key);
		string text2 = val;
		if (_useXor)
		{
			int num = computeXorOperand(key, text);
			int num2 = computePlusOperand(num);
			text2 = string.Empty;
			foreach (char c in val)
			{
				char c2 = (char)((c + num2) ^ num);
				text2 += c2;
			}
		}
		if (_useRijndael)
		{
			PlayerPrefs.SetString(text, encrypt(text, text2));
		}
		else
		{
			PlayerPrefs.SetString(text, text2);
		}
	}

	public static void SetFloat(string key, float val)
	{
		SetString(key, val.ToString());
	}

	public static void SetBool(string key, bool value)
	{
		SetInt(key, value ? 1 : 0);
	}

	public static int GetInt(string key, int defaultValue)
	{
		string text = hashedKey(key);
		if (!PlayerPrefs.HasKey(text))
		{
			return defaultValue;
		}
		int num = (!_useRijndael) ? PlayerPrefs.GetInt(text) : int.Parse(decrypt(text));
		int result = num;
		if (_useXor)
		{
			int num2 = computeXorOperand(key, text);
			int num3 = computePlusOperand(num2);
			result = (num2 ^ num) - num3;
		}
		return result;
	}

	public static int GetInt(string key)
	{
		return GetInt(key, 0);
	}

	public static long GetLong(string key, long defaultValue)
	{
		return long.Parse(GetString(key, defaultValue.ToString()));
	}

	public static long GetLong(string key)
	{
		return GetLong(key, 0L);
	}

	public static string GetString(string key, string defaultValue)
	{
		string text = hashedKey(key);
		if (!PlayerPrefs.HasKey(text))
		{
			return defaultValue;
		}
		string text2 = (!_useRijndael) ? PlayerPrefs.GetString(text) : decrypt(text);
		string text3 = text2;
		if (_useXor)
		{
			int num = computeXorOperand(key, text);
			int num2 = computePlusOperand(num);
			text3 = string.Empty;
			string text4 = text2;
			foreach (char c in text4)
			{
				char c2 = (char)((num ^ c) - num2);
				text3 += c2;
			}
		}
		return text3;
	}

	public static string GetString(string key)
	{
		return GetString(key, string.Empty);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		return float.Parse(GetString(key, defaultValue.ToString()));
	}

	public static float GetFloat(string key)
	{
		return GetFloat(key, 0f);
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		if (!HasKey(key))
		{
			return defaultValue;
		}
		return GetInt(key) == 1;
	}

	public static void SetDouble(string key, double value)
	{
		PlayerPrefs.SetString(key, DoubleToString(value));
	}

	public static double GetDouble(string key, double defaultValue)
	{
		string defaultValue2 = DoubleToString(defaultValue);
		return StringToDouble(PlayerPrefs.GetString(key, defaultValue2));
	}

	public static double GetDouble(string key)
	{
		return GetDouble(key, 0.0);
	}

	private static string DoubleToString(double target)
	{
		return target.ToString("R");
	}

	private static double StringToDouble(string target)
	{
		if (string.IsNullOrEmpty(target))
		{
			return 0.0;
		}
		return double.Parse(target);
	}

	private static string encrypt(string cKey, string data)
	{
		return Helper.EncryptString(data, getEncryptionPassword(cKey));
	}

	private static string decrypt(string cKey)
	{
		return Helper.DecryptString(PlayerPrefs.GetString(cKey), getEncryptionPassword(cKey));
	}

	private static string hashedKey(string key)
	{
		if (!_useHashKey)
		{
			return key;
		}
		if (keyHashs == null)
		{
			keyHashs = new Dictionary<string, string>();
		}
		if (keyHashs.ContainsKey(key))
		{
			return keyHashs[key];
		}
		string text = hashSum(key);
		keyHashs.Add(key, text);
		return text;
	}

	private static int computeXorOperand(string key, string cryptedKey)
	{
		if (xorOperands == null)
		{
			xorOperands = new Dictionary<string, int>();
		}
		if (xorOperands.ContainsKey(key))
		{
			return xorOperands[key];
		}
		int num = 0;
		foreach (char c in cryptedKey)
		{
			num += c;
		}
		num += salt;
		xorOperands.Add(key, num);
		return num;
	}

	private static int computePlusOperand(int xor)
	{
		return xor & (xor << 1);
	}

	public static string hashSum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		byte[] array = Helper.hashBytes(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	private static string getEncryptionPassword(string pw)
	{
		return hashSum(pw + salt);
	}

	public static void setSalt(int s)
	{
		salt = s;
	}

	public static void useRijndael(bool use)
	{
		_useRijndael = use;
	}

	public static void useXor(bool use)
	{
		_useXor = use;
	}
}
