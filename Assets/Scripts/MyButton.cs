using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
	protected Button button;

	protected virtual void Start()
	{
		button = GetComponent<Button>();
		if (button != null)
		{
			button.onClick.AddListener(OnButtonClick);
		}
	}

	public virtual void OnButtonClick()
	{
		Sound.instance.PlayButton();
	}
}
