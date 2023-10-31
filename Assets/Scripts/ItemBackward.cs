public class ItemBackward : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		SphereController.Backward();
		GameState.EndFlying();
	}
}
