using UnityEngine;
using UnityEngine.UI;

public class PromoteQuitDialog : YesNoDialog
{
	public Image feature;

	public Text promoteMessage;

	public Transform installBtn;

	[HideInInspector]
	public Promote promote;

	protected override void Start()
	{
		base.Start();
		onYesClick = Quit;
		promote = PromoteController.instance.GetPromote(PromoteType.QuitDialog);
		StartCoroutine(CUtils.LoadPicture(promote.featureUrl, LoadPictureComplete, promote.featureWidth, promote.featureHeight));
		if (!string.IsNullOrEmpty(promote.message))
		{
			promoteMessage.text = promote.message;
			RectTransform component = promoteMessage.transform.parent.GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			float x = promoteMessage.preferredWidth + 30f;
			Vector2 sizeDelta = component.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x, sizeDelta.y);
			if (promote.rewardType == RewardType.RemoveAds && (CUtils.IsBuyItem() || CUtils.IsAdsRemoved()))
			{
				HideText();
			}
		}
		else
		{
			HideText();
		}
	}

	private void HideText()
	{
		promoteMessage.transform.parent.gameObject.SetActive(value: false);
		installBtn.position = promoteMessage.transform.position + Vector3.up * 0.1f;
	}

	private void Quit()
	{
		Application.Quit();
	}

	public void OnInstallClick()
	{
		CUtils.OpenStore(promote.package);
		PromoteController.instance.OnInstallClick(promote.package);
		Sound.instance.PlayButton();
		Close();
	}

	private void LoadPictureComplete(Texture2D texture)
	{
		feature.sprite = CUtils.CreateSprite(texture, promote.featureWidth, promote.featureHeight);
	}
}
