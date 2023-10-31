using System.Collections;
using UnityEngine;

public class ConnectServer : MonoBehaviour
{
	protected delegate void OnGetDataSuccess(string data);

	[Tooltip("Example: http://superpow.herokuapp.com/legend/")]
	public string rootUrl;

	[Tooltip("Example: 1.0")]
	public string versionAPI;

	protected IEnumerator GetDataFromServer(string url, OnGetDataSuccess onGetDataSuccess)
	{
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
			UnityEngine.Debug.Log("Error: GetDataFromServer - " + www.error);
		}
		else if (!string.IsNullOrEmpty(www.text))
		{
			onGetDataSuccess(www.text);
		}
	}
}
