using UnityEngine;
using UnityEngine.UI;

public class RowInviteFriends : MonoBehaviour
{
	public Text nameText;

	public RectTransform rectTransform;

	public Photo photo;

	public Toggle toggleCheck;

	public void Build(string name, string avatarUrl)
	{
		nameText.text = name;
		photo.url = avatarUrl;
		photo.SetSize(100, 100);
		photo.SetRealSize(50, 50);
		photo.Load();
	}

	public void OnToggleChanged()
	{
		Sound.instance.PlayButton();
	}
}
