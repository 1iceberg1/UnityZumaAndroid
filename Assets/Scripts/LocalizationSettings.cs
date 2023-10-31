using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class LocalizationSettings : ScriptableObject
{
	public string[] sheetTitles;

	public bool useSystemLanguagePerDefault = true;

	public string defaultLangCode = "EN";

	public static LanguageCode GetLanguageEnum(string langCode)
	{
		langCode = langCode.ToUpper();
		IEnumerator enumerator = Enum.GetValues(typeof(LanguageCode)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				LanguageCode languageCode = (LanguageCode)enumerator.Current;
				if (languageCode + string.Empty == langCode)
				{
					return languageCode;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		UnityEngine.Debug.LogError("ERORR: There is no language: [" + langCode + "]");
		return LanguageCode.EN;
	}
}
