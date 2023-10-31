using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
	private class AToast
	{
		public string msg;

		public float time;

		public AToast(string msg, float time)
		{
			this.msg = msg;
			this.time = time;
		}
	}

	public RectTransform backgroundTransform;

	public RectTransform messageTransform;

	public static Toast instance;

	[HideInInspector]
	public bool isShowing;

	private Queue<AToast> queue = new Queue<AToast>();

	private void Awake()
	{
		instance = this;
	}

	public void SetMessage(string msg)
	{
		messageTransform.GetComponent<Text>().text = msg;
		RectTransform rectTransform = backgroundTransform;
		float x = messageTransform.GetComponent<Text>().preferredWidth + 30f;
		Vector2 sizeDelta = backgroundTransform.sizeDelta;
		rectTransform.sizeDelta = new Vector2(x, sizeDelta.y);
	}

	private void Show(AToast aToast)
	{
		SetMessage(aToast.msg);
		CUtils.GetChildren(base.transform).ForEach(delegate(Transform x)
		{
			x.gameObject.SetActive(value: true);
		});
		GetComponent<Animator>().SetBool("show", value: true);
		Invoke("Hide", aToast.time);
		isShowing = true;
	}

	public void ShowMessage(string msg, float time = 1.5f)
	{
		AToast item = new AToast(msg, time);
		queue.Enqueue(item);
		ShowOldestToast();
	}

	private void Hide()
	{
		GetComponent<Animator>().SetBool("show", value: false);
		Invoke("CompleteHiding", 1f);
	}

	private void CompleteHiding()
	{
		CUtils.GetChildren(base.transform).ForEach(delegate(Transform x)
		{
			x.gameObject.SetActive(value: false);
		});
		isShowing = false;
		ShowOldestToast();
	}

	private void ShowOldestToast()
	{
		if (queue.Count != 0 && !isShowing)
		{
			AToast aToast = queue.Dequeue();
			Show(aToast);
		}
	}
}
