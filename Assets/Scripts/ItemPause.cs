public class ItemPause : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		SphereController.Pause();
		GameState.EndFlying();
	}
}
