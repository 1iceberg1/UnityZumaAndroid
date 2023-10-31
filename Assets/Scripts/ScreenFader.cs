using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
	public static ScreenFader instance;

	public const float DURATION = 0.37f;

	private void Awake()
	{
		instance = this;
	}

	public void FadeOut(Action onComplete)
	{
		GetComponent<Animator>().SetTrigger("fade_out");
		GetComponent<Image>().enabled = true;
		Timer.Schedule(this, 0.37f, delegate
		{
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public void FadeIn(Action onComplete)
	{
		GetComponent<Animator>().SetTrigger("fade_in");
		Timer.Schedule(this, 0.37f, delegate
		{
			GetComponent<Image>().enabled = false;
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public void GotoScene(int sceneIndex)
	{
		FadeOut(delegate
		{
			CUtils.LoadScene(sceneIndex);
		});
	}

	private void OnLevelWasLoaded(int level)
	{
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ScreenFader_Out"))
		{
			FadeIn(null);
		}
	}
}
