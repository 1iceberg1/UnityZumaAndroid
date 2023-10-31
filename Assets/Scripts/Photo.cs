using System;
using UnityEngine;
using UnityEngine.UI;

public class Photo : MonoBehaviour
{
	public Image photo;

	[HideInInspector]
	public string url;

	private int width;

	private int height;

	protected int realWidth;

	protected int realHeight;

	private Sprite sprite;

	public Action<Sprite> onPhotoLoaded;

	public void SetSize(int width, int height)
	{
		this.width = width;
		this.height = height;
	}

	public void SetRealSize(int realWidth, int realHeight)
	{
		this.realWidth = realWidth;
		this.realHeight = realHeight;
	}

	public virtual void Load()
	{
		if (sprite != null)
		{
			SetPhoto(sprite);
		}
		else
		{
			StartCoroutine(CUtils.LoadPicture(url, MyPictureCallback, width, height));
		}
	}

	public virtual void MyPictureCallback(Texture2D texture)
	{
		if (texture == null)
		{
			Load();
			return;
		}
		Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, realWidth, realHeight), new Vector2(0.5f, 0.5f), 100f);
		SetPhoto(sprite);
		OnPhotoLoaded(sprite);
	}

	protected void SetPhoto(Sprite sprite)
	{
		this.sprite = sprite;
		if (sprite == null)
		{
			photo.SetNativeSize();
			return;
		}
		photo.sprite = sprite;
		photo.color = Color.white;
		photo.rectTransform.sizeDelta = new Vector2(width, height);
	}

	protected virtual void OnPhotoLoaded(Sprite sprite)
	{
		if (onPhotoLoaded != null)
		{
			onPhotoLoaded(sprite);
		}
	}
}
