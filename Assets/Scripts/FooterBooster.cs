using Superpow;
using UnityEngine;

public class FooterBooster : MonoBehaviour
{
	public int boosterID;

	public BoosterNumber boosterNumber;

	public void OnClick()
	{
		if (MainController.instance.status == MainController.Status.Completed)
		{
			return;
		}
		int num = Utils.GetBoosterNumber(boosterNumber.boosterID);
		Sphere sphere = BallShooter.instance.slots[0].sphere;
		if (num > 0)
		{
			switch (boosterID)
			{
			case 0:
				if (MonoUtils.instance.GetTotalBalls() != 0)
				{
					Utils.ChangeBoosterNumber(boosterNumber.boosterID, -1);
					MonoUtils.instance.StartCoroutine(MonoUtils.instance.RainBomb());
				}
				break;
			case 1:
				if (sphere.type != Sphere.Type.Bomb)
				{
					Utils.ChangeBoosterNumber(boosterNumber.boosterID, -1);
					BallShooter.instance.CreateBomb();
				}
				break;
			case 2:
				if (sphere.type != Sphere.Type.MultiColor)
				{
					Utils.ChangeBoosterNumber(boosterNumber.boosterID, -1);
					BallShooter.instance.CreateMultiColors();
				}
				break;
			case 3:
				if (sphere.type != Sphere.Type.Color)
				{
					Utils.ChangeBoosterNumber(boosterNumber.boosterID, -1);
					BallShooter.instance.CreateColor();
				}
				break;
			case 4:
				if (!GameState.IsBackwarding())
				{
					Utils.ChangeBoosterNumber(boosterNumber.boosterID, -1);
					SphereController.Backward();
				}
				break;
			}
		}
		else
		{
			ShopDialog shopDialog = (ShopDialog)DialogController.instance.GetDialog(DialogType.Shop);
			shopDialog.GetComponent<TabBehaviour>().SetCurrentTab(0);
			DialogController.instance.ShowDialog(shopDialog);
		}
	}
}
