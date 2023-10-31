using UnityEngine;

public class ItemCoin : Item
{
	protected override void DoTask()
	{
		base.DoTask();
		Vector3 middlePoint = CUtils.GetMiddlePoint(base.transform.position, MonoUtils.instance.coinMoveTarget.position, 0.5f);
		Vector3 middlePoint2 = CUtils.GetMiddlePoint(base.transform.position, MonoUtils.instance.coinMoveTarget.position, -0.5f);
		Vector3 a = (!(middlePoint.magnitude < middlePoint2.magnitude)) ? middlePoint2 : middlePoint;
		SpawnAndMoveObjects component = MainController.instance.gameObject.GetComponent<SpawnAndMoveObjects>();
		component.SetWaypoints(base.transform.position - Vector3.forward, a - Vector3.forward, MonoUtils.instance.coinMoveTarget.position - Vector3.forward);
		component.StartCoroutine(component.DoTask());
		GameState.EndFlying();
	}
}
