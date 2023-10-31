using Superpow;
using UnityEngine;

public class MapController : BaseController
{
	public Transform levelButtons;

	public RectTransform content;

	private int unlockLevel;

	protected override void Awake()
	{
		base.Awake();
		unlockLevel = LevelController.GetUnlockLevel();
		// if (unlockLevel > 20)
		// {
		// 	Vector2 sizeDelta = content.sizeDelta;
		// 	float y = sizeDelta.y;
		// 	float num = UICamera.instance.GetWidth() * 100f;
		// 	int index = Mathf.Min(unlockLevel - 1, levelButtons.childCount - 1);
		// 	Vector3 localPosition = levelButtons.GetChild(index).localPosition;
		// 	float y2 = localPosition.y;
		// 	float min = 0f - (y - num * 2f);
		// 	float max = 0f;
		// 	float value = 0f - (y2 - num * 0.6f);
		// 	value = Mathf.Clamp(value, min, max);
		// 	content.anchoredPosition = new Vector2(value, 320f);
		// }
		// else
		// {
		// 	content.anchoredPosition = new Vector2(-572f, 320f);
		// }
	}

	protected override void Start()
	{
		base.Start();
		if (!CUtils.IsGameRated() && (unlockLevel == 6 || (unlockLevel > 6 && unlockLevel % 5 == 0)) && CUtils.GetCurrentTime() - Utils.GetAskRateTime() > 1800.0)
		{
			DialogController.instance.ShowDialog(DialogType.Rate);
		}
	}

	public void OpenShop()
	{
		ShopDialog shopDialog = (ShopDialog)DialogController.instance.GetDialog(DialogType.Shop);
		shopDialog.GetComponent<TabBehaviour>().SetCurrentTab(0);
		DialogController.instance.ShowDialog(shopDialog);
	}
}
