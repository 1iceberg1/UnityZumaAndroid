using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class LocalizedTextMesh : MonoBehaviour
{
	public string keyValue;

	public void Awake()
	{
		LocalizeTextMesh(keyValue);
	}

	public void LocalizeTextMesh(string keyValue)
	{
		if (keyValue == null)
		{
			UnityEngine.Debug.LogError("Please set the KeyValue that should be used for this TextMesh (" + base.name + ")");
			return;
		}
		TextMesh component = base.gameObject.GetComponent<TextMesh>();
		component.text = Language.Get(keyValue);
	}
}
