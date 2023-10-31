using UnityEngine;

public class Compliment : MonoBehaviour
{
	public enum Type
	{
		Threehit,
		Fourhit,
		GoodJob,
		Great,
		Welldone,
		Excellent,
		Awesome
	}

	public Animator anim;

	public SpriteRenderer sRenderer;

	public Sprite[] sprites;

	public void DoTask(Type type)
	{
		if (IsAvailable2Show())
		{
			sRenderer.sprite = sprites[(int)type];
			anim.SetTrigger("show");
		}
	}

	private bool IsAvailable2Show()
	{
		return anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
	}
}
