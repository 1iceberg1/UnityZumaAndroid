using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogOverlay : MonoBehaviour
{
	private Image overlay;

	private void Awake()
	{
		overlay = GetComponent<Image>();
	}

	private void Start()
	{
		DialogController instance = DialogController.instance;
		instance.onDialogsOpened = (Action)Delegate.Combine(instance.onDialogsOpened, new Action(OnDialogOpened));
		DialogController instance2 = DialogController.instance;
		instance2.onDialogsClosed = (Action)Delegate.Combine(instance2.onDialogsClosed, new Action(OnDialogClosed));
	}

	private void OnLevelWasLoaded(int level)
	{
		overlay.enabled = false;
	}

	private void OnDialogOpened()
	{
		overlay.enabled = true;
	}

	private void OnDialogClosed()
	{
		overlay.enabled = false;
	}

	private void OnDestroy()
	{
		DialogController instance = DialogController.instance;
		instance.onDialogsOpened = (Action)Delegate.Remove(instance.onDialogsOpened, new Action(OnDialogOpened));
		DialogController instance2 = DialogController.instance;
		instance2.onDialogsClosed = (Action)Delegate.Remove(instance2.onDialogsClosed, new Action(OnDialogClosed));
	}
}
