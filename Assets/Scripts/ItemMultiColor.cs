public class ItemMultiColor : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		BallShooter.instance.CreateMultiColors();
	}
}
