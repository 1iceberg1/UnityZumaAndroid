public class ItemColor : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		BallShooter.instance.CreateColor();
	}
}
