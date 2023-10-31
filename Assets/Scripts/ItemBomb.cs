public class ItemBomb : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		BallShooter.instance.CreateBomb();
	}
}
