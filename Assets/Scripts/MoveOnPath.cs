using System.Linq;
using UnityEngine;

public class MoveOnPath : MonoBehaviour
{
	public Transform[] paths;

	public TweenArgs tweenArgs;

	public int currentIndex;

	private int plus = 1;

	private Vector3[] waypoints;

	private void Start()
	{
		if (waypoints == null)
		{
			waypoints = (from x in paths
				select x.position).ToArray();
		}
		MoveToPointComplete();
	}

	public void SetWaypoints(Vector3[] waypoints)
	{
		this.waypoints = waypoints;
	}

	private void MoveToPointComplete()
	{
		if (tweenArgs.loopType == iTween.LoopType.none)
		{
			if (currentIndex == waypoints.Length - 1)
			{
				return;
			}
			currentIndex++;
		}
		else if (tweenArgs.loopType == iTween.LoopType.loop)
		{
			currentIndex = (currentIndex + 1) % waypoints.Length;
		}
		else if (tweenArgs.loopType == iTween.LoopType.pingPong)
		{
			plus = (((plus != -1 || currentIndex != 0) && currentIndex != waypoints.Length - 1) ? plus : (-plus));
			currentIndex += plus;
		}
		Vector3 vector = waypoints[currentIndex];
		iTween.MoveTo(base.gameObject, iTween.Hash("position", vector, "speed", tweenArgs.speed, "easeType", tweenArgs.easeType, "oncomplete", "MoveToPointComplete"));
	}
}
