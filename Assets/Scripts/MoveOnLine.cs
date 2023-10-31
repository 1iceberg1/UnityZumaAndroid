using UnityEngine;

public class MoveOnLine : MonoBehaviour
{
	public Transform pointA;

	public Transform pointB;

	public TweenArgs tweenArgs;

	protected virtual void Start()
	{
		if (!CUtils.EqualVector3(base.transform.position, pointA.position))
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("position", pointA.position, "speed", tweenArgs.speed, "easeType", tweenArgs.easeType, "oncomplete", "OnMoveToPointComplete"));
		}
		else
		{
			OnMoveToPointComplete();
		}
	}

	private void OnMoveToPointComplete()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("position", pointB.position, "looptype", "pingpong", "speed", tweenArgs.speed, "easeType", tweenArgs.easeType));
	}
}
