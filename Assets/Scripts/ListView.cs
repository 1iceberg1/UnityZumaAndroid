using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListView
{
	public enum Type
	{
		Verticle,
		Horizontal
	}

	public Type listType;

	private RectTransform root;

	private List<RectTransform> items;

	private float itemSize;

	private MonoBehaviour behaviour;

	private bool isSnapScroll;

	public ListView(MonoBehaviour behaviour)
	{
		this.behaviour = behaviour;
		items = new List<RectTransform>();
	}

	public ListView SetType(Type listType)
	{
		this.listType = listType;
		return this;
	}

	public ListView SetSnapScroll(bool isSnap)
	{
		isSnapScroll = true;
		return this;
	}

	public ListView SetRoot(RectTransform root)
	{
		this.root = root;
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RectTransform item = (RectTransform)enumerator.Current;
				items.Add(item);
			}
			return this;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public ListView SetItemSize(float itemSize)
	{
		this.itemSize = itemSize;
		return this;
	}

	public void Build()
	{
		UpdateList();
	}

	private void UpdateList()
	{
		List<RectTransform> activeItems = GetActiveItems();
		float num = (!isSnapScroll) ? 0f : itemSize;
		if (listType == Type.Horizontal)
		{
			RectTransform rectTransform = root;
			float x = (float)activeItems.Count * itemSize + num;
			Vector2 sizeDelta = root.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x, sizeDelta.y);
			for (int i = 0; i < activeItems.Count; i++)
			{
				RectTransform rectTransform2 = activeItems[i];
				RectTransform transform = rectTransform2;
				float num2 = itemSize * (float)i;
				Vector2 sizeDelta2 = rectTransform2.sizeDelta;
				transform.SetLocalX(num2 + sizeDelta2.x / 2f + num / 2f);
			}
		}
		else
		{
			RectTransform rectTransform3 = root;
			Vector2 sizeDelta3 = root.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(sizeDelta3.x, (float)activeItems.Count * itemSize + num);
			for (int j = 0; j < activeItems.Count; j++)
			{
				RectTransform rectTransform4 = activeItems[j];
				RectTransform transform2 = rectTransform4;
				float num3 = (0f - itemSize) * (float)j;
				Vector2 sizeDelta4 = rectTransform4.sizeDelta;
				transform2.SetLocalY(num3 - sizeDelta4.y / 2f - num / 2f);
			}
		}
	}

	public void DisappearItems(params int[] indexes)
	{
		foreach (int index in indexes)
		{
			items[index].gameObject.SetActive(value: false);
		}
		UpdateList();
	}

	public void DisappearItemsAfterTime(float delay, params int[] indexes)
	{
		behaviour.StartCoroutine(IEDisappearItems(indexes, delay));
	}

	private IEnumerator IEDisappearItems(int[] indexes, float delay)
	{
		yield return new WaitForSeconds(delay);
		DisappearItems(indexes);
	}

	private List<RectTransform> GetActiveItems()
	{
		return items.FindAll((RectTransform x) => x.gameObject.activeSelf);
	}

	public int GetActiveItemsCount()
	{
		return GetActiveItems().Count;
	}
}
