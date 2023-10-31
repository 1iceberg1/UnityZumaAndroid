using UnityEngine;
using UnityEngine.UI;

public class CButton : MonoBehaviour
{
	public Image[] images;

	public Sprite[] activeSprites;

	public Sprite[] inactiveSprites;

	public Color activeTextColor;

	public Color inActiveTextColor;

	public Button button;

	public GameObject buttonText;

	private bool isActive = true;

	public bool IsActive()
	{
		return isActive;
	}

	public void SetText(string text)
	{
		if (buttonText != null)
		{
			buttonText.SetText(text);
		}
	}

	public void SetActiveSprite(bool isActive)
	{
		Debug.Log("Hello");
		this.isActive = isActive;
		if (button != null)
		{
			button.interactable = isActive;
		}
		if (buttonText != null)
		{
			buttonText.GetComponent<Text>().color = ((!isActive) ? inActiveTextColor : activeTextColor);
		}
		
		for (int i = 0; i < images.Length; i++)
		{
			
			images[i].sprite = ((!isActive) ? inactiveSprites[i] : activeSprites[i]);
			
		}
	}
}
