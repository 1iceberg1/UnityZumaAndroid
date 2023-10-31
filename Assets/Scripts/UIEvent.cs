using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Action<PointerEventData> onPointerClick;

	public Action onMouseDown;

	public Action onMouseUp;

	public Action onMouseDrag;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (onPointerClick != null)
		{
			onPointerClick(eventData);
		}
	}

	private void OnMouseDown()
	{
		if (onMouseDown != null)
		{
			onMouseDown();
		}
	}

	private void OnMouseUp()
	{
		if (onMouseUp != null)
		{
			onMouseUp();
		}
	}

	private void OnMouseDrag()
	{
		if (onMouseDrag != null)
		{
			onMouseDrag();
		}
	}
}
