public class ItemRainBomb : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		GameState.EndFlying();
		MonoUtils.instance.StartCoroutine(MonoUtils.instance.RainBomb());
	}
}
