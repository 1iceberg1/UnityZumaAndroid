public class UICamera : CCamera
{
	public static UICamera instance;

	protected override void Awake()
	{
		instance = this;
		base.Awake();
	}
}
