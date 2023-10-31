using System;

[Serializable]
public class TweenArgs
{
	public float speed = 0.7f;

	public iTween.LoopType loopType;

	public iTween.EaseType easeType = iTween.EaseType.linear;
}
